using CrudOperations;
using RecruitmentPortal.Core.Account;

namespace Auth.Application.Interfaces;

public interface IAuthLoginRepository
{
    #region Login

    Task<Response<DtoUserInfoDetails>> PasswordLogin(DtoLogin login);
    Task<Response> SendAuthOtp(DtoSendAuthOtp request, string otp);
    Task<Response<DtoUserInfoDetails>> OtpLogin(DtoAuthOtpLogin otp);
    Task<Response<DtoUserInfoDetails>> GetAuthUser(string userName, string loginType);
    Task<Response> SendMobileOtp(string strMobileNo, string strBody, string tempID);
    Task<int> GetOtpExpiryMinutes();

    #endregion

    #region Forgot Password

    Task<Response> VerifyAuthOtp(DtoSendAuthOtp request, string otp);
    Task<Response> ForgotAuthPassword(string loginType, string accountValue, string encryptedPassword);

    #endregion

    #region Change Password

    Task<Response<DtoUserInfoDetails>> GetAuthUserById(long userId);
    Task<Response> ChangeAuthPassword(long userId, string encryptedPassword);

    #endregion

    #region Token

    Task<Response> SaveRefreshToken(long userId, string tokenHash, DateTime expiresAt);
    Task<Response<DtoUserInfoDetails>> ValidateRefreshToken(string tokenHash);
    Task<Response> RevokeRefreshToken(string tokenHash, string? replacedByHash = null);

    #endregion
}
