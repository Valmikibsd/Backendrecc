using Microsoft.Extensions.Configuration;
using Payment.Application.Interfaces;
using Payment.Domain.Payment;

namespace Payment.Infrastructure.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IAdvertisementPayServiceRepository _advertisementPayServiceRepository;
        private readonly IPaymentRequestLogRepository _paymentRequestLogRepository;
        private readonly IPaymentCallbackRepository _paymentCallbackRepository;
        private readonly IPaymentApplicationStatusRepository _paymentApplicationStatusRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(
            IPaymentClient paymentClient,
            IAdvertisementPayServiceRepository advertisementPayServiceRepository,
            IPaymentRequestLogRepository paymentRequestLogRepository,
            IPaymentCallbackRepository paymentCallbackRepository,
            IPaymentApplicationStatusRepository paymentApplicationStatusRepository,
            IConfiguration configuration)
        {
            _paymentClient = paymentClient;
            _advertisementPayServiceRepository = advertisementPayServiceRepository;
            _paymentRequestLogRepository = paymentRequestLogRepository;
            _paymentCallbackRepository = paymentCallbackRepository;
            _paymentApplicationStatusRepository = paymentApplicationStatusRepository;
            _configuration = configuration;
        }

        public async Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request)
        {
            int departmentId = ParseInt(request.Ud2);
            int advertisementId = ParseInt(request.Ud3);
            int postId = ParseInt(request.Ud4);

            string? tableServiceId = await _advertisementPayServiceRepository.GetPayServiceIdAsync(
                departmentId,
                advertisementId,
                postId);

            if (!string.IsNullOrWhiteSpace(tableServiceId))
            {
                request.ServiceID = tableServiceId;
            }
            else if (string.IsNullOrWhiteSpace(request.ServiceID))
            {
                request.ServiceID = _configuration["PaymentGateway:ServiceId"];
            }

            if (string.IsNullOrWhiteSpace(request.ServiceID))
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = "Pay Service Id is not configured for this post."
                };
            }

            string applicationNo = (request.ApplicationNo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(applicationNo))
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = "Application number is required."
                };
            }

            var existingCallback = await _paymentCallbackRepository.GetPaymentCallbackAsync(applicationNo);
            bool isApplicationPaid = await _paymentApplicationStatusRepository.IsApplicationPaidAsync(
                applicationNo,
                departmentId,
                advertisementId,
                postId);

            if (isApplicationPaid ||
                (existingCallback != null &&
                 (existingCallback.Success ||
                  IsPaidStatus(existingCallback.CallbackStatus) ||
                  IsPaidUpdateStatus(existingCallback.PayStatusResult))))
            {
                return new PaymentResponse
                {
                    Success = false,
                    AppTranId = applicationNo,
                    Message = "Payment is already completed for this application."
                };
            }

            var result = await _paymentClient.InitiatePaymentAsync(request, applicationNo);

            return result;
        }

        private static bool IsUnauthorizedGatewayResponse(PaymentResponse result)
        {
            string text = ((result.PaymentUrl ?? string.Empty) + " " + (result.RawResponse ?? string.Empty))
                .ToUpperInvariant();

            return text.Contains("VALIDATEREQUEST") ||
                   text.Contains("UNAUTHORIZED") ||
                   text.Contains("FAIL.ASPX");
        }

        private static bool IsPaidStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            string value = status.Trim().ToUpperInvariant();
            return value == "SUCCESS" || value == "SUCCESSFUL" || value == "AUTH" || value == "APPROVED";
        }

        private static bool IsPaidUpdateStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            string value = status.Trim().ToUpperInvariant();
            return value == "PAYSTATUSISSET" || value == "PAYSTATUSSET" || value == "PAYSTATUSALREADYSET";
        }

        private static int ParseInt(string? value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return 0;
        }
    }
}
