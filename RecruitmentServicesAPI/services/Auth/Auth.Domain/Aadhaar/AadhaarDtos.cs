using System.ComponentModel.DataAnnotations;

namespace RecruitmentPortal.Core.Aadhaar;

#region API

public class DtoAadhaar
{
    [StringLength(250, ErrorMessage = "Aadhaar number cannot exceed 250 characters.")]
    public string? AadhaarNumber { get; set; }

    [StringLength(500, ErrorMessage = "Captcha token cannot exceed 500 characters.")]
    public string? CaptchaToken { get; set; }

    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit numeric code.")]
    public string? OTP { get; set; }

    [StringLength(250, ErrorMessage = "OTP Transaction ID cannot exceed 250 characters.")]
    public string? OTPTxn { get; set; }
}

public class DtoAadhaarProfile
{
    public long UserId { get; set; }

    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string? Name { get; set; }

    [StringLength(200, ErrorMessage = "Marathi Name cannot exceed 200 characters.")]
    public string? NameMarathi { get; set; }

    [StringLength(20, ErrorMessage = "Date of Birth cannot exceed 20 characters.")]
    public string? DateOfBirth { get; set; }

    [StringLength(20, ErrorMessage = "Masked Aadhaar Number cannot exceed 20 characters.")]
    public string? MaskedAadhaarNo { get; set; }
}

#endregion
