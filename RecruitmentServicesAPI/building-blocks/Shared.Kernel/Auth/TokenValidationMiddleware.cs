using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Shared.Auth;

/// <summary>
/// Rejects tokens blacklisted on logout. JWT stays valid until expiry;
/// logout does not change the token string, so JwtBearer would still accept it without this check.
/// </summary>
public class TokenValidationMiddleware(RequestDelegate next, IMemoryCache cache)
{
    public async Task Invoke(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(authHeader))
        {
            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? authHeader["Bearer ".Length..].Trim()
                : authHeader.Trim();

            if (!string.IsNullOrEmpty(token)
                && cache.TryGetValue(token, out bool isRevoked)
                && isRevoked)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = false,
                    message = "Session expired. Please log in again."
                }));
                return;
            }
        }

        await next(context);
    }
}
