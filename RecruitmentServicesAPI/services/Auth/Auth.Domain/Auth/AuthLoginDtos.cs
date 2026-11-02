using System.ComponentModel.DataAnnotations;

namespace RecruitmentPortal.Core.Account;

#region Login

public class DtoLogin
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    public string? Password { get; set; }

    [StringLength(20, ErrorMessage = "Login type cannot exceed 20 characters.")]
    public string? LoginType { get; set; }

    [StringLength(500, ErrorMessage = "Captcha token cannot exceed 500 characters.")]
    public string? CaptchaToken { get; set; }
}

public class DtoUserInfoDetails
{
    public long? UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? FullNameMarathi { get; set; }
    public string? EmailId { get; set; }
    public string? Password { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RoutePrefix { get; set; }
    public string? MobileNo { get; set; }
    public string? AadhaarGender { get; set; }
    public int IsActive { get; set; }
    public int? DepartmentId { get; set; }
    public string? UnitID { get; set; }
}

public class DtoAuthOtpLogin
{
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit number starting with 6-9.")]
    public string? MobileNo { get; set; }

    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "OTP is required.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit numeric code.")]
    public string? OTP { get; set; }

    [StringLength(50, ErrorMessage = "Purpose cannot exceed 50 characters.")]
    public string? Purpose { get; set; }

    [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
    public string? Type { get; set; }
}

public class DtoSendAuthOtp
{
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit number starting with 6-9.")]
    public string? Mobile { get; set; }

    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string? UserName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid Email ID format.")]
    [StringLength(100, ErrorMessage = "Email ID cannot exceed 100 characters.")]
    public string? Email { get; set; }

    [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
    public string? Type { get; set; }

    [StringLength(50, ErrorMessage = "Purpose cannot exceed 50 characters.")]
    public string? Purpose { get; set; }

    [StringLength(500, ErrorMessage = "Captcha token cannot exceed 500 characters.")]
    public string? CaptchaToken { get; set; }
}

public class DtoAuthOtpExpiry
{
    public int OtpExpiryMinutes { get; set; }
}

public class DtoGender
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

#endregion

#region Token

public class DtoRefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required.")]
    [StringLength(500, ErrorMessage = "Refresh token cannot exceed 500 characters.")]
    public string? RefreshToken { get; set; }
}

public class DtoRefreshTokenUser
{
    public bool Status { get; set; }
    public string? Message { get; set; }
    public long? UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? EmailId { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? Gender { get; set; }
    public string? RoutePrefix { get; set; }
    public string? MobileNo { get; set; }
    public int? DepartmentId { get; set; }
    public string? UnitID { get; set; }
}

#endregion

#region Forgot Password

public class DtoForgotAuthPassword
{
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be a valid 10-digit number starting with 6-9.")]
    public string? MobileNo { get; set; }

    [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string? UserName { get; set; }

    [StringLength(20, ErrorMessage = "Login type cannot exceed 20 characters.")]
    public string? LoginType { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public string? Password { get; set; }

    [StringLength(500, ErrorMessage = "Captcha token cannot exceed 500 characters.")]
    public string? CaptchaToken { get; set; }
}

#endregion

#region Change Password

public class DtoChangeAuthPassword
{
    [Required(ErrorMessage = "Current password is required.")]
    [StringLength(100, ErrorMessage = "Current password cannot exceed 100 characters.")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long.")]
    public string? NewPassword { get; set; }
}

#endregion
