using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Kernel;

[ApiController]
public class BaseController : ControllerBase
{
    protected long UserId
    {
        get
        {
            var raw =
                User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("nameid")?.Value
                ?? User?.FindFirst("sub")?.Value;

            return long.TryParse(raw, out var userId) ? userId : 0;
        }
    }
    protected string? UserName =>
        User?.Identity?.Name
        ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    protected string? UserEmail =>
        User?.FindFirst(ClaimTypes.Email)?.Value
        ?? User?.FindFirst("email")?.Value;

    protected string? UserRoleId =>
        User?.FindFirst(ClaimTypes.Role)?.Value
        ?? User?.FindFirst("role")?.Value;

    protected string? UserRoleName =>
        User?.FindFirst(Shared.Auth.AuthClaims.RoleName)?.Value
        ?? User?.FindFirst("role_name")?.Value;

    protected string? UserRoutePrefix =>
        User?.FindFirst(Shared.Auth.AuthClaims.RoutePrefix)?.Value
        ?? User?.FindFirst("route_prefix")?.Value
        ?? UserRoleName;

    protected string? UserMobile =>
        User?.FindFirst(ClaimTypes.MobilePhone)?.Value
        ?? User?.FindFirst("mobile_no")?.Value;

    protected string? UserFullName =>
        User?.FindFirst("full_name")?.Value
        ?? UserName;
    protected string? AadhaarGender =>
        User?.FindFirst("aadhaar_gender")?.Value
        ?? string.Empty;

    protected int UserDepartmentId
    {
        get
        {
            var raw = User?.FindFirst("department_id")?.Value;
            return int.TryParse(raw, out var deptId) ? deptId : 0;
        }
    }

    protected string UserUnitId =>
        User?.FindFirst("unit_id")?.Value ?? string.Empty;

    protected IActionResult? InvalidUserResponse()
    {
        if (UserId <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Invalid UserId."
            });
        }

        return null;
    }

    protected string IpAddress
    {
        get
        {
            var httpContext = HttpContext;
            if (httpContext is null)
            {
                return string.Empty;
            }

            var request = httpContext.Request;

            var forwardedFor = request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                var firstIp = forwardedFor.Split(',')[0].Trim();
                if (!string.IsNullOrWhiteSpace(firstIp))
                {
                    return NormalizeLocalIp(firstIp);
                }
            }

            var realIp = request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return NormalizeLocalIp(realIp.Trim());
            }

            return NormalizeLocalIp(httpContext.Connection.RemoteIpAddress?.ToString());
        }
    }

    private static string NormalizeLocalIp(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return string.Empty;
        }

        return ipAddress == "::1" ? "127.0.0.1" : ipAddress.Trim();
    }
}
