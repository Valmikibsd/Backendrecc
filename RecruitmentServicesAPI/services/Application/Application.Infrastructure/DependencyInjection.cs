using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //var connectionString = configuration.GetConnectionString("Default")
        //    ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        //// Register repositories / Db access for RP_Application here.
        //services.AddSingleton(new ConnectionStringHolder(connectionString));
        _ = configuration;
        return services;
    }
}

public sealed class ConnectionStringHolder(string connectionString)
{
    public string ConnectionString { get; } = connectionString;
}
