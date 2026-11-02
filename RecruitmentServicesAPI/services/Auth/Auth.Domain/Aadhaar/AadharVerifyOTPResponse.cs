namespace RecruitmentPortal.Core.Aadhaar
{
    public class AadharVerifyOTPResponse
    {
        public string? Status { get; set; }
        public string? ApplicantName { get; set; }
        public string? ApplicantName_LL { get; set; }
        public string? Gender { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        public string? StateName { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? Pincode { get; set; }
        public string? Email { get; set; }
        public string? DateOfBirth { get; set; }
        public int Age { get; set; }
        public string? ReferenceNo { get; set; }
        public string? UIDNo { get; set; }
        public string? ApplicantImage_string { get; set; }
        public string? OTPTransactioNumber { get; set; }
        public string? Ret { get; set; }
        public string? AadharNumberForDisplay { get; set; }
        public string? AadharNumber { get; set; }
    }
}
