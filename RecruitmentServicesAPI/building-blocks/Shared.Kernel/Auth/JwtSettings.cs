namespace Shared.Auth;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "StateRecruitment";
    public string Audience { get; set; } = "StateRecruitmentClients";
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
}

