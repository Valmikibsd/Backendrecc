using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        // Register repositories / Db access for RP_Notification here.
        services.AddSingleton(new ConnectionStringHolder(connectionString));
        return services;
    }
}

public sealed class ConnectionStringHolder(string connectionString)
{
    public string ConnectionString { get; } = connectionString;
}
