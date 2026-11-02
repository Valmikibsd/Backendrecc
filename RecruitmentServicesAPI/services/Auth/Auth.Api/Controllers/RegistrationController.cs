using Auth.Application.Interfaces;
using Shared.Kernel.Interfaces;
using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Registration;
using Shared.Kernel;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/registration")]
public class RegistrationController(IRegistrationRepository repo, ICaptchaService captchaService) : BaseController
{
    private readonly IRegistrationRepository _repo = repo;
    private readonly ICaptchaService _captchaService = captchaService;

    [HttpPost("saveregistration")]
    [AllowAnonymous]
    public async Task<IActionResult> SaveRegistration([FromBody] DtoRegistration dtoRegistration)
    {
        var response = await _repo.SaveRegistration(dtoRegistration).ConfigureAwait(false);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpPost("sendOTP")]
    [AllowAnonymous]
    public async Task<IActionResult> SendEmailAndMobileOtp([FromBody] DtoEmailAndMobileOTP dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Type))
        {
            return BadRequest(new Response { Status = false, Message = "OTP type is required." });
        }

        if (dto.Type.Equals("mobile", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(dto.Mobile))
        {
            return BadRequest(new Response { Status = false, Message = "Mobile Number is required." });
        }

        if (dto.Type.Equals("email", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest(new Response { Status = false, Message = "Email ID is required." });
        }

        if (!_captchaService.ValidateToken(dto.CaptchaToken))
        {
            return BadRequest(new Response
            {
                Status = false,
                Message = "CAPTCHA verification expired. A new CAPTCHA is shown. Please enter it."
            });
        }

        var response = await _repo.SendEmailAndMobileOtp(dto).ConfigureAwait(false);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpPost("verifyOTP")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmailAndMobileOtp([FromBody] DtoEmailAndMobileOTP dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Type) || string.IsNullOrWhiteSpace(dto.OTP))
        {
            return BadRequest(new Response { Status = false, Message = "OTP details are required." });
        }

        var response = await _repo.VerifyEmailAndMobileOtp(dto).ConfigureAwait(false);
        return response.Status ? Ok(response) : Conflict(response);
    }
}
