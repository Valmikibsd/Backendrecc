using Auth.Application.Interfaces;
using ClientsideEncryption;
using CrudOperations;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MOLCryptoEngine;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Net;
using RecruitmentPortal.Core.Aadhaar;
using RecruitmentPortal.InfraHelper;
using RecruitmentPortal.Infrastructure.InfraHelper;
using System.Data;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

#pragma warning disable SYSLIB0014
namespace RecruitmentPortal.Infrastructure.Aadhaar
{
    public class UIDService : IUIDService
    {
        private readonly AadhaarSettings options;
        private readonly ILogger<UIDService> logger;
        private readonly ICrudOperationService _crudOperationService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string StrTransIdGlobal = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss");
        private readonly WebHelper _webHelper;
        public UIDService(IOptions<AadhaarSettings> options, ILogger<UIDService> logger, ICrudOperationService crudOperationService, WebHelper webHelper, IHostEnvironment hostEnvironment)
        {
            this.options = options.Value;
            this.logger = logger;
            _crudOperationService = crudOperationService;
            _webHelper = webHelper;
            _hostEnvironment = hostEnvironment;
        }

        private string ResolveCertificatePath(string? configuredPath)
        {
            if (string.IsNullOrWhiteSpace(configuredPath))
                throw new ArgumentException("Certificate path is not configured.", nameof(configuredPath));

            // Fully qualified only (e.g. C:\certs\file.cer). Leading "/" is NOT treated as absolute.
            if (Path.IsPathFullyQualified(configuredPath))
                return configuredPath;

            return Path.GetFullPath(
                Path.Combine(_hostEnvironment.ContentRootPath, configuredPath.TrimStart('/', '\\')));
        }

        public async Task<AadharOTPResponse> SendOtpAsync(string AadhaarPlainText)
        {
            AadhaarServiceLogModel aadhaarServiceLogModel = new();
            aadhaarServiceLogModel.AadhaarNo = AESEncrytDecry.EncryptStringAES(AadhaarPlainText);
            aadhaarServiceLogModel.IPAddress = _webHelper.GetCurrentIpAddress();
            aadhaarServiceLogModel.ServiceName = "Aadhaar OTP Send";
            AadharOTPResponse aadharRes = new AadharOTPResponse();
            Random randomNumber = new Random();

            var randomNumberString = randomNumber.Next(0, 9999).ToString("0000");
            string requestDate = $"AUA-OTP-HMS-{randomNumberString}-{DateTime.Now.AddMinutes(-15).ToString("yyyy-MM-dd")}T{DateTime.Now.ToString("HH:mm:ss:fff")}";
            string strAuthXml = GetOtpxml(AadhaarPlainText, requestDate);
            try
            {
                string? asaServerUrl;
                if (options.VersionSpecifics2_5)
                {
                    asaServerUrl = options.ASAServerURL2_5;
                }
                else
                {
                    asaServerUrl = options.ASAServerURL;
                }

                if (string.IsNullOrWhiteSpace(asaServerUrl))
                {
                    throw new InvalidOperationException("ASA Server URL is not configured.");
                }
                aadhaarServiceLogModel.ServiceUrl = asaServerUrl;
                var address = new Uri(asaServerUrl);

                var request = WebRequest.Create(address) as HttpWebRequest;
                if (request != null)
                {
                    request.Method = "POST";
                    request.ContentType = "application/xml";
                    request.ContentLength = strAuthXml.Length;

                    if (asaServerUrl.Contains("https://") == true)
                    {
                        string certificateFile = ResolveCertificatePath(options.MahaCert);
                        var cert2 = X509Certificate.CreateFromCertFile(certificateFile);
                        request.ClientCertificates.Add(cert2);
                    }

                    string requestBody = strAuthXml;
                    aadhaarServiceLogModel.Request = strAuthXml;
                    byte[] byteData = Encoding.UTF8.GetBytes(requestBody);
                    request.ContentLength = byteData.Length;

                    request.Proxy = WebRequest.GetSystemWebProxy();
                    request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    request.Timeout = options.UIDTimeout;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(byteData, 0, byteData.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            StreamReader requestStream = new StreamReader(response.GetResponseStream());
                            string xml = requestStream.ReadToEnd();
                            aadhaarServiceLogModel.Response = xml;
                            if (xml != null)
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(AuthRes));
                                AuthRes? ar = (AuthRes?)serializer.Deserialize(new StringReader(xml));

                                if (ar != null && ar.otpRet?.Equals("y", StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    aadharRes.OTPTXN = requestDate;
                                    aadhaarServiceLogModel.IsError = false;
                                }
                                else
                                {
                                    aadharRes.OTPTXN = null;
                                    aadhaarServiceLogModel.IsError = true;
                                }
                            }
                        }
                        else
                        {
                            aadhaarServiceLogModel.IsError = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                aadhaarServiceLogModel.Response = ex.Message;
                aadhaarServiceLogModel.IsError = true;
                logger.LogError(ex, "Aadhaar OTP send failed.");
                throw;
            }

            finally
            {
                await InsertAadharLogDetails(aadhaarServiceLogModel);
            }

            return aadharRes;
        }
        public async Task<AadharVerifyOTPResponse> VerifyOTPAsync(string AadhaarPlainText, string OTP, string OTPTxn)
        {
            AadhaarServiceLogModel aadhaarServiceLogModel = new();
            aadhaarServiceLogModel.AadhaarNo = AESEncrytDecry.EncryptStringAES(AadhaarPlainText);
            aadhaarServiceLogModel.IPAddress = _webHelper.GetCurrentIpAddress();
            aadhaarServiceLogModel.ServiceName = "Aadhaar OTP Verify";
            AadharVerifyOTPResponse aadharVerifyOTPResponse = new();
            try
            {
                string strPidXml = otpParamXML(OTP);
                string hashdPid = string.Empty;
                string strEncPidXML = string.Empty;
                string strResult = string.Empty;
                byte[]? strNewSessionKey2_5 = null;
                if (options.VersionSpecifics2_5)
                {
                    #region 2.5
                    strNewSessionKey2_5 = getNewSessionKey2_5();
                    string ts = DateTime.Now.ToString("yyyy-MM-dd") + "T" + DateTime.Now.ToString("HH:mm:ss");
                    byte[] inputData = Encoding.UTF8.GetBytes(strPidXml);
                    byte[] cipherTextWithTS = encrypt2_5(inputData, strNewSessionKey2_5, ts);
                    byte[] srcHash = generateHash(inputData);
                    byte[] iv = generateIv(ts);
                    byte[] aad = generateAad(ts);
                    string iv_string = Encoding.ASCII.GetString(iv);
                    string aad_string = Encoding.ASCII.GetString(aad);
                    byte[] encSrcHash = encryptDecryptUsingSessionKey(true, strNewSessionKey2_5, iv, aad, srcHash);
                    string skey = encryptWithPublicKey2_5(strNewSessionKey2_5);
                    string strEncryptedXML = "";
                    aadhaarServiceLogModel.ServiceUrl = options.KUAServerURL2_5;
                    strEncryptedXML = GetAuthXml_2_5(strEncPidXML, AadhaarPlainText, skey, hashdPid, false, OTPTxn, (cipherTextWithTS), (encSrcHash));
                    aadhaarServiceLogModel.Request = strEncryptedXML;
                    strResult = AadhaarProdService(strEncryptedXML);
                    aadhaarServiceLogModel.Response = strResult;
                    #endregion
                }
                else
                {
                    string strNewSessionKey = getNewSessionKey();
                    aadhaarServiceLogModel.ServiceUrl = options.KUAServerURL;
                    strEncPidXML = encryptPidWithSessionKey(strPidXml, Convert.FromBase64String(strNewSessionKey));
                    hashdPid = computeHashMacAndEncrypt(strPidXml, Convert.FromBase64String(strNewSessionKey));
                    string strEncSessionKey = "";
                    strEncSessionKey = encryptWithPublicKey(strNewSessionKey);
                    string strEncryptedXML = "";
                    strEncryptedXML = getAuthXml(strEncPidXML, AadhaarPlainText, strEncSessionKey, hashdPid, false, OTPTxn);
                    aadhaarServiceLogModel.Request = strEncryptedXML;
                    strResult = AadhaarProdService(strEncryptedXML);
                    aadhaarServiceLogModel.Response = strResult;
                }

                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.Split('|')[0] == "ERROR")
                    {
                        aadhaarServiceLogModel.IsError = true;
                    }
                    else
                    {
                        aadhaarServiceLogModel.IsError = false;
                        aadharVerifyOTPResponse.Status = strResult.Split('|')[0];
                        aadharVerifyOTPResponse.ApplicantName = strResult.Split('|')[1];
                        aadharVerifyOTPResponse.ApplicantName_LL = strResult.Split('|')[3];
                        aadharVerifyOTPResponse.Gender = strResult.Split('|')[4];
                        aadharVerifyOTPResponse.MobileNo = strResult.Split('|')[5];
                        aadharVerifyOTPResponse.Address = strResult.Split('|')[6];
                        aadharVerifyOTPResponse.StateName = strResult.Split('|')[7];
                        aadharVerifyOTPResponse.DistrictName = strResult.Split('|')[8];
                        aadharVerifyOTPResponse.TalukaName = strResult.Split('|')[9];
                        aadharVerifyOTPResponse.Pincode = strResult.Split('|')[10];
                        aadharVerifyOTPResponse.Email = strResult.Split('|')[11];
                        aadharVerifyOTPResponse.DateOfBirth = strResult.Split('|')[12];
                        aadharVerifyOTPResponse.Age = Convert.ToInt32(strResult.Split('|')[13]);
                        aadharVerifyOTPResponse.ReferenceNo = strResult.Split('|')[14];
                        aadharVerifyOTPResponse.UIDNo = QueryStringEncryptDecrypt.EncryptQueryString("UID=" + AadhaarPlainText);
                        aadharVerifyOTPResponse.ApplicantImage_string = Convert.ToString(strResult.Split('|')[16]);
                        aadharVerifyOTPResponse.OTPTransactioNumber = strResult.Split('|')[18];
                        aadharVerifyOTPResponse.Ret = strResult.Split('|')[19];
                        aadharVerifyOTPResponse.AadharNumberForDisplay = MaskAadhaar(AadhaarPlainText);
                        aadharVerifyOTPResponse.AadharNumber = AadhaarPlainText;
                        if (string.IsNullOrWhiteSpace(aadharVerifyOTPResponse.ReferenceNo) || aadharVerifyOTPResponse.ReferenceNo == "NETWORK_ISSUE" || aadharVerifyOTPResponse.ReferenceNo.Trim().Length != 19)
                        {
                            AadhaarRefrenceResponseModel refrenceResponseModel = new();
                            refrenceResponseModel = await GetAadhaarReferenceNumberbyUID(AadhaarPlainText);
                            aadharVerifyOTPResponse.ReferenceNo = refrenceResponseModel.AadhaarReferenceNumber;
                            if (string.IsNullOrWhiteSpace(aadharVerifyOTPResponse.ReferenceNo) || aadharVerifyOTPResponse.ReferenceNo == "NETWORK_ISSUE" || aadharVerifyOTPResponse.ReferenceNo.Trim().Length != 19)
                            {
                                aadhaarServiceLogModel.IsError = true;
                                aadhaarServiceLogModel.Response = $"Network issue at {options.UidURL}?id={AadhaarPlainText}&operation=GETREFNUM";
                                aadharVerifyOTPResponse = new AadharVerifyOTPResponse();
                            }
                            else
                            {
                                await SaveAadhaarDetails(aadharVerifyOTPResponse, AadhaarPlainText);
                            }
                        }
                        else
                        {
                            await SaveAadhaarDetails(aadharVerifyOTPResponse, AadhaarPlainText);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                aadhaarServiceLogModel.Response = ex.Message;
                aadhaarServiceLogModel.IsError = true;
                logger.LogError(ex, "Aadhaar OTP verify failed.");
                throw;
            }

            //aadhaarServiceLogModel.Status = (int)Status.Active;
            finally
            {
                await InsertAadharLogDetails(aadhaarServiceLogModel);
            }

            return aadharVerifyOTPResponse;
        }

        private static string MaskAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length != 12)
            {
                return aadhaar;
            }

            return "XXXXXXXX" + aadhaar.Substring(8);
        }

        private static string DecryptOrBlank(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : AESEncrytDecry.DecryptStringAES(value);
        }

        private static string EncryptOrBlank(string? value)
        {
            var text = value?.Trim();
            return string.IsNullOrEmpty(text)
                ? string.Empty
                : AESEncrytDecry.EncryptStringAES(text);
        }

        private static string EncryptContactOrBlank(string? value)
        {
            var text = value?.Trim();
            if (string.IsNullOrEmpty(text) || text == "0")
            {
                return string.Empty;
            }

            return AESEncrytDecry.EncryptStringAES(text);
        }

        private async Task<Response> SaveAadhaarDetails(AadharVerifyOTPResponse aadharVerifyOTPResponse, string AadhaarPlainText)
        {
            var userAadhaarDetails = new UserAadhaarDetails
            {
                Type = 2,
                AadharNumber = EncryptOrBlank(AadhaarPlainText),
                AadharReferenceNumber = EncryptOrBlank(aadharVerifyOTPResponse.ReferenceNo),
                FullName = EncryptOrBlank(aadharVerifyOTPResponse.ApplicantName),
                FullNameMarathi = EncryptOrBlank(aadharVerifyOTPResponse.ApplicantName_LL),
                DateOfBirth = EncryptOrBlank(aadharVerifyOTPResponse.DateOfBirth),
                Gender = EncryptOrBlank(aadharVerifyOTPResponse.Gender),
                MobileNo = EncryptContactOrBlank(aadharVerifyOTPResponse.MobileNo),
                State = EncryptOrBlank(aadharVerifyOTPResponse.StateName),
                District = EncryptOrBlank(aadharVerifyOTPResponse.DistrictName),
                Taluka = EncryptOrBlank(aadharVerifyOTPResponse.TalukaName),
                Address = EncryptOrBlank(aadharVerifyOTPResponse.Address),
                Pincode = EncryptOrBlank(aadharVerifyOTPResponse.Pincode),
                Email = EncryptContactOrBlank(aadharVerifyOTPResponse.Email),
                Photo = string.IsNullOrWhiteSpace(aadharVerifyOTPResponse.ApplicantImage_string)
                            ? null
                            : Convert.FromBase64String(aadharVerifyOTPResponse.ApplicantImage_string),
                Age = aadharVerifyOTPResponse.Age,
                IPAddress = _webHelper.GetCurrentIpAddress() ?? string.Empty,
                AadharNumberForDisplay = aadharVerifyOTPResponse.AadharNumberForDisplay ?? string.Empty,
                OtpTxn = aadharVerifyOTPResponse.OTPTransactioNumber ?? string.Empty
            };

            return await InsertAadhaarDetails(userAadhaarDetails);
        }
        public string GetOtpxml(string AadhaarPlainText, string strdate)
        {
            var strAuthXml = string.Empty;
            if (options.VersionSpecifics2_5)
            {
                strAuthXml = @"<Auth xmlns=""http://aua.maharashtra.gov.in/auth/gom-auth-request"">
                <Txn>strdate</Txn>
                <Ver>2.5</Ver>
                <SubAUACode>MYSUB_AUA_CODE</SubAUACode>
                <AUAToken>sha256Token</AUAToken>

                <AUASkey>strAUASkey</AUASkey>
                <ReqType>REQ_TYPE</ReqType>
                <DeviceId>MY_DEVICE_ID</DeviceId> 
                <UID>MY_UID</UID>
                <type>_Type_</type>
                <Ch>CH_VALUE</Ch>
                </Auth>";

                strAuthXml = strAuthXml.Replace("strdate", strdate);
                strAuthXml = strAuthXml.Replace("MYSUB_AUA_CODE", options.SubAUACode2_5);
                strAuthXml = strAuthXml.Replace("REQ_TYPE", "otp");
                strAuthXml = strAuthXml.Replace("MY_DEVICE_ID", "TEST");
                strAuthXml = strAuthXml.Replace("CH_VALUE", "00");

                if (options.flg_IsVID_lbl_maxlength)
                {
                    strAuthXml = strAuthXml.Replace("_Type_", "V");
                }
                else
                {
                    strAuthXml = strAuthXml.Replace("_Type_", "A");
                }
                byte[]? SecurityKey = MOLSecurity.GenerateKey(256);
                byte[]? byteIV = MOLSecurity.GenerateIV(128, options.IvValue);
                string strEnxryptedPassword = MOLSecurity.AESEncrypt(options.AddharPassword, byteIV, SecurityKey);
                //string filepath = ResolveCertificatePath(options.SessionKeyEncryption);
                string strEnxryptedAUASkey = encryptWithPublicKey(Convert.ToBase64String(SecurityKey));
                string strEnxryptedUid = MOLSecurity.AESEncrypt(AadhaarPlainText, byteIV, SecurityKey);
                SHA256 sha256 = SHA256.Create();
                var encoding = new System.Text.ASCIIEncoding();

                byte[] ByteTocken = encoding.GetBytes((strEnxryptedPassword + "~" + strEnxryptedUid + "~" + ByteArrayToString(sha256.ComputeHash(encoding.GetBytes(Convert.ToBase64String(SecurityKey)))) + "~" + ByteArrayToString(sha256.ComputeHash(encoding.GetBytes(strdate)))).ToLower());
                string strTocken = ByteArrayToString(sha256.ComputeHash(ByteTocken));
                strAuthXml = strAuthXml.Replace("sha256Token", strTocken);
                strAuthXml = strAuthXml.Replace("strAUASkey", strEnxryptedAUASkey);
                strAuthXml = strAuthXml.Replace("MY_UID", strEnxryptedUid);
            }
            else
            {
                strAuthXml = @"<Auth xmlns=""http://aua.maharashtra.gov.in/auth/gom-auth-request"">
            <Txn>strdate</Txn>
            <Ver>1.1</Ver>
            <SubAUACode>MYSUB_AUA_CODE</SubAUACode>
            <ReqType>REQ_TYPE</ReqType>
            <DeviceId>MY_DEVICE_ID</DeviceId>
            <UID>MY_UID</UID>
            <Ch>CH_VALUE</Ch>
            </Auth>";
                const string strAuaDeviceId = "1224S086489";
                strAuthXml = strAuthXml.Replace("strdate", strdate);
                strAuthXml = strAuthXml.Replace("MYSUB_AUA_CODE", options.SubAUACode);
                strAuthXml = strAuthXml.Replace("REQ_TYPE", "otp");
                strAuthXml = strAuthXml.Replace("MY_DEVICE_ID", strAuaDeviceId);
                strAuthXml = strAuthXml.Replace("MY_UID", AadhaarPlainText);
                strAuthXml = strAuthXml.Replace("CH_VALUE", "00");
            }
            return strAuthXml;
        }
        public string otpParamXML(string strOTP_PIN)
        {
            string strPidXML = string.Empty;
            if (options.VersionSpecifics2_5)
            {
                strPidXML = @"<Pid ts=""TIME_STAMP"" ver=""2.0"">                
                                <Pv otp=""OTP_PIN""/> 
                                </Pid>";
            }
            else
            {

                strPidXML = @"<Pid ts=""TIME_STAMP"" ver=""2.0"">                
                                <Pv otp=""OTP_PIN""/> 
                                </Pid>";
            }
            strPidXML = strPidXML.Replace("TIME_STAMP", StrTransIdGlobal);  // E|P  exact or partial
            strPidXML = strPidXML.Replace("OTP_PIN", strOTP_PIN);  // M / F / T
            return strPidXML;
        }
        public string ByteArrayToString(byte[] data)
        {
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public byte[] getNewSessionKey2_5()
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.KeySize = 256;
                myAes.GenerateKey();
                return myAes.Key;
            }

        }
        public byte[] encrypt2_5(byte[] inputData, byte[] sessionKey, string ts)
        {

            byte[] iv = generateIv(ts);
            byte[] aad = generateAad(ts);
            byte[] cipherText = encryptDecryptUsingSessionKey(true, sessionKey, iv, aad, inputData);
            byte[] tsInBytes = Encoding.ASCII.GetBytes(ts); //ts.getBytes("UTF-8");
            byte[] packedCipherData = new byte[cipherText.Length + tsInBytes.Length];
            Array.Copy(tsInBytes, 0, packedCipherData, 0, tsInBytes.Length);
            Array.Copy(cipherText, 0, packedCipherData, tsInBytes.Length, cipherText.Length);
            return packedCipherData;
        }
        public byte[] generateHash(byte[] message)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            SHA256 sha256 = SHA256.Create();
            byte[] final = sha256.ComputeHash(message);
            return final;
        }
        public byte[] generateIv(string ts)
        {
            return getLastBits(ts, 96 / 8);
        }
        public byte[] generateAad(string ts)
        {
            return getLastBits(ts, 128 / 8);
        }
        public byte[] getLastBits(string ts, int bits)
        {
            byte[] tsInBytes = Encoding.UTF8.GetBytes(ts);
            byte[] bits1 = new byte[bits];
            Array.Copy(tsInBytes, tsInBytes.Length - bits, bits1, 0, bits);
            return bits1;
        }
        public byte[] encryptDecryptUsingSessionKey(bool cipherOperation, byte[] skey, byte[] iv, byte[] aad, byte[] data)
        {
            int AUTH_TAG_SIZE_BITS = 128;
            AeadParameters aeadParam = new AeadParameters(new KeyParameter(skey), AUTH_TAG_SIZE_BITS, iv, aad);
            GcmBlockCipher gcmb = new GcmBlockCipher(new AesEngine());
            gcmb.Init(cipherOperation, aeadParam);
            int outputSize = gcmb.GetOutputSize(data.Length);
            byte[] result = new byte[outputSize];
            int processLen = gcmb.ProcessBytes(data, 0, data.Length, result, 0);
            gcmb.DoFinal(result, processLen);
            return result;
        }
        public string encryptWithPublicKey2_5(byte[] sessionkey)
        {
            string strCertificateFile = ResolveCertificatePath(options.AuthCerFile_Preprod);
            //var strCertificateFile = _objHttpContext.Server.MapPath("~/Certificate\\" + _strUidAuthCerFile_Preprod);
            X509Certificate2 certificate = getPublicKey2_5(strCertificateFile);
            byte[] cipherbytes = sessionkey;
            RSA? rsaPublicKey = certificate.GetRSAPublicKey();
            if (rsaPublicKey == null)
            {
                throw new InvalidOperationException("Failed to load RSA public key from certificate.");
            }
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaPublicKey.ExportParameters(false));
            byte[] cipher = rsa.Encrypt(cipherbytes, false);
            return Convert.ToBase64String(cipher);
        }
        public static X509Certificate2 getPublicKey2_5(string strUIDAuthCerFile)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            string strCertificateFile = strUIDAuthCerFile;
            X509Certificate2 cert2 = new X509Certificate2(strCertificateFile);
            string strDate = cert2.GetExpirationDateString();
            return cert2;
        }
        public string GetAuthXml_2_5(string strEncPid, string strUid, string strSessionKey, string strhadPid, bool isBiometric, string otpTxn, byte[] strEncPID, byte[] strhadPid2_5)
        {

            string? strSubKuaCode;
            if (options.VersionSpecifics2_5)
            {
                strSubKuaCode = options.SubKUACode2_5;
            }
            else
            {
                strSubKuaCode = options.SubKUACode;
            }

            if (strSubKuaCode == null)
            {
                strSubKuaCode = string.Empty;
            }
            string strKUADevice_ID = "enBIO10052012XDev09";
            //string strKUADevice_ID = "UDC-CIDCO-0001";
            string strAuthXml = string.Empty;
            if (options.VersionSpecifics2_5)
            {
                strAuthXml = @"<KUAData  xmlns=""http://kua.maharashtra.gov.in/kyc/gom-kyc-request"">
                                <uid>MY_UID</uid>
                                <appCode>APP_CODE</appCode> 
                                <Token>sha256Token</Token>
                                <KUASkey>strAUASkey</KUASkey> 
                                <sa>MYKUA_KSA_CODE</sa>
                                <saTxn>TRANS_ID</saTxn>
                                <Data type=""X"">MY_PID_DATA</Data> 
                                <Hmac>HMAC_FROM_PID</Hmac> 
                                <Skey ci=""CERT_END_DATE"">SKEY_VALUE</Skey> 
                                <Uses otp=""OTP_VALUE"" pin=""n"" BT_VALUE bio=""BIO_VALUE"" pfa=""PFA_VALUE"" pa=""PA_VALUE""  pi=""PI_VALUE""/>
                                <Meta rdsId="""" rdsVer="""" dpId="""" dc="""" mi="""" mc=""""/>
                                <type>_TYPE_</type>
                                <rc>Y</rc>
                                <lr>Y</lr>
                                <pfr>N</pfr>
                </KUAData>";
            }
            else
            {
                strAuthXml = @"<KUAData  xmlns=""http://kua.maharashtra.gov.in/kyc/gom-kyc-request"">
                                <uid>MY_UID</uid>
                                <appCode>APP_CODE</appCode>  
                                <sa>MYKUA_KSA_CODE</sa>
                                <saTxn>TRANS_ID</saTxn>
                                <Data type=""X"">MY_PID_DATA</Data> 
                                <Hmac>HMAC_FROM_PID</Hmac> 
                                <Skey ci=""CERT_END_DATE"">SKEY_VALUE</Skey> 
                                <Uses otp=""OTP_VALUE"" pin=""n"" BT_VALUE bio=""BIO_VALUE"" pfa=""PFA_VALUE"" pa=""PA_VALUE""  pi=""PI_VALUE""/>
                                <Meta udc=""MY_DEVICE_ID"" rdsId="""" rdsVer="""" dpId="""" dc="""" mi="""" mc=""""/>
                               <rc>Y</rc>
                               <lr>Y</lr>
                               <pfr>N</pfr>
                </KUAData>";
            }

            strAuthXml = strAuthXml.Replace("MYKUA_KSA_CODE", strSubKuaCode);
            strAuthXml = strAuthXml.Replace("REQ_TYPE", "AUTH");
            strAuthXml = strAuthXml.Replace("MY_DEVICE_ID", strKUADevice_ID);
            //strAuthXml = strAuthXml.Replace("MY_UID", strUid);
            strAuthXml = strAuthXml.Replace("APP_CODE", "KYCApp");
            strAuthXml = strAuthXml.Replace("FDC_VALUE", "NC");
            strAuthXml = strAuthXml.Replace("SKEY_VALUE", strSessionKey);

            if (options.VersionSpecifics2_5)
            {
                strAuthXml = strAuthXml.Replace("MY_PID_DATA", Convert.ToBase64String(strEncPID));
                strAuthXml = strAuthXml.Replace("HMAC_FROM_PID", Convert.ToBase64String(strhadPid2_5));
                strAuthXml = strAuthXml.Replace("CERT_END_DATE", getCertificateExpDate_2_5());
            }
            else
            {
                strAuthXml = strAuthXml.Replace("MY_PID_DATA", strEncPid);
                strAuthXml = strAuthXml.Replace("MY_PID_DATA", strEncPid);
                strAuthXml = strAuthXml.Replace("CERT_END_DATE", getCertificateExpDate());
            }
            strAuthXml = strAuthXml.Replace("TIME_STAMP_ts", StrTransIdGlobal);
            strAuthXml = strAuthXml.Replace("TRANS_ID", "UKC:" + otpTxn);

            if (options.flg_IsVID_lbl_maxlength)
            {
                strAuthXml = strAuthXml.Replace("_TYPE_", "V");
            }
            else
            {
                strAuthXml = strAuthXml.Replace("_TYPE_", "A");
            }


            if (isBiometric == true)
            {
                strAuthXml = strAuthXml.Replace("PI_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BIO_VALUE", "y");
                strAuthXml = strAuthXml.Replace("PA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("PFA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BT_VALUE", "bt=\"FMR\"");
                strAuthXml = strAuthXml.Replace("PIP_VALUE", "10.186.6.29");
                strAuthXml = strAuthXml.Replace("LOV_VALUE", "110016");
                strAuthXml = strAuthXml.Replace("OTP_VALUE", "n");
            }
            else
            {
                strAuthXml = strAuthXml.Replace("PI_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BIO_VALUE", "n");
                strAuthXml = strAuthXml.Replace("PA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("PFA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BT_VALUE", "");
                strAuthXml = strAuthXml.Replace("PIP_VALUE", "10.186.6.29");
                strAuthXml = strAuthXml.Replace("LOV_VALUE", "110016");
                strAuthXml = strAuthXml.Replace("OTP_VALUE", "y");
            }

            byte[] SecurityKey = MOLSecurity.GenerateKey(256);
            byte[] byteIV = MOLSecurity.GenerateIV(128, options.IvValue);
            string strEnxryptedUserid = MOLSecurity.AESEncrypt(options.AddharUserName, byteIV, SecurityKey);
            string strEnxryptedPassword = MOLSecurity.AESEncrypt(options.AddharPassword, byteIV, SecurityKey);
            string strEnxryptedAUASkey = encryptWithPublicKey(Convert.ToBase64String(SecurityKey));
            string strEnxryptedUid = MOLSecurity.AESEncrypt(strUid, byteIV, SecurityKey);

            SHA256 sha256 = SHA256.Create();
            var encoding = new ASCIIEncoding();
            byte[] ByteTocken = encoding.GetBytes((strEnxryptedPassword + "~" + strEnxryptedUid + "~" + ByteArrayToString(sha256.ComputeHash(encoding.GetBytes(Convert.ToBase64String(SecurityKey)))) + "~" + ByteArrayToString(sha256.ComputeHash(encoding.GetBytes("UKC:" + otpTxn)))).ToLower());
            string strTocken = ByteArrayToString(sha256.ComputeHash(ByteTocken));
            strAuthXml = strAuthXml.Replace("sha256Token", strTocken);
            strAuthXml = strAuthXml.Replace("strAUASkey", strEnxryptedAUASkey);
            strAuthXml = strAuthXml.Replace("MY_UID", strEnxryptedUid);
            return strAuthXml;
        }
        public String getCertificateExpDate_2_5()
        {
            String strCertExpDate = "";
            if (strCertExpDate.Length != 0)
                return strCertExpDate;

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            var strCertificateFile = ResolveCertificatePath(options.AuthCerFile_Preprod);
            X509Certificate2 cert2 = new X509Certificate2(strCertificateFile);
            String strDate = cert2.GetExpirationDateString();
            DateTime dt = Convert.ToDateTime(strDate);
            strCertExpDate = dt.ToString("yyyyMMdd");
            return strCertExpDate;
        }
        public string getCertificateExpDate()
        {
            string strCertExpDate = "";
            if (strCertExpDate.Length != 0)
                return strCertExpDate;

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            var strCertificateFile = ResolveCertificatePath(options.UIDAuthCerFile);
            X509Certificate2 cert2 = new X509Certificate2(strCertificateFile);
            string strDate = cert2.GetExpirationDateString();
            DateTime dt = Convert.ToDateTime(strDate);
            strCertExpDate = dt.ToString("yyyyMMdd");
            return strCertExpDate;
        }
        public string AadhaarProdService(string strAuthXml)
        {
            string strResult = "NA";
            string? KuaServerUrl = string.Empty;
            string strCertificateFile = string.Empty;
            try
            {
                if (options.VersionSpecifics2_5)
                {
                    KuaServerUrl = options.KUAServerURL2_5;
                    strCertificateFile = ResolveCertificatePath(options.AuthCerFile_Preprod);
                }
                else
                {
                    KuaServerUrl = options.KUAServerURL;
                    strCertificateFile = ResolveCertificatePath(options.UIDAuthCerFile);
                }

                if (string.IsNullOrWhiteSpace(KuaServerUrl))
                {
                    return "NOT_RESPONDING|UID Service is not Responding. Please try again:";
                }

                var address = new Uri(KuaServerUrl);

                HttpWebRequest? request = WebRequest.Create(address) as HttpWebRequest;
                if (request == null)
                {
                    return "NOT_RESPONDING|UID Service is not Responding. Please try again:";
                }

                request.Method = "POST";
                request.ContentType = "application/xml";
                request.ContentLength = strAuthXml.Length;

                if (KuaServerUrl.Contains("https://") == true)
                {
                    X509Certificate cert2 = X509Certificate.CreateFromCertFile(strCertificateFile);
                    request.ClientCertificates.Add(cert2);
                }

                var requestBody = strAuthXml;
                var byteData = Encoding.UTF8.GetBytes(requestBody);
                request.ContentLength = byteData.Length;

                request.Proxy = WebRequest.GetSystemWebProxy();
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = options.UIDTimeout;

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(byteData, 0, byteData.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader requestStream = new StreamReader(response.GetResponseStream());
                        var xml = requestStream.ReadToEnd();

                        if (xml != null)
                        {
                            var serializer = new XmlSerializer(typeof(KuaRes));
                            var ar = (KuaRes?)serializer.Deserialize(new StringReader(xml));

                            string Track_ResponseCode = string.Empty;
                            string Track_TransactionID = string.Empty;
                            string Track_RETFlag = string.Empty;

                            if (ar != null && (ar.ret?.Equals("y", StringComparison.OrdinalIgnoreCase) == true))
                            {
                                string Ret = Convert.ToString(ar.ret);
                                string ApplicantName = Convert.ToString(ar.UidData[0].Poi[0].name);
                                string Aadharno = Convert.ToString(ar.UidData[0].uid);
                                string AadharReference_Number = Convert.ToString(ar.UidData[0].uid);
                                string ApplicantNameMarathi = Convert.ToString(ar.UidData[0].LData[0].name);

                                var TempGender = string.Empty;

                                if (Convert.ToString(ar.UidData[0].Poi[0].gender) == "M")
                                { TempGender = "Male"; }
                                else if (Convert.ToString(ar.UidData[0].Poi[0].gender) == "F")
                                { TempGender = "Female"; }
                                else
                                { TempGender = "Other"; }

                                int phone = Convert.ToInt32(ar.UidData[0].Poi[0].phone);
                                string Address = Convert.ToString(ar.UidData[0].Poa[0].house) + "," + Convert.ToString(ar.UidData[0].Poa[0].street) + "," + Convert.ToString(ar.UidData[0].Poa[0].lm) + "," + Convert.ToString(ar.UidData[0].Poa[0].po) + "," + Convert.ToInt32(ar.UidData[0].Poa[0].pc);
                                string State = Convert.ToString(ar.UidData[0].Poa[0].state);
                                string DistrictName = Convert.ToString(ar.UidData[0].Poa[0].dist);
                                string TalukaName = Convert.ToString(ar.UidData[0].Poa[0].vtc);
                                string PinCode = Convert.ToString(ar.UidData[0].Poa[0].pc);
                                string ApplicantEmail = Convert.ToString(ar.UidData[0].Poi[0].email);
                                // byte[] Base64Image = Convert.FromBase64String(ar.UidData[0].Pht);
                                string Image = ar.UidData[0].Pht;

                                int age = 0;
                                if (ar.UidData[0].Poi[0].dob.Count() == 4)
                                {
                                    int year = Convert.ToInt32(ar.UidData[0].Poi[0].dob);
                                    //DateTime strdate = Convert.ToDateTime(date);
                                    int x = year;
                                    int y = DateTime.Now.Year;
                                    int z = (y - x);
                                    age = z;
                                }
                                else
                                {
                                    DateTime AadhaarDate = DateTime.ParseExact(Convert.ToString(ar.UidData[0].Poi[0].dob.Replace("-", "/")), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    DateTime strdate = Convert.ToDateTime(AadhaarDate);
                                    int x = strdate.Year;
                                    int y = DateTime.Now.Year;
                                    int z = (y - x);
                                    age = z;
                                }

                                string DateOfBirth = Convert.ToString(ar.UidData[0].Poi[0].dob.Replace("-", "/"));


                                string EnterAadhaarNumber = Aadharno;

                                if (Aadharno != null) //Aadhaar masking
                                {
                                    Aadharno = "xxxxxxxx" + Aadharno.Substring(8, 4);
                                }

                                var encodedTextBytes = Convert.FromBase64String(ar.Rar);
                                string plainText_Token = Encoding.UTF8.GetString(encodedTextBytes);
                                string Response_TokenAadhar = plainText_Token.Substring(plainText_Token.IndexOf("{") + 1, plainText_Token.IndexOf(",") - plainText_Token.IndexOf("{") - 1);
                                Track_ResponseCode = Convert.ToString(ar.code);
                                Track_TransactionID = Convert.ToString(ar.txn);
                                Track_RETFlag = Convert.ToString(ar.ret);



                                strResult = "SUCCESS" + "|" + ApplicantName + "|" + Aadharno + "|" + ApplicantNameMarathi + "|" + TempGender
                                             + "|" + phone + "|" + Address + "|" + State + "|" + DistrictName + "|" + TalukaName
                                             + "|" + PinCode + "|" + ApplicantEmail + "|" + DateOfBirth + "|" + age + "|" + EnterAadhaarNumber + "|" + Response_TokenAadhar
                                             + "|" + Image
                                             + "|" + Track_ResponseCode
                                             + "|" + Track_TransactionID
                                             + "|" + Track_RETFlag
                                             + "|" + AadharReference_Number;

                            }
                            else
                            {

                                string Response_TokenAadhar = "";
                                Track_ResponseCode = Convert.ToString(ar?.code) ?? string.Empty;
                                Track_TransactionID = Convert.ToString(ar?.txn) ?? string.Empty;
                                Track_RETFlag = Convert.ToString(ar?.ret) ?? string.Empty;


                                string Error_Msg = "There is some problem in authenticating your Aadhaar Number. Please try again after sometime."; // (" + errorCode + ")";
                                strResult = "ERROR" + "|" + Error_Msg
                                        + "|" + Track_ResponseCode
                                        + "|" + Track_TransactionID
                                        + "|" + Track_RETFlag
                                        + "|" + Response_TokenAadhar;
                            }
                        }
                    }
                    else
                    {
                        string Error_MSG = "UID Service is not Responding. Please try again:";
                        strResult = "NOT_RESPONDING" + "|" + Error_MSG;
                    }

                    return strResult;
                }
            }
            catch
            {
                //ExceptionLogging.LogException(ex.Message);
                throw;
            }
        }
        public string getNewSessionKey()
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.KeySize = 256;
                myAes.GenerateKey();
                return Convert.ToBase64String(myAes.Key);
            }
        }
        public string encryptPidWithSessionKey(string pidXml, byte[] key)
        {

            var inputData = Encoding.UTF8.GetBytes(pidXml);
            var cipherTextWithTs = encrypt(inputData, key, StrTransIdGlobal);
            return Convert.ToBase64String(cipherTextWithTs);
        }
        public byte[] encrypt(byte[] inputData, byte[] sessionKey, string ts)
        {
            var iv = generateIv(ts);
            var aad = generateAad(ts);
            var cipherText = EncryptingSessionKeyUsingBouncyCastleAlgorithm(true, sessionKey, iv, aad, inputData);
            var tsInBytes = Encoding.UTF8.GetBytes(ts);
            var packedCipherData = new byte[cipherText.Length + tsInBytes.Length];
            Array.Copy(tsInBytes, 0, packedCipherData, 0, tsInBytes.Length);
            Array.Copy(cipherText, 0, packedCipherData, tsInBytes.Length, cipherText.Length);
            return packedCipherData;
        }
        public byte[] EncryptingSessionKeyUsingBouncyCastleAlgorithm(bool cipherOperation, byte[] skey, byte[] iv, byte[] aad, byte[] data)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(skey), 128, iv, aad);
            cipher.Init(cipherOperation, parameters);
            var cipherText = new byte[cipher.GetOutputSize(data.Length)];
            var len = cipher.ProcessBytes(data, 0, data.Length, cipherText, 0);
            cipher.DoFinal(cipherText, len);
            return cipherText;
        }
        public string computeHashMacAndEncrypt(string boimetric, byte[] key)
        {
            var encoding = new ASCIIEncoding();
            var message = encoding.GetBytes(boimetric);
            var sha256 = SHA256.Create();
            var final = sha256.ComputeHash(message);
            var iv = generateIv(StrTransIdGlobal);
            var aad = generateAad(StrTransIdGlobal);
            var enchashpid = EncryptingSessionKeyUsingBouncyCastleAlgorithm(true, key, iv, aad, final);
            return Convert.ToBase64String(enchashpid);
        }
        public string encryptWithPublicKey(string stringToEncrypt)
        {
            var certificate = getPublicKey();
            RSA? rsaPublicKey = certificate.GetRSAPublicKey();
            if (rsaPublicKey == null)
            {
                throw new InvalidOperationException("Failed to load RSA public key from certificate.");
            }
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.ImportParameters(rsaPublicKey.ExportParameters(false));
            var cipherbytes = Convert.FromBase64String(stringToEncrypt);
            var cipher = rsaCryptoServiceProvider.Encrypt(cipherbytes, false);
            return Convert.ToBase64String(cipher);
        }
        public X509Certificate2 getPublicKey()
        {
            var rsa = new RSACryptoServiceProvider();
            var strCertificateFile = ResolveCertificatePath(options.SessionKeyEncryption);
            var cert2 = new X509Certificate2(strCertificateFile);
            var strDate = cert2.GetExpirationDateString();
            return cert2;
        }
        public string getAuthXml(string strEncPid, string strUid, string strSessionKey, string strhadPid, bool isBiometric, string otpTxn)
        {
            string strSubKuaCode = options.SubKUACode ?? string.Empty;
            string strKUADevice_ID = "enBIO10052012XDev09";
            string strAuthXml = @"<KUAData  xmlns=""http://kua.maharashtra.gov.in/kyc/gom-kyc-request"">
                                <uid>MY_UID</uid>
                                <appCode>APP_CODE</appCode>  
                                <sa>MYKUA_KSA_CODE</sa>
                                <saTxn>TRANS_ID</saTxn>
                                <Data type=""X"">MY_PID_DATA</Data> 
                                <Hmac>HMAC_FROM_PID</Hmac> 
                                <Skey ci=""CERT_END_DATE"">SKEY_VALUE</Skey> 
                                <Uses otp=""OTP_VALUE"" pin=""n"" BT_VALUE bio=""BIO_VALUE"" pfa=""PFA_VALUE"" pa=""PA_VALUE""  pi=""PI_VALUE""/>
                                <Meta udc=""MY_DEVICE_ID"" rdsId="""" rdsVer="""" dpId="""" dc="""" mi="""" mc=""""/>
                               <rc>Y</rc>
                               <lr>Y</lr>
                               <pfr>N</pfr>
                </KUAData>";

            strAuthXml = strAuthXml.Replace("MYKUA_KSA_CODE", strSubKuaCode);
            strAuthXml = strAuthXml.Replace("REQ_TYPE", "AUTH");
            strAuthXml = strAuthXml.Replace("MY_DEVICE_ID", strKUADevice_ID);
            strAuthXml = strAuthXml.Replace("MY_UID", strUid);
            strAuthXml = strAuthXml.Replace("APP_CODE", "KYCApp");
            strAuthXml = strAuthXml.Replace("FDC_VALUE", "NC");
            strAuthXml = strAuthXml.Replace("SKEY_VALUE", strSessionKey);
            strAuthXml = strAuthXml.Replace("MY_PID_DATA", strEncPid);
            strAuthXml = strAuthXml.Replace("HMAC_FROM_PID", strhadPid);
            strAuthXml = strAuthXml.Replace("CERT_END_DATE", getCertificateExpDate());
            strAuthXml = strAuthXml.Replace("TIME_STAMP_ts", StrTransIdGlobal);
            strAuthXml = strAuthXml.Replace("TRANS_ID", "UKC:" + otpTxn);


            if (isBiometric == true)
            {
                strAuthXml = strAuthXml.Replace("PI_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BIO_VALUE", "y");
                strAuthXml = strAuthXml.Replace("PA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("PFA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BT_VALUE", "bt=\"FMR\"");
                strAuthXml = strAuthXml.Replace("PIP_VALUE", "10.186.6.29");
                strAuthXml = strAuthXml.Replace("LOV_VALUE", "110016");
                strAuthXml = strAuthXml.Replace("OTP_VALUE", "n");
            }
            else
            {
                strAuthXml = strAuthXml.Replace("PI_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BIO_VALUE", "n");
                strAuthXml = strAuthXml.Replace("PA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("PFA_VALUE", "n");
                strAuthXml = strAuthXml.Replace("BT_VALUE", "");
                strAuthXml = strAuthXml.Replace("PIP_VALUE", "10.186.6.29");
                strAuthXml = strAuthXml.Replace("LOV_VALUE", "110016");
                strAuthXml = strAuthXml.Replace("OTP_VALUE", "y");
            }

            return strAuthXml;
        }


        #region Aadhaar & Aadhaar Reference Number

        /// <summary>
        /// Get AadhaarReferenceNumber by UID
        /// </summary>
        /// <param name="AadhaarPlainText"></param>
        /// <returns></returns>
        public Task<AadhaarRefrenceResponseModel> GetAadhaarReferenceNumberbyUID(string AadhaarPlainText)
        {
            AadhaarRefrenceResponseModel aadhaarRefrenceResponseModel = new();
            try
            {
                aadhaarRefrenceResponseModel.AadhaarNumber = AadhaarPlainText;
                var uri = new Uri($"{options.UidURL}?id={AadhaarPlainText}&operation=GETREFNUM");
                HttpWebRequest? request = WebRequest.Create(uri) as HttpWebRequest;
                if (request != null)
                {
                    request.Method = "GET";
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            StreamReader requestStream = new StreamReader(response.GetResponseStream());
                            var xml = requestStream.ReadToEnd();
                            if (xml != null)
                            {
                                var serializer = new XmlSerializer(typeof(KuaRes));
                                var ar = (KuaRes?)serializer.Deserialize(new StringReader(xml));
                                if (ar?.ret != null)
                                {
                                    aadhaarRefrenceResponseModel.AadhaarReferenceNumber = ar.ret.ToString();
                                    if (aadhaarRefrenceResponseModel.AadhaarReferenceNumber.Length != 19)
                                    {
                                        throw new Exception("ERROR IN UIDAI WEBSITE");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get Aadhaar reference number failed.");
                throw;
            }
            return Task.FromResult(aadhaarRefrenceResponseModel);
        }

        /// <summary>
        /// Get UID by ReferenceNumber
        /// </summary>
        /// <param name="aadharRefrenceNumber"></param>
        /// <returns></returns>
        public Task<AadhaarResponseModel> GetUIDbyReferenceNumber(string aadharRefrenceNumber)
        {
            AadhaarResponseModel aadhaarResponseModel = new();
            try
            {
                aadhaarResponseModel.AadhaarReferenceNumber = aadharRefrenceNumber;
                var uri = new Uri($"{options.UidURL}?id={aadharRefrenceNumber}&operation=GETUID");
                HttpWebRequest? request = WebRequest.Create(uri) as HttpWebRequest;
                if (request != null)
                {
                    request.Method = "GET";
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                        {
                            StreamReader requestStream = new StreamReader(response.GetResponseStream());
                            var xml = requestStream.ReadToEnd();
                            if (xml != null)
                            {
                                var serializer = new XmlSerializer(typeof(KuaRes));
                                var ar = (KuaRes?)serializer.Deserialize(new StringReader(xml));
                                if (ar?.ret != null)
                                {
                                    aadhaarResponseModel.AadhaarNumber = ar.ret.ToString();
                                    if (aadhaarResponseModel.AadhaarNumber.Length != 12)
                                    {
                                        throw new Exception("ERROR IN UIDAI WEBSITE");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get UID by reference number failed.");
                throw;
            }
            return Task.FromResult(aadhaarResponseModel);
        }

        #endregion

        #region Log Details
        public async Task<Response> InsertAadharLogDetails(AadhaarServiceLogModel aadhaarServiceLogModel)
        {
            try
            {
                var result = await _crudOperationService.InsertUpdateDelete<Response>("[dbo].[InsertAadhaarLogReg]", aadhaarServiceLogModel, ConnectionNames.Registration);
                return result ?? new Response { Status = false, Message = "Failed to insert Aadhaar log." };
            }
            catch
            {
                //ExceptionLogging.LogException(ex.Message);
                throw;
            }
        }
        public async Task<Response> InsertAadhaarDetails(UserAadhaarDetails userAadhaarDetails)
        {
            try
            {
                var result = await _crudOperationService.InsertUpdateDelete<Response>("[dbo].[ManageAadhaarDetailsReg]", userAadhaarDetails, ConnectionNames.Registration);
                return result ?? new Response { Status = false, Message = "Failed to insert Aadhaar details." };
            }
            catch
            {
                //ExceptionLogging.LogException(ex.Message);
                throw;
            }
        }      
        public async Task<Response> CheckAadhaarExists(DtoAadhaar model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.AadhaarNumber))
                {
                    return new Response
                    {
                        Status = false,
                        Message = "Aadhaar Number is required."
                    };
                }

                var response = await _crudOperationService.InsertUpdateDelete<Response>(
                    "[dbo].[ManageAadhaarDetailsReg]",
                    new
                    {
                        Type = 1,
                        AadharNumber = AESEncrytDecry.EncryptStringAES(model.AadhaarNumber)                         
                    },
                    ConnectionNames.Registration).ConfigureAwait(false);

                return response ?? new Response { Status = false, Message = "Check Aadhaar failed." };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Check Aadhaar exists failed.");
                throw;
            }
        }
        public async Task<Response<DtoAadhaarProfile>> GetAadhaarDetailsAsync(DtoAadhaar model)
        {
            try
            {
                var response = await _crudOperationService.GetSingleRecord<DtoAadhaarProfile>(
                    "[dbo].[ManageAadhaarDetailsReg]",
                    new
                    {
                        Type = 3,
                        AadharNumber = model.AadhaarNumber
                    },
                    ConnectionNames.Registration);

                if (!response.Status || response.Data == null || response.Data.UserId <= 0)
                {
                    return new Response<DtoAadhaarProfile>
                    {
                        Status = false,
                        Message = "Aadhaar details not found."
                    };
                }

                response.Data.Name = DecryptOrBlank(response.Data.Name);
                response.Data.NameMarathi = DecryptOrBlank(response.Data.NameMarathi);
                response.Data.DateOfBirth = DecryptOrBlank(response.Data.DateOfBirth);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get Aadhaar details failed.");
                throw;
            }
        }
        #endregion
    }
}
