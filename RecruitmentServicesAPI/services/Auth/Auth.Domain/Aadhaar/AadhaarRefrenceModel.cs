namespace RecruitmentPortal.Core.Aadhaar
{
    public class AadhaarRefrenceRequestModel
    {
        public string? AadhaarNumber { get; set; }
    }

    public class AadhaarRefrenceResponseModel
    {
        public string? AadhaarNumber { get; set; }
        public string? AadhaarReferenceNumber { get; set; }
    }

}
