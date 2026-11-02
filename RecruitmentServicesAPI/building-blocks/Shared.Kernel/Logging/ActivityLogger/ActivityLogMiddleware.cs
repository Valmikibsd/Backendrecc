using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.Logging.ActivityLogger;

/// <summary>
/// Logs API activity for every request, including controllers that do not inherit BaseController.
/// </summary>
public class ActivityLogMiddleware(RequestDelegate next, ILogger<ActivityLogMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ActivityLogMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext, IServiceScopeFactory scopeFactory)
    {
        if (ActivityLogContext.ShouldSkip(httpContext))
        {
            await _next(httpContext);
            return;
        }

        await CaptureRequestBodyAsync(httpContext);

        try
        {
            await _next(httpContext);
        }
        finally
        {
            TryPersist(httpContext, scopeFactory);
        }
    }

    private async Task CaptureRequestBodyAsync(HttpContext httpContext)
    {
        try
        {
            var request = httpContext.Request;
            if (request.ContentLength is null or 0 || !request.Body.CanRead)
            {
                return;
            }

            request.EnableBuffering();
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            ActivityLogContext.SetRequestBody(httpContext, body);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to capture request body for activity log.");
        }
    }

    private void TryPersist(HttpContext httpContext, IServiceScopeFactory scopeFactory)
    {
        try
        {
            ActivityLogContext.PersistInBackground(
                scopeFactory,
                _logger,
                ActivityLogContext.Create(httpContext));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Activity logging failed unexpectedly.");
        }
    }
}
