using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Shared.Kernel.Encryption
{
    /// <summary>
    /// Middleware that decrypts encrypted HTTP requests and encrypts JSON HTTP responses.
    /// Client opts in via header <c>X-Request-Response-Encrypted: 1</c> (or by sending ciphertext).
    /// Runs at the gateway so downstream microservices receive and return plain JSON.
    /// </summary>
    public class RequestResponseEncryptionMiddleware
    {
        public const string EncryptionHeaderName = "X-Request-Response-Encrypted";
        /// <summary>Legacy header from earlier builds — still accepted.</summary>
        public const string LegacyEncryptionHeaderName = "X-Payload-Encrypted";
        public const string EncryptedQueryParam = "encrypted";

        private readonly RequestDelegate _next;
        private readonly RequestResponseEncryptionSettings _settings;
        private readonly ILogger<RequestResponseEncryptionMiddleware> _logger;

        public RequestResponseEncryptionMiddleware(
            RequestDelegate next,
            IOptions<RequestResponseEncryptionSettings> options,
            ILogger<RequestResponseEncryptionMiddleware> logger)
        {
            _next = next;
            _settings = options?.Value ?? new RequestResponseEncryptionSettings();
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, RequestResponseCryptoHelper crypto)
        {
            if (!_settings.Enabled || ShouldBypass(context))
            {
                await _next(context);
                return;
            }

            bool clientWantsEncryption = HasEncryptionHeader(context);
            bool decryptedRequest;

            try
            {
                decryptedRequest = await TryDecryptRequestAsync(context, crypto).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Request payload decryption failed for {Path}", context.Request.Path);
                await WritePlainErrorAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    "Invalid encrypted request. Ensure API and Angular requestResponseEncryptionKey values match.")
                    .ConfigureAwait(false);
                return;
            }

            clientWantsEncryption = clientWantsEncryption || decryptedRequest;

            if (!clientWantsEncryption)
            {
                // Swagger / plain clients: leave request and response unencrypted.
                await _next(context);
                return;
            }

            var originalBody = context.Response.Body;
            await using var responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            try
            {
                await _next(context);

                responseBuffer.Seek(0, SeekOrigin.Begin);
                string responseText = await new StreamReader(responseBuffer, Encoding.UTF8).ReadToEndAsync()
                    .ConfigureAwait(false);

                if (ShouldEncryptResponse(context, responseText))
                {
                    string encrypted = crypto.Encrypt(responseText);
                    byte[] bytes = Encoding.UTF8.GetBytes(encrypted);
                    context.Response.Headers.Remove("Content-Length");
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    context.Response.ContentLength = bytes.Length;
                    context.Response.Headers.Remove("Content-Encoding");
                    await originalBody.WriteAsync(bytes).ConfigureAwait(false);
                }
                else
                {
                    responseBuffer.Seek(0, SeekOrigin.Begin);
                    await responseBuffer.CopyToAsync(originalBody).ConfigureAwait(false);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }

        private static bool ShouldBypass(HttpContext context)
        {
            if (HttpMethods.IsOptions(context.Request.Method))
            {
                return true;
            }

            PathString path = context.Request.Path;
            if (path.StartsWithSegments("/swagger")
                || path.StartsWithSegments("/favicon.ico")
                || path.StartsWithSegments("/assets")
                || path.StartsWithSegments("/_framework")
                || path.StartsWithSegments("/health"))
            {
                return true;
            }

            string? pathValue = path.Value;
            if (!string.IsNullOrEmpty(pathValue)
                && pathValue.Contains("webhook", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static bool HasEncryptionHeader(HttpContext context)
        {
            return IsTruthyHeader(context, EncryptionHeaderName)
                || IsTruthyHeader(context, LegacyEncryptionHeaderName);
        }

        private static bool IsTruthyHeader(HttpContext context, string headerName)
        {
            return context.Request.Headers.TryGetValue(headerName, out StringValues values)
                && values.Any(v => string.Equals(v, "1", StringComparison.Ordinal)
                    || string.Equals(v, "true", StringComparison.OrdinalIgnoreCase));
        }

        private static async Task<bool> TryDecryptRequestAsync(
            HttpContext context,
            RequestResponseCryptoHelper crypto)
        {
            bool decryptedSomething = false;

            if (!context.Request.HasFormContentType)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);
                string body = await reader.ReadToEndAsync().ConfigureAwait(false);
                context.Request.Body.Position = 0;

                if (RequestResponseCryptoHelper.LooksEncrypted(body))
                {
                    string plainJson = crypto.Decrypt(body);
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainJson);
                    context.Request.Body = new MemoryStream(plainBytes);
                    context.Request.ContentLength = plainBytes.Length;
                    context.Request.ContentType = "application/json";
                    decryptedSomething = true;
                }
            }

            if (context.Request.Query.TryGetValue(EncryptedQueryParam, out StringValues encryptedQuery)
                && RequestResponseCryptoHelper.LooksEncrypted(encryptedQuery.ToString()))
            {
                string plainJson = crypto.Decrypt(encryptedQuery.ToString());
                ApplyDecryptedQuery(context, plainJson);
                decryptedSomething = true;
            }

            return decryptedSomething;
        }

        private static void ApplyDecryptedQuery(HttpContext context, string plainJson)
        {
            List<KeyValuePair<string, string?>> pairs;
            try
            {
                using var doc = JsonDocument.Parse(plainJson);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException("Encrypted query payload is not a JSON object.");
                }

                pairs = new List<KeyValuePair<string, string?>>();
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    string? value = prop.Value.ValueKind switch
                    {
                        JsonValueKind.Null => null,
                        JsonValueKind.Undefined => null,
                        JsonValueKind.String => prop.Value.GetString(),
                        JsonValueKind.True => "true",
                        JsonValueKind.False => "false",
                        JsonValueKind.Number => prop.Value.GetRawText(),
                        _ => prop.Value.GetRawText()
                    };
                    pairs.Add(new KeyValuePair<string, string?>(prop.Name, value));
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Encrypted query payload is not a JSON object of string values.", ex);
            }

            if (pairs.Count == 0)
            {
                context.Request.QueryString = QueryString.Empty;
                return;
            }

            context.Request.QueryString = QueryString.Create(pairs);
        }

        private static bool ShouldEncryptResponse(HttpContext context, string responseText)
        {
            if (string.IsNullOrEmpty(responseText))
            {
                return false;
            }

            if (context.Response.Headers.TryGetValue("Content-Encoding", out StringValues encoding)
                && encoding.Any(v => !string.IsNullOrWhiteSpace(v)))
            {
                return false;
            }

            string? contentType = context.Response.ContentType;
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            bool isJson = contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
                || contentType.Contains("text/plain", StringComparison.OrdinalIgnoreCase)
                || contentType.Contains("application/problem+json", StringComparison.OrdinalIgnoreCase);

            if (!isJson)
            {
                return false;
            }

            if (RequestResponseCryptoHelper.LooksEncrypted(responseText))
            {
                return false;
            }

            return true;
        }

        private static async Task WritePlainErrorAsync(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            var payload = JsonSerializer.Serialize(new { status = false, message });
            await context.Response.WriteAsync(payload).ConfigureAwait(false);
        }
    }
}
