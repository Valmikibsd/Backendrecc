using Auth.Application.Interfaces;
using Shared.Kernel.Interfaces;
using Shared.Kernel.Helpers;
using ClientsideEncryption;
using CrudOperations;
using RecruitmentPortal.Core.Account;
using RecruitmentPortal.Infrastructure;

namespace RecruitmentPortal.Infrastructure.Auth;

public class AuthLoginRepository(
    ICrudOperationService crudOperationService,
    ISmsService smsService) : IAuthLoginRepository
{
    private readonly ICrudOperationService _crud = crudOperationService;
    private readonly ISmsService _sms = smsService;

    #region Login

    public async Task<Response<DtoUserInfoDetails>> PasswordLogin(DtoLogin login)
    {
        var response = await GetAuthUser(login.UserName ?? "", login.LoginType ?? "Mobile");

        if (!response.Status || response.Data?.UserId == null)
        {
            return new Response<DtoUserInfoDetails>
            {
                Status = false,
                Message = "Invalid Credentials",
                Data = null
            };
        }

        var decryptedPassword = AESEncrytDecry.DecryptStringAES(response.Data.Password!);
        if (decryptedPassword != login.Password)
        {
            return new Response<DtoUserInfoDetails>
            {
                Status = false,
                Message = "Invalid Credentials",
                Data = null
            };
        }

        response.Data.Password = null;
        return new Response<DtoUserInfoDetails>
        {
            Status = true,
            Message = response.Message,
            Data = response.Data
        };
    }

    public async Task<Response> SendAuthOtp(DtoSendAuthOtp request, string otp)
    {
        var value = AuthOtpValue(request);

        return await _crud.InsertUpdateDelete<Response>(
            "[dbo].[ManageAuthOTP]",
            new
            {
                Value = value,
                OTP = otp,
                Type = request.Type,
                Purpose = request.Purpose
            },
            ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to save OTP." };
    }

    public async Task<Response<DtoUserInfoDetails>> OtpLogin(DtoAuthOtpLogin otp)
    {
        var purpose = string.IsNullOrWhiteSpace(otp.Purpose) ? "login" : otp.Purpose;
        var verify = await VerifyAuthOtp(
            new DtoSendAuthOtp
            {
                Mobile = otp.MobileNo,
                Type = "mobile",
                Purpose = purpose
            },
            otp.OTP ?? "");

        if (!verify.Status)
        {
            return new Response<DtoUserInfoDetails>
            {
                Status = false,
                Message = verify.Message
            };
        }

        var user = await GetAuthUser(otp.MobileNo ?? "", "Mobile").ConfigureAwait(false);

        if (user.Data != null)
        {
            user.Data.Password = null;
        }

        return user;
    }

    public async Task<Response<DtoUserInfoDetails>> GetAuthUser(string userName, string loginType)
    {
        return await _crud.GetSingleRecord<DtoUserInfoDetails>(
            "[dbo].[GetAuthUser]",
            new { UserName = userName, LoginType = loginType },
            ConnectionNames.Registration);
    }

    public Task<Response> SendMobileOtp(string strMobileNo, string smsText, string tempID)
    {
        return _sms.SendAsync(strMobileNo, smsText, tempID);
    }

    public async Task<int> GetOtpExpiryMinutes()
    {
        var response = await _crud.GetSingleRecord<DtoAuthOtpExpiry>(
            "[dbo].[GetAuthOtpExpiry]",
            new { },
            ConnectionNames.Registration);

        if (response.Status && response.Data != null && response.Data.OtpExpiryMinutes > 0)
        {
            return response.Data.OtpExpiryMinutes;
        }

        return 3;
    }

    private static string? AuthOtpValue(DtoSendAuthOtp request)
    {
        if (string.Equals(request.Type, "email", StringComparison.OrdinalIgnoreCase))
        {
            return request.Email;
        }

        if (string.Equals(request.Type, "department", StringComparison.OrdinalIgnoreCase))
        {
            return request.UserName;
        }

        return request.Mobile;
    }

    #endregion

    #region Forgot Password

    public async Task<Response> VerifyAuthOtp(DtoSendAuthOtp request, string otp)
    {
        var value = AuthOtpValue(request);

        return await _crud.InsertUpdateDelete<Response>(
            "[dbo].[VerifyAuthOTP]",
            new
            {
                Value = value,
                OTP = otp,
                Type = request.Type,
                Purpose = request.Purpose
            },
            ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to verify OTP." };
    }

    public async Task<Response> ForgotAuthPassword(string loginType, string accountValue, string encryptedPassword)
    {
        return await _crud.InsertUpdateDelete<Response>(
            "[dbo].[ForgotAuthPassword]",
            new { LoginType = loginType, AccountValue = accountValue, Password = encryptedPassword },
            ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to reset password." };
    }

    #endregion

    #region Change Password

    public async Task<Response<DtoUserInfoDetails>> GetAuthUserById(long userId)
    {
        return await _crud.GetSingleRecord<DtoUserInfoDetails>(
            "[dbo].[GetAuthUserById]",
            new { UserId = userId },
            ConnectionNames.Registration);
    }

    public async Task<Response> ChangeAuthPassword(long userId, string encryptedPassword)
    {
        return await _crud.InsertUpdateDelete<Response>(
            "[dbo].[ChangeAuthPassword]",
            new { UserId = userId, Password = encryptedPassword },
            ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to change password." };
    }

    #endregion

    #region Token

    public async Task<Response> SaveRefreshToken(long userId, string tokenHash, DateTime expiresAt)
    {
        return await _crud.InsertUpdateDelete<Response>(
            "[dbo].[SaveRefreshToken]",
            new { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt },
            ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to save refresh token." };
    }

    public async Task<Response<DtoUserInfoDetails>> ValidateRefreshToken(string tokenHash)
    {
        return await _crud.GetSingleRecord<DtoUserInfoDetails>(
            "[dbo].[ValidateRefreshToken]",
            new { TokenHash = tokenHash },
            ConnectionNames.Registration);
    }

    public async Task<Response> RevokeRefreshToken(string tokenHash, string? replacedByHash = null)
    {
        return await _crud.InsertUpdateDelete<Response>(
            "[dbo].[RevokeRefreshToken]",
            new { TokenHash = tokenHash, ReplacedByHash = replacedByHash },
            ConnectionNames.Registration) ?? new Response { Status = false, Message = "Unable to revoke refresh token." };
    }

    #endregion
}
