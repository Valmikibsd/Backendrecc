using Microsoft.Extensions.Configuration;
using Payment.Application.Interfaces;
using Payment.Domain.Payment;
using System.Globalization;

namespace Payment.Infrastructure.Payment
{
    public class PaymentClient : IPaymentClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IPaymentChecksumService _checksumService;
        private readonly IPaymentRequestLogRepository _paymentRequestLogRepository;

        public PaymentClient(
            HttpClient httpClient,
            IConfiguration configuration,
            IPaymentChecksumService checksumService,
            IPaymentRequestLogRepository paymentRequestLogRepository)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _checksumService = checksumService;
            _paymentRequestLogRepository = paymentRequestLogRepository;
        }

        public async Task<PaymentResponse> InitiatePaymentAsync(
            PaymentRequest request,
            string transactionId)
        {
            string? baseUrl =
                _configuration["PaymentGateway:BaseUrl"];

            string? clientId =
                _configuration["PaymentGateway:UserId"];

            string? clientPassword =
                _configuration["PaymentGateway:Password"];

            string? gatewayServiceId = string.IsNullOrWhiteSpace(request.ServiceID)
                ? _configuration["PaymentGateway:ServiceId"]
                : request.ServiceID.Trim();

            string? returnUrl =
                string.IsNullOrWhiteSpace(request.ReturnUrl)
                    ? _configuration["PaymentGateway:ReturnUrl"]
                    : request.ReturnUrl.Trim();

            if (string.IsNullOrWhiteSpace(request.Ud5))
            {
                request.Ud5 = "0";
            }

            if (string.IsNullOrWhiteSpace(request.Ud6))
            {
                request.Ud6 = _configuration["PaymentGateway:Ud6"];
            }

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Payment gateway BaseUrl is missing.");

            if (string.IsNullOrWhiteSpace(clientId))
                throw new InvalidOperationException("Payment gateway ClientID is missing.");

            if (string.IsNullOrWhiteSpace(clientPassword))
                throw new InvalidOperationException("Payment gateway ClientPW is missing.");

            if (string.IsNullOrWhiteSpace(gatewayServiceId))
                throw new InvalidOperationException("Payment gateway ServiceId is missing.");

            if (string.IsNullOrWhiteSpace(returnUrl))
                throw new InvalidOperationException("Payment gateway ReturnUrl is missing.");

            if (string.IsNullOrWhiteSpace(request.Ud6))
                throw new InvalidOperationException("Payment gateway Ud6 is missing.");

            // -------------------------------------------------------
            // 1. Gateway URL
            // -------------------------------------------------------

            string paymentUrl =
                $"{baseUrl.TrimEnd('/')}/PGWAY/InitSecureHTTPRequest.aspx";

            // -------------------------------------------------------
            // 2. Timestamp
            // -------------------------------------------------------

            string timestamp =
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssffff",
                    CultureInfo.InvariantCulture);

            // -------------------------------------------------------
            // 3. Generate Token
            // -------------------------------------------------------

            string token =
                _checksumService.GenerateToken(
                    clientPassword,
                    timestamp);

            // -------------------------------------------------------
            // 4. Amount
            // -------------------------------------------------------

            string amount =
                request.Amount.ToString(
                    "0.00",
                    CultureInfo.InvariantCulture);

            // -------------------------------------------------------
            // 5. Receipt
            // -------------------------------------------------------

            string receipt =
                request.IsReceipt
                    ? "TRUE"
                    : "FALSE";

            // -------------------------------------------------------
            // 6. Create raw checksum message
            // -------------------------------------------------------

            string rawMessage =
                gatewayServiceId + "|" +
                clientId + "|" +
                token + "|" +
                transactionId + "|" +
                amount + "|" +
                receipt + "|" +
                (request.Email ?? "") + "|" +
                (request.Mobile ?? "") + "|" +
                (request.Name ?? "") + "|" +
                (request.Ud1 ?? "") + "|" +
                (request.Ud2 ?? "") + "|" +
                (request.Ud3 ?? "") + "|" +
                (request.Ud4 ?? "") + "|" +
                (request.Ud5 ?? "") + "|" +
                returnUrl + "|" +
                timestamp;

            // -------------------------------------------------------
            // 7. Add Ud6 to checksum calculation
            // -------------------------------------------------------

            string checksumMessage =
                rawMessage + "|" +
                (request.Ud6 ?? "");

            // -------------------------------------------------------
            // 8. Generate CRC32 checksum
            // -------------------------------------------------------

            uint checksum =
                _checksumService.GenerateChecksum(
                    checksumMessage);

            string checksumValue =
                checksum.ToString(
                    CultureInfo.InvariantCulture);

            // -------------------------------------------------------
            // 9. Gateway parameters
            // -------------------------------------------------------

            var parameters =
                new Dictionary<string, string>
                {
                    ["SID"] = gatewayServiceId,

                    ["ID"] = clientId,

                    ["AppTranID"] = transactionId,

                    ["Amt"] = amount,

                    ["IsReceipt"] = receipt,

                    ["Email"] =
                        request.Email ?? "",

                    ["Mobile"] =
                        request.Mobile ?? "",

                    ["Udf1"] =
                        request.Ud1 ?? "",

                    ["Udf2"] =
                        request.Ud2 ?? "",

                    ["Udf3"] =
                        request.Ud3 ?? "",

                    ["Udf4"] =
                        request.Ud4 ?? "",

                    ["Udf5"] =
                        request.Ud5 ?? "",

                    ["RU"] = returnUrl,

                    ["Name"] =
                        request.Name ?? "",

                    ["Timestamp"] = timestamp,

                    ["Token"] = token,

                    ["Checksum"] = checksumValue
                };

            // -------------------------------------------------------
            // 10. Create exact request string
            // -------------------------------------------------------

            string requestData =
                string.Join(
                    "&",
                    parameters.Select(
                        x =>
                            $"{x.Key}={Uri.EscapeDataString(x.Value ?? "")}"));

            // -------------------------------------------------------
            // 11. SAVE REQUEST BEFORE CALLING GATEWAY
            // -------------------------------------------------------

            var paymentLog =
                new PaymentRequestLog
                {
                    AppTranID = transactionId,
                    ServiceID = gatewayServiceId,
                    ClientID =
                        clientId,

                    Amount =
                        request.Amount,

                    IsReceipt =
                        receipt,

                    Email =
                        request.Email ?? "",

                    Mobile =
                        request.Mobile ?? "",

                    Name =
                        request.Name ?? "",

                    Udf1 =
                        request.Ud1,

                    Udf2 =
                        request.Ud2,

                    Udf3 =
                        request.Ud3,

                    Udf4 =
                        request.Ud4,

                    Udf5 =
                        request.Ud5,

                    ReturnUrl =
                        returnUrl,

                    Timestamp =
                        timestamp,

                    Token =
                        token,

                    Checksum =
                        checksumValue,

                    RequestUrl =
                        paymentUrl,

                    RequestData =
                        requestData,

                    ReqStatus =
                        "Created",

                    CreatedOn =
                        DateTime.Now
                };

            long logId =
                await _paymentRequestLogRepository
                    .SaveRequestAsync(paymentLog);

            // -------------------------------------------------------
            // 12. Call Payment Gateway
            // -------------------------------------------------------

            try
            {
                using var content =
                    new FormUrlEncodedContent(parameters);

                HttpResponseMessage response =
                    await _httpClient.PostAsync(
                        paymentUrl,
                        content);

                string responseText =
                    await response.Content
                        .ReadAsStringAsync();

                // ---------------------------------------------------
                // 13. Update Gateway Response
                // ---------------------------------------------------

                if (!response.IsSuccessStatusCode)
                {
                    await _paymentRequestLogRepository
                        .UpdateResponseAsync(
                            transactionId,
                            responseText,
                            "GatewayHttpError",
                            $"HTTP {(int)response.StatusCode}");

                    return new PaymentResponse
                    {
                        Success = false,
                        AppTranId = transactionId,
                        Message =
                            $"Payment gateway returned HTTP " +
                            $"{(int)response.StatusCode}",
                        RawResponse = responseText
                    };
                }

                var parsed = ParseGatewayResponse(
                    responseText,
                    transactionId);

                await _paymentRequestLogRepository
                    .UpdateResponseAsync(
                        transactionId,
                        responseText,
                        parsed.Success
                            ? "GatewayOk"
                            : "GatewayFailed",
                        parsed.Success ? null : parsed.Message);

                return parsed;
            }
            catch (Exception ex)
            {
                // ---------------------------------------------------
                // 16. Save exception
                // ---------------------------------------------------

                await _paymentRequestLogRepository
                    .UpdateResponseAsync(
                        transactionId,
                        "",
                        "GatewayException",
                        ex.Message);

                throw;
            }
        }

        private PaymentResponse ParseGatewayResponse(
            string responseText,
            string transactionId)
        {
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return new PaymentResponse
                {
                    Success = false,

                    AppTranId =
                        transactionId,

                    Message =
                        "Empty response from payment gateway."
                };
            }

            responseText =
                responseText.Trim();

            // Gateway returned an error
            if (responseText.Contains(
                    "!ERROR!",
                    StringComparison.OrdinalIgnoreCase))
            {
                return new PaymentResponse
                {
                    Success = false,

                    AppTranId =
                        transactionId,

                    Message =
                        responseText,

                    RawResponse =
                        responseText
                };
            }

            // Expected response:
            //
            // PAYID:http://payment-url
            //

            int httpIndex =
                responseText.IndexOf(
                    ":http",
                    StringComparison.OrdinalIgnoreCase);

            if (httpIndex <= 0)
            {
                return new PaymentResponse
                {
                    Success = false,

                    AppTranId =
                        transactionId,

                    Message =
                        "Invalid payment gateway response.",

                    RawResponse =
                        responseText
                };
            }

            string payId =
                responseText
                    .Substring(
                        0,
                        httpIndex)
                    .Trim();

            string paymentUrl =
                responseText
                    .Substring(
                        httpIndex + 1)
                    .Trim();

            if (string.IsNullOrWhiteSpace(payId) ||
                string.IsNullOrWhiteSpace(paymentUrl))
            {
                return new PaymentResponse
                {
                    Success = false,

                    AppTranId =
                        transactionId,

                    Message =
                        "Invalid PAYID response.",

                    RawResponse =
                        responseText
                };
            }

            string redirectUrl =
                $"{paymentUrl}" +
                $"?payreq={Uri.EscapeDataString(payId)}";

            return new PaymentResponse
            {
                Success = true,

                AppTranId =
                    transactionId,

                PayId =
                    payId,

                PaymentUrl =
                    redirectUrl,

                Message =
                    "Payment request created successfully.",

                RawResponse =
                    responseText
            };
        }
    }
}