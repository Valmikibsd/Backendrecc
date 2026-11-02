using System.Security.Cryptography;
using System.Text;

namespace RecruitmentPortal.Infrastructure.InfraHelper;

public static class Utility
{
    // string EncryptKey = "@bm@cde@m@h@0nl!ne@11823";
    //string EncryptIV = "BMC@02@1";
    public static int CreateOtp()
    {
        int min = 100000;
        int max = 999999;
        int otp = 0;
        Random rdm = new Random();
        otp = rdm.Next(min, max);
        return otp;
    }
    public static string ReturnStrEncryptCode(string id)
    {
        string msg = string.Empty;
        msg = Encrypt(id, "sblw-3hn9-sqoy59");
        return msg;
    }
    public static string ReturnStrDecryptCode(string id)
    {
        string msg = string.Empty;
        string ids = id.Replace(" ", "+");
        msg = Decrypt(ids, "sblw-3hn9-sqoy59");
        return msg;
    }
    public static string Encrypt(string input, string key)
    {
        byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        tripleDES.Mode = CipherMode.ECB;
        tripleDES.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tripleDES.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        tripleDES.Clear();
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
    public static string Decrypt(string input, string key)
    {
        byte[] inputArray = Convert.FromBase64String(input);
        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        tripleDES.Mode = CipherMode.ECB;
        tripleDES.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tripleDES.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        tripleDES.Clear();
        return UTF8Encoding.UTF8.GetString(resultArray);
    }
    public static string SimpleTripleDes(string Data, string strKey, string striv)
    {
        byte[] key = Encoding.UTF8.GetBytes(strKey);
        byte[] iv = Encoding.UTF8.GetBytes(striv);
        byte[] data = Encoding.UTF8.GetBytes(Data);
        byte[] enc = new byte[0];
        TripleDES tdes = TripleDES.Create();
        tdes.IV = iv;
        tdes.Key = key;
        tdes.Mode = CipherMode.CBC;
        tdes.Padding = PaddingMode.Zeros;
        ICryptoTransform ict = tdes.CreateEncryptor();
        enc = ict.TransformFinalBlock(data, 0, data.Length);
        return ByteArrayToString(enc);
    }
    public static string SimpleTripleDesDecrypt(string Data, string strKey, string striv)
    {
        byte[] key = Encoding.UTF8.GetBytes(strKey);
        byte[] iv = Encoding.UTF8.GetBytes(striv);
        byte[] data = StringToByteArray(Data);
        byte[] enc = new byte[0];
        TripleDES tdes = TripleDES.Create();
        tdes.IV = iv;
        tdes.Key = key;
        tdes.Mode = CipherMode.CBC;
        tdes.Padding = PaddingMode.Zeros;
        ICryptoTransform ict = tdes.CreateDecryptor();
        enc = ict.TransformFinalBlock(data, 0, data.Length);
        return Encoding.UTF8.GetString(enc).TrimEnd('\0').TrimEnd('|');
    }
    public static byte[] StringToByteArray(string hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }
    public static string ByteArrayToString(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }
    public static DateTime datetoserver()
    {
        string zoneId = "India Standard Time";
        TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
        DateTime result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);
        return result;
    }

    public static string MaskAadhaar(string aadhaar)
    {
        if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length != 12)
            return aadhaar; // return as-is if invalid

        return "XXXXXXXX" + aadhaar.Substring(8);
    }
}
