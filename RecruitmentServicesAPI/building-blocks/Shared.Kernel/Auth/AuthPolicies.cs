namespace Shared.Auth;

public static class AuthClaims
{
    public const string RoleName = "role_name";
    public const string RoutePrefix = "route_prefix";
}

public static class RolePrefixes
{
    public const string SuperAdmin = "super-admin";
    public const string Admin = "admin";
    public const string DeptUser = "dept-user";
    public const string Applicant = "applicant";
    public const string Support = "support";
    public const string Accounts = "accounts";
}

public static class AuthPolicies
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string AdminOnly = "AdminOnly";
    public const string DeptUserOnly = "DeptUserOnly";
    public const string ApplicantOnly = "ApplicantOnly";
    public const string SupportOnly = "SupportOnly";
    public const string AccountsOnly = "AccountsOnly";
    public const string ReportAccess = "ReportAccess";
    public const string ViewApplicationAccess = "ViewApplicationAccess";
}
