using Microsoft.AspNetCore.Http;

namespace RecruitmentPortal.InfraHelper
{
    public class WebHelper(IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string GetCurrentIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return string.Empty;
            }

            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                var firstIp = forwardedFor.Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(firstIp))
                {
                    return NormalizeLocalIp(firstIp);
                }
            }
            
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return NormalizeLocalIp(realIp.Trim());
            }

            return NormalizeLocalIp(context.Connection.RemoteIpAddress?.ToString());
        }

        private static string NormalizeLocalIp(string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return string.Empty;
            }

            return ipAddress == "::1" ? "127.0.0.1" : ipAddress.Trim();
        }
    }
}
