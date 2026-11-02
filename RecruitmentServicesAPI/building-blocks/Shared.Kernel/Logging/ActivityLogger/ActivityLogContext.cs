using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.Logging.ActivityLogger;

internal static class ActivityLogContext
{
    private const string RequestBodyItemKey = "ActivityLog.RequestBody";

    internal static bool ShouldSkip(HttpContext httpContext)
    {
        if (HttpMethods.IsOptions(httpContext.Request.Method))
        {
            return true;
        }

        var path = httpContext.Request.Path;
        if (!path.HasValue)
        {
            return true;
        }

        var value = path.Value!;
        return value.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || value.Contains("/health", StringComparison.OrdinalIgnoreCase)
            || value.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase);
    }

    internal static void SetRequestBody(HttpContext httpContext, string? body)
    {
        if (!string.IsNullOrWhiteSpace(body))
        {
            httpContext.Items[RequestBodyItemKey] = body;
        }
    }

    internal static LogUserActivity Create(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var requestBody = httpContext.Items.TryGetValue(RequestBodyItemKey, out var bodyObj)
            ? bodyObj as string
            : null;

        return new LogUserActivity
        {
            UserId = ResolveUserId(httpContext),
            ClientIp = ResolveClientIp(httpContext),
            RequestUrl = UriHelper.GetDisplayUrl(request),
            CreatedOn = DateTime.Now,
            RequestBody = requestBody,
            HttpStatusCode = httpContext.Response.StatusCode,
            HttpMethod = request.Method
        };
    }

    internal static void PersistInBackground(
        IServiceScopeFactory scopeFactory,
        ILogger logger,
        LogUserActivity log)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var repository = scope.ServiceProvider.GetRequiredService<ILogUserActivityRepository>();
                await repository.SaveLogAsync(log).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to persist user activity log for {RequestUrl}.", log.RequestUrl);
            }
        });
    }

    private static long? ResolveUserId(HttpContext httpContext)
    {
        var raw =
            httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContext.User?.FindFirst("nameid")?.Value
            ?? httpContext.User?.FindFirst("sub")?.Value;

        return long.TryParse(raw, out var userId) && userId > 0
            ? userId
            : null;
    }

    private static string ResolveClientIp(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var firstIp = forwardedFor.Split(',')[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstIp))
            {
                return NormalizeLocalIp(firstIp);
            }
        }

        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return NormalizeLocalIp(realIp.Trim());
        }

        return NormalizeLocalIp(httpContext.Connection.RemoteIpAddress?.ToString());
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
