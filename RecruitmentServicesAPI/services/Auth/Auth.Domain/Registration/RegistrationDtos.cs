using System.ComponentModel.DataAnnotations;

namespace RecruitmentPortal.Core.Registration;

#region Registration

public class DtoRegistration
{
    public long UserId { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Email ID is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email ID format.")]
    [StringLength(100, ErrorMessage = "Email ID cannot exceed 100 characters.")]
    public string? EmailId { get; set; }

    [Required(ErrorMessage = "Mobile number is required.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit number starting with 6-9.")]
    public string? MobileNo { get; set; }

    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Alternate mobile number must be a valid 10-digit number starting with 6-9.")]
    public string? AlternateMobileNo { get; set; }

    public bool? IsWhatsapp { get; set; }
    public string? IPAddress { get; set; }
}

#endregion

#region OTP

public class DtoEmailAndMobileOTP
{
    [EmailAddress(ErrorMessage = "Invalid Email ID format.")]
    [StringLength(100, ErrorMessage = "Email ID cannot exceed 100 characters.")]
    public string? Email { get; set; }

    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit number starting with 6-9.")]
    public string? Mobile { get; set; }

    [StringLength(10, ErrorMessage = "OTP cannot exceed 10 characters.")]
    public string? OTP { get; set; }

    [StringLength(20, ErrorMessage = "Type cannot exceed 20 characters.")]
    public string? Type { get; set; }

    [StringLength(500, ErrorMessage = "Captcha token cannot exceed 500 characters.")]
    public string? CaptchaToken { get; set; }
}

#endregion
