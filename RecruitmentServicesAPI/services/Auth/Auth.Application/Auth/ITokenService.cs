using RecruitmentPortal.Core.Account;

namespace Auth.Application.Interfaces;

public interface ITokenService
{
    string GenerateOtp(int length = 6);
    AuthTokenResult CreateAccessAndRefreshToken(DtoUserInfoDetails user);
    string HashRefreshToken(string refreshToken);
}

public class AuthTokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshExpiresAt { get; set; }
}
