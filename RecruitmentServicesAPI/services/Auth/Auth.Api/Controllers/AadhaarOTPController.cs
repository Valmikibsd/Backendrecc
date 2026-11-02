using Auth.Application.Interfaces;
using Shared.Kernel.Interfaces;
using ClientsideEncryption;
using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentPortal.Core.Aadhaar;
using Shared.Kernel;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/aadhaar")]
public class AadhaarOTPController : BaseController
{
    private readonly IUIDService _uidService;
    private readonly ICaptchaService _captchaService;

    public AadhaarOTPController(IUIDService uidService, ICaptchaService captchaService)
    {
        _uidService = uidService;
        _captchaService = captchaService;
    }

    [HttpPost("check-aadhaar")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckAadhaar([FromBody] DtoAadhaar model)
    {
        var response = await _uidService.CheckAadhaarExists(model).ConfigureAwait(false);
        return response.Status
            ? Ok(response)
            : Conflict(response);
    }

    [HttpPost("send-aadhaar-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendAadhaarOTP([FromBody] DtoAadhaar model)
    {
        if (string.IsNullOrWhiteSpace(model?.AadhaarNumber))
        {
            return BadRequest(new Response { Status = false, Message = "Aadhaar Number is required." });
        }

        if (!_captchaService.ValidateToken(model.CaptchaToken))
        {
            return BadRequest(new Response { Status = false, Message = "CAPTCHA verification expired. A new CAPTCHA is shown. Please enter it." });
        }

        var otpResponse = await _uidService.SendOtpAsync(model.AadhaarNumber);
        if (string.IsNullOrWhiteSpace(otpResponse.OTPTXN))
        {
            return Conflict(new Response { Status = false, Message = "Invalid Aadhaar number or UIDAI service is temporarily unavailable. Please verify and try again." });
        }

        return Ok(new Response<AadharOTPResponse>
        {
            Status = true,
            Message = "OTP sent successfully.",
            Data = otpResponse
        });
    }

    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyAadhaarOTP([FromBody] DtoAadhaar model)
    {
        if (string.IsNullOrWhiteSpace(model?.AadhaarNumber)
            || string.IsNullOrWhiteSpace(model.OTP)
            || string.IsNullOrWhiteSpace(model.OTPTxn))
        {
            return BadRequest(new Response { Status = false, Message = "Aadhaar Number and OTP details are required." });
        }

        var aadhaarResponse = await _uidService.VerifyOTPAsync(model.AadhaarNumber, model.OTP, model.OTPTxn);
        if (aadhaarResponse == null || string.IsNullOrEmpty(aadhaarResponse.ReferenceNo) || string.IsNullOrEmpty(aadhaarResponse.AadharNumber))
        {
            return Conflict(new Response { Status = false, Message = "Aadhaar verification failed." });
        }

        return Ok(new Response<UserAadhaarDetails>
        {
            Status = true,
            Message = "Aadhaar verified successfully.",
            Data = new UserAadhaarDetails
            {
                AadharNumber = AESEncrytDecry.EncryptStringAES(aadhaarResponse.AadharNumber)
            }
        });
    }

    [HttpPost("get-aadhaar-details")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAadhaarDetails([FromBody] DtoAadhaar model)
    {
        var response = await _uidService.GetAadhaarDetailsAsync(model).ConfigureAwait(false);
        return response.Status
            ? Ok(response)
            : Conflict(response);
    }
}
