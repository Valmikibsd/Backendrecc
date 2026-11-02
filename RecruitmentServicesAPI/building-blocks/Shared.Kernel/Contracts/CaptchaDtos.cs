namespace RecruitmentPortal.Core.Common;

public class CaptchaGenerateResponse
{
    public string CaptchaId { get; set; } = string.Empty;
    public string CaptchaCode { get; set; } = string.Empty;
}

public class CaptchaValidateRequest
{
    public string CaptchaId { get; set; } = string.Empty;
    public string CaptchaText { get; set; } = string.Empty;
}

public class CaptchaValidateResponse
{
    public string CaptchaToken { get; set; } = string.Empty;
}

public static class CaptchaErrorCodes
{
    public const string Required = "REQUIRED";
    public const string Expired = "EXPIRED";
    public const string Invalid = "INVALID";
}

public class CaptchaValidationResult
{
    public bool Success { get; set; }
    public string CaptchaToken { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public static CaptchaValidationResult Ok(string captchaToken) => new()
    {
        Success = true,
        CaptchaToken = captchaToken
    };

    public static CaptchaValidationResult Fail(string errorCode, string message) => new()
    {
        Success = false,
        ErrorCode = errorCode,
        Message = message
    };
}
