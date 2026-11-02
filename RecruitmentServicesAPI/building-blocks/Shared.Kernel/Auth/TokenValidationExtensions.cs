using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Auth;

public static class TokenValidationExtensions
{
    public static IServiceCollection AddTokenValidation(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder app)
    {
        app.UseMiddleware<TokenValidationMiddleware>();
        return app;
    }
}
