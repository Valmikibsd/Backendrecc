using Shared.Kernel.Interfaces;
using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Common;
using Shared.Kernel;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/captcha")]
public class CaptchaController(ICaptchaService captchaService) : BaseController
{
    private readonly ICaptchaService _captchaService = captchaService;

    [HttpGet("generate")]
    [AllowAnonymous]
    public IActionResult Generate()
    {
        var data = _captchaService.Generate();
        return Ok(new Response<CaptchaGenerateResponse>
        {
            Status = true,
            Message = "Captcha generated successfully.",
            Data = data
        });
    }

    [HttpPost("validate")]
    [AllowAnonymous]
    public IActionResult Validate([FromBody] CaptchaValidateRequest request)
    {
        var result = _captchaService.Validate(
            request?.CaptchaId ?? string.Empty,
            request?.CaptchaText ?? string.Empty);

        if (!result.Success)
        {
            return Ok(new Response<object>
            {
                Status = false,
                Message = result.Message,
                Data = new { errorCode = result.ErrorCode }
            });
        }

        return Ok(new Response<CaptchaValidateResponse>
        {
            Status = true,
            Message = "Captcha verified successfully.",
            Data = new CaptchaValidateResponse { CaptchaToken = result.CaptchaToken }
        });
    }
}
