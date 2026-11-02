using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Kernel.Encryption;

public static class RequestResponseEncryptionExtensions
{
    public static IServiceCollection AddRequestResponseEncryption(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RequestResponseEncryptionSettings>(
            configuration.GetSection(RequestResponseEncryptionSettings.SectionName));
        services.AddSingleton<RequestResponseCryptoHelper>();
        return services;
    }

    /// <summary>
    /// Decrypts/encrypts /api traffic at the gateway only. Downstream services stay plain JSON.
    /// </summary>
    public static IApplicationBuilder UseRequestResponseEncryption(this IApplicationBuilder app)
    {
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/api"),
            branch => branch.UseMiddleware<RequestResponseEncryptionMiddleware>());
        return app;
    }
}
