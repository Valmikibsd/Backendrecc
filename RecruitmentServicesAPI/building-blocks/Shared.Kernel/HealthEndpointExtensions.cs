using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Shared.Kernel;

public static class HealthEndpointExtensions
{
    public static WebApplication MapSharedHealth(this WebApplication app, string serviceName)
    {
        app.MapGet("/health", () => Results.Ok(new
        {
            service = serviceName,
            status = "Healthy",
            utcNow = DateTime.UtcNow
        }));
        return app;
    }
}
