using CrudOperations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Logging.ErrorLogger;

namespace Shared.Logging;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.TryAddScoped<ICrudOperationService>(sp =>
            new CrudOperationDataAccess(sp.GetRequiredService<IConfiguration>()));
        services.AddScoped<IApplicationErrorLogService, ApplicationErrorLogService>();
        return services;
    }

    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
