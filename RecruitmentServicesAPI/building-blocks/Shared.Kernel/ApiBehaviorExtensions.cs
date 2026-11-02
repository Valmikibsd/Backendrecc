using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Kernel;

public sealed class ValidationResponse
{
    public bool Status { get; set; }
    public string Message { get; set; } = "Validation failed.";
    public Dictionary<string, string[]> Errors { get; set; } = new();
}

public static class ApiBehaviorExtensions
{
    public static IServiceCollection AddSharedApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => error.ErrorMessage)
                            .ToArray());

                return new BadRequestObjectResult(new ValidationResponse
                {
                    Status = false,
                    Errors = errors
                });
            };
        });

        return services;
    }
}
