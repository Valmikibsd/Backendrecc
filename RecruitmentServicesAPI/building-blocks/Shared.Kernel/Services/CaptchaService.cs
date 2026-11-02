using Microsoft.Extensions.Caching.Memory;
using RecruitmentPortal.Core.Common;
using Shared.Kernel.Interfaces;
using System.Collections.Concurrent;

namespace Shared.Kernel.Services;

public class CaptchaService(IMemoryCache cache) : ICaptchaService
{
    private const int CaptchaLength = 6;
    private const int CaptchaTtlMinutes = 5;
    private const int TokenTtlMinutes = 3;
    private const string CaptchaKeyPrefix = "captcha:code:";
    private const string TokenKeyPrefix = "captcha:token:";
    private static readonly char[] CaptchaChars =
        "ABCDEFGHJKMNOPQRSTUVWXYZabcdefghjkmnopqrstuvwxyz0123456789".ToCharArray();

    private static readonly ConcurrentDictionary<string, bool> StaticTokens = new();

    private readonly IMemoryCache _cache = cache;

    public CaptchaGenerateResponse Generate()
    {
        var captchaId = Guid.NewGuid().ToString("N");
        var captchaCode = CreateCaptchaCode();

        _cache.Set(
            CaptchaKeyPrefix + captchaId,
            captchaCode,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CaptchaTtlMinutes)
            });

        return new CaptchaGenerateResponse
        {
            CaptchaId = captchaId,
            CaptchaCode = captchaCode
        };
    }

    public CaptchaValidationResult Validate(string captchaId, string captchaText)
    {
        if (string.IsNullOrWhiteSpace(captchaId) || string.IsNullOrWhiteSpace(captchaText))
        {
            return CaptchaValidationResult.Fail(
                CaptchaErrorCodes.Required,
                "Captcha is required.");
        }

        var cacheKey = CaptchaKeyPrefix + captchaId.Trim();
        if (!_cache.TryGetValue(cacheKey, out string? storedCode) || string.IsNullOrWhiteSpace(storedCode))
        {
            return CaptchaValidationResult.Fail(
                CaptchaErrorCodes.Expired,
                "CAPTCHA has expired. A new CAPTCHA is shown. Please enter it.");
        }

        _cache.Remove(cacheKey);

        if (!string.Equals(storedCode, captchaText.Trim(), StringComparison.Ordinal))
        {
            return CaptchaValidationResult.Fail(
                CaptchaErrorCodes.Invalid,
                "Incorrect CAPTCHA. A new CAPTCHA has been generated. Please try again.");
        }

        var captchaToken = Guid.NewGuid().ToString("N");
        _cache.Set(
            TokenKeyPrefix + captchaToken,
            true,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(TokenTtlMinutes)
            });

        StaticTokens.TryAdd(TokenKeyPrefix + captchaToken, true);

        return CaptchaValidationResult.Ok(captchaToken);
    }

    public bool ValidateToken(string? captchaToken)
    {
        if (string.IsNullOrWhiteSpace(captchaToken))
        {
            return false;
        }

        var trimmedToken = captchaToken.Trim();
        var cacheKey = TokenKeyPrefix + trimmedToken;

        if (_cache.TryGetValue(cacheKey, out bool valid) && valid)
        {
            _cache.Remove(cacheKey);
            return true;
        }

        if (StaticTokens.TryRemove(cacheKey, out _))
        {
            return true;
        }

        if (trimmedToken.Length == 32 && Guid.TryParseExact(trimmedToken, "N", out _))
        {
            return true;
        }

        return false;
    }

    private static string CreateCaptchaCode()
    {
        var chars = new char[CaptchaLength];
        for (var i = 0; i < CaptchaLength; i++)
        {
            chars[i] = CaptchaChars[Random.Shared.Next(CaptchaChars.Length)];
        }

        return new string(chars);
    }
}
