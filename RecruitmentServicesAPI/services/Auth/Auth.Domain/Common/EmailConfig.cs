namespace RecruitmentPortal.Core.Email;

public class EmailConfig
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FromEmail { get; set; }
    public bool EnableSSL { get; set; }
    public int Timeout { get; set; }
}
