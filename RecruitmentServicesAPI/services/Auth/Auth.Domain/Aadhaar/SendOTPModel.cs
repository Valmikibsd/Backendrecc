namespace RecruitmentPortal.Core.Aadhaar
{
    public class SendOTPModel
    {
        public string? AadhaarNumber { get; set; }
        public string? CaptchaToken { get; set; }
    }
    public class CheckAadhaarModel
    {
        public string? AadhaarNumber { get; set; }
    }
}
