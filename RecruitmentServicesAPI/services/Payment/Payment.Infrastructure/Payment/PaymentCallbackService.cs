using Payment.Application.Interfaces;
using Payment.Domain.Payment;
using System.Globalization;
using System.Linq;

namespace Payment.Infrastructure.Payment
{
    public class PaymentCallbackService : IPaymentCallbackService
    {
        private readonly IPaymentCallbackRepository _callbackRepository;
        private readonly IPaymentRequestLogRepository _requestLogRepository;
        private readonly IPaymentChecksumService _checksumService;
        private readonly IPaymentApplicationStatusRepository _applicationStatusRepository;

        public PaymentCallbackService(
            IPaymentCallbackRepository callbackRepository,
            IPaymentRequestLogRepository requestLogRepository,
            IPaymentChecksumService checksumService,
            IPaymentApplicationStatusRepository applicationStatusRepository)
        {
            _callbackRepository = callbackRepository;
            _requestLogRepository = requestLogRepository;
            _checksumService = checksumService;
            _applicationStatusRepository = applicationStatusRepository;
        }

        public async Task<PaymentCallbackResponse> ProcessCallbackAsync(string msg)
        {
            var response = new PaymentCallbackResponse();

            if (string.IsNullOrWhiteSpace(msg))
            {
                response.Success = false;
                response.Message = "Payment gateway response is empty.";
                response.CallbackResult = "EmptyMsg";
                return response;
            }

            string[] values = msg.Split('|');

            if (values.Length < 17)
            {
                response.Success = false;
                response.Message =
                    $"Invalid gateway response. Expected at least 17 fields but received {values.Length}.";
                response.CallbackResult = "InvalidMsg";
                return response;
            }

            MapGatewayFields(response, values);
            ValidateChecksum(response, values, msg);

            var requestLog = string.IsNullOrWhiteSpace(response.AppTranId)
                ? null
                : await _requestLogRepository.GetPaymentRequestAsync(response.AppTranId);

            if (requestLog == null)
            {
                response.Success = false;
                response.CallbackResult = "InitiateNotFound";
                response.PayStatusResult = "PayStatusIsNotSet";
                response.Message =
                    "No initiate log found for this AppTranId. Check PaymentRequestLog.";
                await SaveCallback(response, msg);
                return response;
            }

            if (response.ChecksumValid == false)
            {
                response.Success = false;
                response.CallbackResult = "ChecksumFail";
                response.PayStatusResult = "PayStatusIsNotSet";
                response.Message =
                    "Callback checksum did not match. Application was not marked paid.";
                await SaveCallback(response, msg);
                return response;
            }

            if (!IsSuccessfulStatus(response.CallbackStatus))
            {
                response.Success = false;
                response.CallbackResult = GetFailedCallbackResult(response.CallbackStatus);
                response.PayStatusResult = "PayStatusIsNotSet";
                response.Message = GetStatusMessage(response.CallbackStatus, response.CallbackErrMsg);
                await SaveCallback(response, msg);
                return response;
            }

            if (response.Amount.HasValue &&
                requestLog.Amount != response.Amount.Value)
            {
                response.Success = false;
                response.CallbackResult = "AmountMismatch";
                response.PayStatusResult = "PayStatusIsNotSet";
                response.Message =
                    $"Paid amount {response.Amount} does not match initiated amount {requestLog.Amount}.";
                await SaveCallback(response, msg);
                return response;
            }

            response.Success = true;
            response.Message = GetStatusMessage(response.CallbackStatus, response.CallbackErrMsg);

            if (await _callbackRepository.HasApplicationUpdatedAsync(response.AppTranId ?? string.Empty))
            {
                response.CallbackResult = "Duplicate";
                response.PayStatusResult = "PayStatusAlreadySet";
                response.Message =
                    "Payment was already marked on the application. Duplicate callback ignored.";
                await SaveCallback(response, msg);
                return response;
            }

            await UpdateApplicationFromCallback(response, requestLog);
            await SaveCallback(response, msg);
            return response;
        }

        public async Task<PaymentCallbackResponse> SyncApplicationPaymentAsync(PaymentSyncRequest request)
        {
            var response = new PaymentCallbackResponse
            {
                Success = false,
                CallbackResult = "SyncStarted"
            };

            PaymentRequestLog? requestLog = null;

            if (!string.IsNullOrWhiteSpace(request.ApplicationNo))
            {
                requestLog = await _requestLogRepository.GetPaymentRequestAsync(request.ApplicationNo.Trim());
            }

            if (requestLog == null)
            {
                response.CallbackResult = "InitiateNotFound";
                response.Message =
                    "No payment initiate record found. Check PaymentRequestLog. Applicant may not have reached the gateway.";
                return response;
            }

            response.AppTranId = requestLog.AppTranID;
            response.Amount = requestLog.Amount;
            response.Ud1 = requestLog.Udf1;
            response.Ud2 = requestLog.Udf2;
            response.Ud3 = requestLog.Udf3;
            response.Ud4 = requestLog.Udf4;
            response.Ud5 = requestLog.Udf5;

            var callback = await _callbackRepository.GetPaymentCallbackAsync(requestLog.AppTranID);

            if (callback == null)
            {
                response.CallbackResult = "NoCallback";
                response.PayStatusResult = "PayStatusIsNotSet";
                response.Message =
                    "Amount may have been cut at bank, but PaymentResponseLog has no callback for this AppTranId. Ask gateway/support with AppTranId.";
                return response;
            }

            response.CallbackStatus = callback.CallbackStatus;
            response.TransactionRefNo = callback.TransactionRefNo;
            response.BankTranRefNo = callback.BankTranRefNo;
            response.ReceiptNo = callback.ReceiptNo;
            response.CallbackErrMsg = callback.CallbackErrMsg;
            response.CallbackId = callback.CallbackId;

            if (!IsSuccessfulStatus(callback.CallbackStatus) && callback.Success != true)
            {
                response.CallbackResult = "NotPaid";
                response.Message = GetStatusMessage(callback.CallbackStatus, callback.CallbackErrMsg);
                return response;
            }

            if (string.Equals(callback.PayStatusResult, "PayStatusIsSet", StringComparison.OrdinalIgnoreCase)
                || string.Equals(callback.PayStatusResult, "PayStatusSet", StringComparison.OrdinalIgnoreCase)
                || string.Equals(callback.PayStatusResult, "PayStatusAlreadySet", StringComparison.OrdinalIgnoreCase)
                || string.Equals(callback.PayStatusResult, "Application payment status updated.", StringComparison.OrdinalIgnoreCase))
            {
                response.Success = true;
                response.CallbackResult = "AlreadyPaid";
                response.PayStatusResult = "PayStatusAlreadySet";
                response.Message = "Application is already marked paid.";
                return response;
            }

            string applicationNo = FirstNonEmpty(
                request.ApplicationNo,
                requestLog.AppTranID);
            int departmentId = request.DepartmentId > 0
                ? request.DepartmentId
                : ParseInt(requestLog.Udf2);
            int advertisementId = request.AdvertisementId > 0
                ? request.AdvertisementId
                : ParseInt(requestLog.Udf3);
            int postId = request.PostId > 0
                ? request.PostId
                : ParseInt(requestLog.Udf4);

            var update = await _applicationStatusRepository.UpdatePayStatusAsync(
                applicationNo,
                departmentId,
                advertisementId,
                postId,
                1,
                DateTime.Now);

            response.Success = update.Status;
            response.PayStatusResult = update.Status
                ? "PayStatusIsSet"
                : "PayStatusIsSetFailed";
            response.CallbackResult = update.Status ? "Paid" : "PayStatusFail";
            response.Message = update.Status
                ? "Payment was received earlier. Application is now marked paid."
                : "Callback exists but application update failed. See PaymentResponseLog and run this sync after fixing table mapping.";

            callback.PayStatusResult = response.PayStatusResult;
            callback.CallbackResult = "SyncRetry";
            await _callbackRepository.SaveCallbackAsync(callback, "SYNC_RETRY");

            return response;
        }

        private async Task UpdateApplicationFromCallback(
            PaymentCallbackResponse response,
            PaymentRequestLog requestLog)
        {
            string applicationNo = FirstNonEmpty(
                response.AppTranId,
                requestLog.AppTranID);
            int departmentId = ParseInt(response.Ud2) > 0 ? ParseInt(response.Ud2) : ParseInt(requestLog.Udf2);
            int advertisementId = ParseInt(response.Ud3) > 0 ? ParseInt(response.Ud3) : ParseInt(requestLog.Udf3);
            int postId = ParseInt(response.Ud4) > 0 ? ParseInt(response.Ud4) : ParseInt(requestLog.Udf4);

            if (string.IsNullOrWhiteSpace(applicationNo) ||
                departmentId <= 0 ||
                advertisementId <= 0 ||
                postId <= 0)
            {
                response.CallbackResult = "KeysMissing";
                response.PayStatusResult = "PayStatusIsNotSet";
                response.Message =
                    "Callback success but application number or Ud2-Ud4 are missing. Amount may be cut.";
                return;
            }

            var update = await _applicationStatusRepository.UpdatePayStatusAsync(
                applicationNo,
                departmentId,
                advertisementId,
                postId,
                1,
                DateTime.Now);

            if (update.Status)
            {
                response.CallbackResult = "Paid";
                response.PayStatusResult = "PayStatusIsSet";
                response.Message = update.Message;
                return;
            }

            response.CallbackResult = "PayStatusFail";
            response.PayStatusResult = "PayStatusIsSetFailed";
            response.Message = update.Message;
        }

        private async Task SaveCallback(PaymentCallbackResponse response, string msg)
        {
            response.CallbackId = await _callbackRepository.SaveCallbackAsync(response, msg);
        }

        private void ValidateChecksum(
            PaymentCallbackResponse response,
            string[] values,
            string msg)
        {
            if (values.Length < 18)
            {
                response.ChecksumValid = null;
                return;
            }

            string checksumValue = values[values.Length - 1]?.Trim() ?? string.Empty;
            string payload = string.Join("|", values.Take(values.Length - 1));
            uint expected = _checksumService.GenerateChecksum(payload);

            bool isValid =
                string.Equals(
                    expected.ToString(CultureInfo.InvariantCulture),
                    checksumValue,
                    StringComparison.OrdinalIgnoreCase);

            if (!isValid && uint.TryParse(checksumValue, out uint provided))
            {
                isValid = provided == expected;
            }

            response.ChecksumValid = isValid;

            if (!isValid)
            {
                response.CallbackErrMsg =
                    (response.CallbackErrMsg ?? string.Empty) +
                    " Checksum mismatch on callback msg.";
            }
        }

        private static void MapGatewayFields(PaymentCallbackResponse response, string[] values)
        {
            response.CallbackStatus = values[0]?.Trim();
            response.AppTranId = values[1]?.Trim();
            response.TransactionRefNo = values[2]?.Trim();
            response.BankTranRefNo = values[3]?.Trim();
            response.BankId = values[4]?.Trim();
            response.Amount = ParseDecimal(values[5]);
            response.Ud1 = values[6]?.Trim();
            response.Ud2 = values[7]?.Trim();
            response.Ud3 = values[8]?.Trim();
            response.Ud4 = values[9]?.Trim();
            response.Ud5 = values[10]?.Trim();
            response.TransactionNo = values[11]?.Trim();
            response.PayeeName = values[12]?.Trim();
            response.TotalAmount = ParseDecimal(values[13]);
            response.PortalFee = ParseDecimal(values[14]);
            response.CallbackErrMsg = values[15]?.Trim();
            response.ReceiptNo = values[16]?.Trim();
        }

        private static decimal? ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (decimal.TryParse(
                value.Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }

            return null;
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            foreach (string? value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }

            return string.Empty;
        }

        private static int ParseInt(string? value)
        {
            if (int.TryParse(value, out int result))
                return result;
            return 0;
        }

        private static bool IsSuccessfulStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return status.Trim().ToUpperInvariant() switch
            {
                "SUCCESS" => true,
                "SUCCESSFUL" => true,
                "AUTH" => true,
                "APPROVED" => true,
                _ => false
            };
        }

        private static string GetFailedCallbackResult(string? status)
        {
            string value = status?.Trim().ToUpperInvariant() ?? "";
            return value switch
            {
                "FAILED" => "Failed",
                "CANCELLED" => "Cancelled",
                "CANCELED" => "Cancelled",
                _ => "NotPaid"
            };
        }

        private static string GetStatusMessage(string? status, string? errorMessage)
        {
            string value = status?.Trim().ToUpperInvariant() ?? "";

            return value switch
            {
                "SUCCESS" => "Payment successful.",
                "SUCCESSFUL" => "Payment successful.",
                "AUTH" => "Payment authorized successfully.",
                "APPROVED" => "Payment approved.",
                "FAILED" => "Payment failed.",
                "CANCELLED" => "Payment cancelled by user.",
                "CANCELED" => "Payment cancelled by user.",
                _ => string.IsNullOrWhiteSpace(errorMessage) ? "Payment status received." : errorMessage
            };
        }
    }
}
