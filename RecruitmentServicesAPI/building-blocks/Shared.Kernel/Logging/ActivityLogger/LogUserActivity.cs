namespace Shared.Logging.ActivityLogger;

public class LogUserActivity
{
    public long? UserId { get; set; }
    public string? ClientIp { get; set; }
    public string? RequestUrl { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? RequestBody { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? HttpMethod { get; set; }
    public string? ResponseMessage { get; set; }
}
