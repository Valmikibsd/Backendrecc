using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecruitmentPortal.Core.Account;
using Shared.Auth;

namespace RecruitmentPortal.Infrastructure.Auth;

public class TokenService(IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public string GenerateOtp(int length = 6)
    {
        var max = (int)Math.Pow(10, length);
        return RandomNumberGenerator.GetInt32(0, max).ToString($"D{length}");
    }

    public AuthTokenResult CreateAccessAndRefreshToken(DtoUserInfoDetails user)
    {
        if (string.IsNullOrWhiteSpace(_jwt.Key) || _jwt.Key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters.");
        }

        var expiryMinutes = _jwt.ExpiryMinutes > 0 ? _jwt.ExpiryMinutes : 15;
        var refreshDays = _jwt.RefreshTokenDays > 0 ? _jwt.RefreshTokenDays : 7;
        var username = string.IsNullOrWhiteSpace(user.Username)
            ? (user.EmailId ?? user.MobileNo ?? "User")
            : user.Username;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()!),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, user.RoleId.ToString()),
            new(ClaimTypes.Email, user.EmailId ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.RoleName))
        {
            claims.Add(new Claim(AuthClaims.RoleName, user.RoleName.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(user.RoutePrefix))
        {
            claims.Add(new Claim(AuthClaims.RoutePrefix, user.RoutePrefix.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(user.MobileNo))
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.MobileNo.Trim()));
            claims.Add(new Claim("mobile_no", user.MobileNo.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(user.FullName))
        {
            claims.Add(new Claim("full_name", user.FullName.Trim()));
        }

        if (user.DepartmentId.HasValue && user.DepartmentId.Value > 0)
        {
            claims.Add(new Claim("department_id", user.DepartmentId.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(user.UnitID))
        {
            claims.Add(new Claim("unit_id", user.UnitID.Trim()));
        }
        var aadhaarGender = user.AadhaarGender?.Trim();
        if (!string.IsNullOrWhiteSpace(aadhaarGender))
        {
            claims.Add(new Claim("aadhaar_gender", aadhaarGender));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        var refreshBytes = RandomNumberGenerator.GetBytes(64);
        var refreshToken = Convert.ToBase64String(refreshBytes);

        return new AuthTokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            RefreshExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
        };
    }

    public string HashRefreshToken(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }
}
