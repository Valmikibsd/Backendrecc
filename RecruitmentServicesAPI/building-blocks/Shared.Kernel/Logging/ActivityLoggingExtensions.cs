using CrudOperations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Logging.ActivityLogger;

namespace Shared.Logging;

public static class ActivityLoggingExtensions
{
    /// <summary>
    /// Registers activity logging services. Middleware logs every request,
    /// including controllers that do not inherit BaseController.
    /// </summary>
    public static IServiceCollection AddActivityLogging(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.TryAddScoped<ICrudOperationService>(sp =>
            new CrudOperationDataAccess(sp.GetRequiredService<IConfiguration>()));
        services.AddScoped<ILogUserActivityRepository, LogUserActivityRepository>();
        return services;
    }

    /// <summary>
    /// Adds activity-log middleware. Call after UseAuthentication / UseAuthorization so UserId is available.
    /// </summary>
    public static IApplicationBuilder UseActivityLogging(this WebApplication app)
    {
        app.UseMiddleware<ActivityLogMiddleware>();
        return app;
    }
}
