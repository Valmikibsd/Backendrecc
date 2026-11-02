using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Master.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Master service uses existing RecruitmentPortal.Infrastructure.MasterRepository
        // registered in Master.Api Program.cs against MahaRec_Masters.
        _ = configuration;
        return services;
    }
}
