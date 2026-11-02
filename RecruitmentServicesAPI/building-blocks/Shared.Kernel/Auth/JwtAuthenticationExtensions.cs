using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Auth;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddSharedJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                  ?? throw new InvalidOperationException("Jwt section is not configured.");

        if (string.IsNullOrWhiteSpace(jwt.Issuer) ||
            string.IsNullOrWhiteSpace(jwt.Audience) ||
            string.IsNullOrWhiteSpace(jwt.Key) ||
            jwt.Key.Length < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Issuer, Jwt:Audience and Jwt:Key (at least 32 characters) must be configured.");
        }

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            AddRoutePrefixPolicy(options, AuthPolicies.SuperAdminOnly, RolePrefixes.SuperAdmin);
            AddRoutePrefixPolicy(options, AuthPolicies.AdminOnly, RolePrefixes.Admin);
            AddRoutePrefixPolicy(options, AuthPolicies.DeptUserOnly, RolePrefixes.DeptUser);
            AddRoutePrefixPolicy(options, AuthPolicies.ApplicantOnly, RolePrefixes.Applicant);
            AddRoutePrefixPolicy(options, AuthPolicies.SupportOnly, RolePrefixes.Support);
            AddRoutePrefixPolicy(options, AuthPolicies.AccountsOnly, RolePrefixes.Accounts);
            AddRoutePrefixesPolicy(options, AuthPolicies.ReportAccess, RolePrefixes.SuperAdmin, RolePrefixes.Admin, RolePrefixes.DeptUser, RolePrefixes.Support, RolePrefixes.Accounts);
            AddRoutePrefixesPolicy(options, AuthPolicies.ViewApplicationAccess, RolePrefixes.Applicant, RolePrefixes.SuperAdmin, RolePrefixes.Admin, RolePrefixes.DeptUser, RolePrefixes.Support, RolePrefixes.Accounts);
        });
        return services;
    }

    private static void AddRoutePrefixPolicy(AuthorizationOptions options, string policyName, params string[] routePrefixes)
    {
        AddRoutePrefixesPolicy(options, policyName, routePrefixes);
    }

    private static void AddRoutePrefixesPolicy(AuthorizationOptions options, string policyName, params string[] routePrefixes)
    {
        options.AddPolicy(policyName, policy =>
            policy.RequireAssertion(context =>
            {
                var claim = context.User.FindFirst(AuthClaims.RoutePrefix)?.Value;

                if (string.IsNullOrWhiteSpace(claim) || routePrefixes == null || routePrefixes.Length == 0)
                {
                    return false;
                }

                return routePrefixes.Any(routePrefix =>
                    string.Equals(claim, routePrefix, StringComparison.OrdinalIgnoreCase));
            }));
    }
}
