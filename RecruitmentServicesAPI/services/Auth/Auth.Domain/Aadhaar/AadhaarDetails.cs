namespace RecruitmentPortal.Core.Aadhaar;

public class UserAadhaarDetails
{
    public int? Type { get; set; }
    public string? AadharNumber { get; set; }
    public string? AadharReferenceNumber { get; set; }
    public string? FullName { get; set; }
    public string? FullNameMarathi { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? MobileNo { get; set; }
    public string? State { get; set; }
    public string? District { get; set; }
    public string? Taluka { get; set; }
    public string? Address { get; set; }
    public string? Pincode { get; set; }
    public byte[]? Photo { get; set; }

    [Ignore]
    public string? IsAuthenticationType { get; set; }

    public string? AadharNumberForDisplay { get; set; }

    [Ignore]
    public string? IsHavingAadhaar { get; set; }

    public string? OtpTxn { get; set; }
    public string? Email { get; set; }
    public int? Age { get; set; }
    public string? IPAddress { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreAttribute : Attribute
{
}
