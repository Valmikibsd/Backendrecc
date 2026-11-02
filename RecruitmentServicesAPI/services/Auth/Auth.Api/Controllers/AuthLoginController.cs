using Auth.Application.Interfaces;
using Shared.Kernel.Interfaces;
using ClientsideEncryption;
using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Account;
using RecruitmentPortal.Infrastructure;
using Shared.Kernel;
using Shared.Kernel.Helpers;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthLoginController(
    IAuthLoginRepository loginRepository,
    ITokenService tokenService,
    ICaptchaService captchaService) : BaseController
{
    private readonly IAuthLoginRepository _login = loginRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ICaptchaService _captchaService = captchaService;

    #region Login

    [HttpPost("userPasswordLogin")]
    [AllowAnonymous]
    public Task<IActionResult> UserPasswordLogin([FromBody] DtoLogin dtoLogin)
    {
        dtoLogin.LoginType = "Mobile";
        return PasswordLoginAsync(dtoLogin);
    }

    [HttpPost("departmentLogin")]
    [AllowAnonymous]
    public Task<IActionResult> DepartmentLogin([FromBody] DtoLogin dtoLogin)
    {
        dtoLogin.LoginType = "Department";
        return PasswordLoginAsync(dtoLogin);
    }

    [HttpPost("sendUserOtp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendUserOtp([FromBody] DtoSendAuthOtp request)
    {
        request ??= new DtoSendAuthOtp();
        request.Type = string.IsNullOrWhiteSpace(request.Type) ? "mobile" : request.Type.Trim().ToLowerInvariant();
        request.Purpose = string.IsNullOrWhiteSpace(request.Purpose) ? "login" : request.Purpose.Trim().ToLowerInvariant();

        if ((request.Purpose == "login" || request.Purpose == "forgotpassword" || request.Purpose == "cancelapplication")
            && !_captchaService.ValidateToken(request.CaptchaToken))
        {
            return BadRequest(new Response { Status = false, Message = "CAPTCHA verification expired. A new CAPTCHA is shown. Please enter it." });
        }

        if (request.Type == "mobile" && string.IsNullOrWhiteSpace(request.Mobile))
        {
            return BadRequest(new Response { Status = false, Message = "Mobile Number is required." });
        }

        if (request.Type == "department" && string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest(new Response { Status = false, Message = "Username is required." });
        }

        if (request.Type == "email" && string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new Response { Status = false, Message = "Email is required." });
        }

        var otp = _tokenService.GenerateOtp(6);
        var response = await _login.SendAuthOtp(request, otp).ConfigureAwait(false)
            ?? new Response { Status = false, Message = "Unable to save OTP." };
        if (!response.Status)
        {
            return Conflict(response);
        }

        if (request.Type == "mobile" || request.Type == "department")
        {
            var smsMobile = request.Mobile;
            if (request.Type == "department")
            {
                var deptUser = await _login.GetAuthUser(request.UserName ?? "", "Department").ConfigureAwait(false);
                smsMobile = deptUser.Data?.MobileNo;
                if (string.IsNullOrWhiteSpace(smsMobile))
                {
                    return Conflict(new Response { Status = false, Message = "Username is not registered or mobile is not available." });
                }
            }

            var otpMinutes = await _login.GetOtpExpiryMinutes().ConfigureAwait(false);
            // REPLACE: Use LoginOtp template when separate template is configured in DLT
            //var smsText = SMSMessageHelper.LoginOtp
            //    .Replace("{#MobileOTP#}", otp)
            //    .Replace("{#var#}", otpMinutes.ToString());
            //var sms = await _login.SendMobileOtp(smsMobile ?? "", smsText, SMSTemplateIDHelper.LoginOtpTempID);

            var smsText = SMSMessageHelper.CommonOtp
                .Replace("{#MobileOTP#}", otp)
                .Replace("{#var#}", otpMinutes.ToString());
            var sms = await _login.SendMobileOtp(smsMobile ?? "", smsText, SMSTemplateIDHelper.CommonOtpTempID);
            if (!sms.Status)
            {
                return Conflict(sms);
            }

            return Ok(new Response
            {
                Status = true,
                Message = MaskedOtpMessage(smsMobile)
            });
        }

        return Ok(new Response { Status = true, Message = response.Message ?? "OTP sent successfully." });
    }

    [HttpGet("otpExpiry")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOtpExpiry()
    {
        var minutes = await _login.GetOtpExpiryMinutes().ConfigureAwait(false);
        return Ok(new { otpExpiryMinutes = minutes, Status = true });
    }

    [HttpPost("userOtpLogin")]
    [AllowAnonymous]
    public async Task<IActionResult> UserOtpLogin([FromBody] DtoAuthOtpLogin dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MobileNo) || string.IsNullOrWhiteSpace(dto.OTP))
        {
            return BadRequest(new Response { Status = false, Message = "Mobile Number and OTP are required." });
        }

        dto.Purpose = string.IsNullOrWhiteSpace(dto.Purpose) ? "login" : dto.Purpose;
        var response = await _login.OtpLogin(dto).ConfigureAwait(false);
        if (!response.Status || response.Data == null)
        {
            return BadRequest(new Response { Status = false, Message = response.Message ?? "OTP login failed." });
        }

        return Ok(await BuildLoginResult(response.Data, "Logged in successfully."));
    }

    private async Task<IActionResult> PasswordLoginAsync(DtoLogin dtoLogin)
    {
        if (string.IsNullOrWhiteSpace(dtoLogin.UserName) || string.IsNullOrWhiteSpace(dtoLogin.Password))
        {
            return BadRequest(new Response { Status = false, Message = "Username and password cannot be empty." });
        }

        if (!_captchaService.ValidateToken(dtoLogin.CaptchaToken))
        {
            return BadRequest(new Response { Status = false, Message = "CAPTCHA verification expired. A new CAPTCHA is shown. Please enter it." });
        }

        var response = await _login.PasswordLogin(dtoLogin).ConfigureAwait(false);
        if (!response.Status || response.Data == null)
        {
            return Unauthorized(new Response { Status = false, Message = response.Message ?? "Invalid Credentials" });
        }

        return Ok(await BuildLoginResult(response.Data, "Logged in successfully."));
    }

    private static string MaskedOtpMessage(string? mobile)
    {
        var value = mobile ?? "";
        if (value.Length < 4)
        {
            return "OTP sent successfully.";
        }

        return $"OTP sent to mobile ending {value[^4..]}.";
    }

    private async Task<object> BuildLoginResult(DtoUserInfoDetails user, string message, string? oldTokenHash = null)
    {
        user.AadhaarGender = TryDecrypt(user.AadhaarGender);
        user.FullName = TryDecrypt(user.FullName);
        user.FullNameMarathi = TryDecrypt(user.FullNameMarathi);

        var tokens = _tokenService.CreateAccessAndRefreshToken(user);
        var hash = _tokenService.HashRefreshToken(tokens.RefreshToken);

        if (!string.IsNullOrWhiteSpace(oldTokenHash))
        {
            await _login.RevokeRefreshToken(oldTokenHash, hash).ConfigureAwait(false);
        }

        await _login.SaveRefreshToken(user.UserId!.Value, hash, tokens.RefreshExpiresAt).ConfigureAwait(false);

        return new
        {
            user.UserId,
            user.EmailId,
            user.Username,
            user.FullName,
            user.FullNameMarathi,
            user.RoleId,
            user.RoleName,
            user.RoutePrefix,
            user.MobileNo,
            user.DepartmentId,
            user.UnitID,
            token = tokens.AccessToken,
            refreshToken = tokens.RefreshToken,
            Message = message,
            Status = true,
            user.AadhaarGender
        };
    }

    private static string? TryDecrypt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var trimmed = value.Trim();
        if (trimmed.Length < 24 || trimmed.Length % 4 != 0)
        {
            return trimmed;
        }

        try
        {
            return AESEncrytDecry.DecryptStringAES(trimmed);
        }
        catch
        {
            return trimmed;
        }
    }

    #endregion

    #region Forgot Password

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp([FromBody] DtoAuthOtpLogin dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OTP))
        {
            return BadRequest(new Response { Status = false, Message = "OTP is required." });
        }

        var purpose = string.IsNullOrWhiteSpace(dto.Purpose) ? "forgotpassword" : dto.Purpose;
        var type = string.IsNullOrWhiteSpace(dto.Type) ? "mobile" : dto.Type.Trim().ToLowerInvariant();
        var request = new DtoSendAuthOtp
        {
            Mobile = dto.MobileNo,
            UserName = dto.UserName,
            Type = type,
            Purpose = purpose
        };

        var response = await _login.VerifyAuthOtp(request, dto.OTP).ConfigureAwait(false);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpPost("forgotPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] DtoForgotAuthPassword dto)
    {
        var loginType = string.Equals(dto?.LoginType, "Department", StringComparison.OrdinalIgnoreCase)
            ? "Department"
            : "Mobile";
        var accountValue = loginType == "Department" ? dto?.UserName : dto?.MobileNo;

        if (string.IsNullOrWhiteSpace(accountValue) || string.IsNullOrWhiteSpace(dto?.Password))
        {
            return BadRequest(new Response
            {
                Status = false,
                Message = loginType == "Department"
                    ? "Username and password are required."
                    : "Mobile number and password are required."
            });
        }

        if (loginType == "Department" && !_captchaService.ValidateToken(dto.CaptchaToken))
        {
            return BadRequest(new Response { Status = false, Message = "CAPTCHA verification expired. A new CAPTCHA is shown. Please enter it." });
        }

        var user = await _login.GetAuthUser(accountValue, loginType).ConfigureAwait(false);
        if (user.Status && user.Data != null && !string.IsNullOrWhiteSpace(user.Data.Password))
        {
            var currentPassword = AESEncrytDecry.DecryptStringAES(user.Data.Password);
            if (currentPassword == dto.Password)
            {
                return Conflict(new Response { Status = false, Message = "New password must be different from current password." });
            }
        }

        var encryptedPassword = AESEncrytDecry.EncryptStringAES(dto.Password);
        var response = await _login.ForgotAuthPassword(loginType, accountValue, encryptedPassword).ConfigureAwait(false)
            ?? new Response { Status = false, Message = "Unable to update password." };

        return response.Status ? Ok(response) : Conflict(response);
    }

    #endregion

    #region Change Password

    [HttpPost("changePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] DtoChangeAuthPassword dto)
    {
        if (UserId <= 0)
        {
            return Unauthorized(new Response { Status = false, Message = "Please sign in again." });
        }

        if (string.IsNullOrWhiteSpace(dto?.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return BadRequest(new Response { Status = false, Message = "Current password and new password are required." });
        }

        var user = await _login.GetAuthUserById(UserId).ConfigureAwait(false);
        if (!user.Status || user.Data == null || string.IsNullOrWhiteSpace(user.Data.Password))
        {
            return Conflict(new Response { Status = false, Message = user.Message ?? "User not found." });
        }

        var storedPassword = AESEncrytDecry.DecryptStringAES(user.Data.Password);
        if (storedPassword != dto.CurrentPassword)
        {
            return Conflict(new Response { Status = false, Message = "Current password is incorrect." });
        }

        if (storedPassword == dto.NewPassword)
        {
            return Conflict(new Response { Status = false, Message = "New password must be different from current password." });
        }

        var encryptedPassword = AESEncrytDecry.EncryptStringAES(dto.NewPassword);
        var response = await _login.ChangeAuthPassword(UserId, encryptedPassword).ConfigureAwait(false)
            ?? new Response { Status = false, Message = "Unable to change password." };

        return response.Status ? Ok(response) : Conflict(response);
    }

    #endregion

    #region Token

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] DtoRefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            return Unauthorized(new { message = "Refresh token is required." });
        }

        var hash = _tokenService.HashRefreshToken(request.RefreshToken);
        var stored = await _login.ValidateRefreshToken(hash).ConfigureAwait(false);
        if (!stored.Status || stored.Data == null || stored.Data.UserId is null or 0)
        {
            return Unauthorized(new { message = stored.Message ?? "Invalid refresh token." });
        }

        return Ok(await BuildLoginResult(stored.Data, "Token refreshed.", oldTokenHash: hash));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] DtoRefreshTokenRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            var hash = _tokenService.HashRefreshToken(request.RefreshToken);
            await _login.RevokeRefreshToken(hash).ConfigureAwait(false);
        }

        return Ok(new { message = "User logged out successfully." });
    }

    #endregion
}
