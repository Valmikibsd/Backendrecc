using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Shared.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSharedSerilog(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        var defaultLevel = ParseLevel(builder.Configuration["Logging:LogLevel:Default"]);
        var aspNetLevel = ParseLevel(builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore"]);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(defaultLevel)
            .MinimumLevel.Override("Microsoft", aspNetLevel)
            .MinimumLevel.Override("Microsoft.AspNetCore", aspNetLevel)
            .MinimumLevel.Override("System", aspNetLevel)
            .MinimumLevel.Override("Yarp", aspNetLevel)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console()
            .WriteTo.File(
                path: $"logs/{serviceName.ToLowerInvariant()}-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14)
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }

    private static LogEventLevel ParseLevel(string? value) =>
        Enum.TryParse<LogEventLevel>(value, ignoreCase: true, out var level)
            ? level
            : LogEventLevel.Warning;
}
