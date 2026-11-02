using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Payment.Application.Interfaces;
using Payment.Domain.Payment;
using Shared.Auth;

namespace Payment.Api.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentCallbackService _callbackService;
        private readonly IConfiguration _configuration;

        public PaymentController(
            IPaymentService paymentService,
            IPaymentCallbackService callbackService,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _callbackService = callbackService;
            _configuration = configuration;
        }

        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ServiceID))
            {
                request.ServiceID = _configuration["PaymentGateway:ServiceId"];
            }

            var result = await _paymentService.InitiatePaymentAsync(request);

            if (!result.Success || string.IsNullOrWhiteSpace(result.PaymentUrl))
            {
                return Conflict(result);
            }

            return Ok(result);
        }

        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        [HttpPost("sync-status")]
        public async Task<IActionResult> SyncPaymentStatus([FromBody] PaymentSyncRequest request)
        {
            var result = await _callbackService.SyncApplicationPaymentAsync(request);
            return result.Success ? Ok(result) : Conflict(result);
        }

        [AllowAnonymous]
        [HttpPost("callback")]
        [HttpGet("callback")]
        public async Task<IActionResult> PaymentCallback(string? msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = Request.HasFormContentType
                    ? Request.Form["msg"].ToString()
                    : Request.Query["msg"].ToString();
            }

            if (string.IsNullOrWhiteSpace(msg))
            {
                return RedirectToPortal("missing");
            }

            var result = await _callbackService.ProcessCallbackAsync(msg);
            string payFlag = result.Success ? "ok" : "fail";
            return RedirectToPortal(payFlag, result);
        }

        private IActionResult RedirectToPortal(string payFlag, PaymentCallbackResponse? result = null)
        {
            string? portalReturnUrl = _configuration["PaymentGateway:PortalReturnUrl"];
            if (string.IsNullOrWhiteSpace(portalReturnUrl))
            {
                return Ok(new
                {
                    success = payFlag == "ok",
                    message = "Payment callback processed."
                });
            }

            string separator = portalReturnUrl.Contains("?") ? "&" : "?";
            string url = $"{portalReturnUrl}{separator}pay={Uri.EscapeDataString(payFlag)}";

            if (result != null)
            {
                if (!string.IsNullOrWhiteSpace(result.AppTranId))
                {
                    url += "&appNo=" + Uri.EscapeDataString(result.AppTranId);
                }

                if (!string.IsNullOrWhiteSpace(result.Ud2))
                {
                    url += "&dept=" + Uri.EscapeDataString(result.Ud2);
                }

                if (!string.IsNullOrWhiteSpace(result.Ud3))
                {
                    url += "&adv=" + Uri.EscapeDataString(result.Ud3);
                }

                if (!string.IsNullOrWhiteSpace(result.Ud4))
                {
                    url += "&post=" + Uri.EscapeDataString(result.Ud4);
                }
            }

            return Redirect(url);
        }
    }
}
