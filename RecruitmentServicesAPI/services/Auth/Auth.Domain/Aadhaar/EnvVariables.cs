namespace RecruitmentPortal.Core.Aadhaar
{
    public class AadhaarSettings
    {
        public string? UidURL { get; set; }
        public string? AddharUserName { get; set; }
        public string? AddharPassword { get; set; }
        public string? IvValue { get; set; }
        public bool VersionSpecifics2_5 { get; set; }      
        public string? SubAUACode2_5 { get; set; }
        public bool flg_IsVID_lbl_maxlength { get; set; }
        public string? SessionKeyEncryption { get; set; }
        public string? SubAUACode { get; set; }
        public string? ASAServerURL2_5 { get; set; }
        public string? ASAServerURL { get; set; }
        public string? MahaCert { get; set; }
        public int UIDTimeout { get; set; }
        public string? AuthCerFile_Preprod { get; set; }
        public string? UIDAuthCerFile { get; set; }
        public string? SubKUACode2_5 { get; set; }
        public string? SubKUACode { get; set; }
        public string? KUAServerURL2_5 { get; set; }
        public string? KUAServerURL { get; set; }       
    }
}
