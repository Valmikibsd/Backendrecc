using RecruitmentPortal.Core.Common;

namespace Shared.Kernel.Interfaces;

public interface ICaptchaService
{
    CaptchaGenerateResponse Generate();
    CaptchaValidationResult Validate(string captchaId, string captchaText);
    bool ValidateToken(string? captchaToken);
}
