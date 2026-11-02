using Shared.Kernel.Interfaces;
using CrudOperations;
using Microsoft.Extensions.Configuration;

namespace Shared.Kernel.Services;

public class SmsService(IConfiguration configuration) : ISmsService
{
    public async Task<Response> SendAsync(string mobileNo, string message, string templateId)
    {
        var gatewayUrl = configuration["SMS:gatewayUrl"];
        var appId = configuration["SMS:appId"];
        var userId = configuration["SMS:userId"];
        var password = configuration["SMS:password"];
        var senderName = configuration["SMS:sendername"];

        if (string.IsNullOrWhiteSpace(gatewayUrl)
            || string.IsNullOrWhiteSpace(appId)
            || string.IsNullOrWhiteSpace(userId)
            || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(senderName))
        {
            return new Response { Status = false, Message = "SMS gateway configuration is missing." };
        }

        if (string.IsNullOrWhiteSpace(mobileNo)
            || string.IsNullOrWhiteSpace(message)
            || string.IsNullOrWhiteSpace(templateId))
        {
            return new Response { Status = false, Message = "SMS message details are missing." };
        }

        var url =
            gatewayUrl +
            "?appid=" + Uri.EscapeDataString(appId) +
            "&userId=" + Uri.EscapeDataString(userId) +
            "&pass=" + Uri.EscapeDataString(password) +
            "&contenttype=1" +
            "&alert=1" +
            "&selfid=true" +
            "&from=" + Uri.EscapeDataString(senderName) +
            "&to=" + Uri.EscapeDataString(mobileNo) +
            "&text=" + Uri.EscapeDataString(message) +
            "&templateid=" + Uri.EscapeDataString(templateId);

        using var client = new HttpClient();
        var gatewayResult = await client.GetStringAsync(url).ConfigureAwait(false);

        var isSuccess = !string.IsNullOrWhiteSpace(gatewayResult)
            && !gatewayResult.Contains("Error", StringComparison.OrdinalIgnoreCase)
            && !gatewayResult.Contains("Failed", StringComparison.OrdinalIgnoreCase);

        return new Response
        {
            Status = isSuccess,
            Message = isSuccess ? "OTP sent successfully." : gatewayResult
        };
    }
}
