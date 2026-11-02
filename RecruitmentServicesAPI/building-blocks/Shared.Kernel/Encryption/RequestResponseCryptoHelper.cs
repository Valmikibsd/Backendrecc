using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace Shared.Kernel.Encryption
{
    /// <summary>
    /// Encrypt/decrypt helper for HTTP request and response payloads.
    /// AES-256-CBC + PBKDF2-SHA256. Wire format: saltHex:ivHex:cipherBase64
    /// </summary>
    public class RequestResponseCryptoHelper
    {
        private const int KeySizeBytes = 32;
        private const int SaltSizeBytes = 16;
        private const int IvSizeBytes = 16;

        private readonly string _secretKey;
        private readonly int _iterations;

        public RequestResponseCryptoHelper(IOptions<RequestResponseEncryptionSettings> options)
        {
            var settings = options?.Value ?? new RequestResponseEncryptionSettings();
            if (string.IsNullOrWhiteSpace(settings.SecretKey))
            {
                throw new InvalidOperationException(
                    "RequestResponseEncryption:SecretKey is not configured. Use a MahaRecruitment project-specific secret (KeyId recommended).");
            }

            _secretKey = settings.SecretKey.Trim();
            _iterations = settings.Iterations > 0 ? settings.Iterations : 10000;
        }

        public string Encrypt(string plainText)
        {
            if (plainText == null)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            byte[] iv = RandomNumberGenerator.GetBytes(IvSizeBytes);
            byte[] key = DeriveKey(salt);

            using var aes = Aes.Create();
            aes.KeySize = KeySizeBytes * 8;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return $"{Convert.ToHexString(salt)}:{Convert.ToHexString(iv)}:{Convert.ToBase64String(cipherBytes)}";
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentException("Encrypted text is empty.", nameof(encryptedText));
            }

            encryptedText = encryptedText.Trim().Trim('"');

            string[] parts = encryptedText.Split(':');
            if (parts.Length != 3)
            {
                throw new FormatException("Invalid encrypted text format. Expecting salt:iv:cipher.");
            }

            byte[] salt = Convert.FromHexString(parts[0]);
            byte[] iv = Convert.FromHexString(parts[1]);
            byte[] cipherBytes = Convert.FromBase64String(parts[2]);
            byte[] key = DeriveKey(salt);

            using var aes = Aes.Create();
            aes.KeySize = KeySizeBytes * 8;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }

        public static bool LooksEncrypted(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Trim().Trim('"');
            string[] parts = value.Split(':');
            if (parts.Length != 3)
            {
                return false;
            }

            return parts[0].Length == SaltSizeBytes * 2
                && parts[1].Length == IvSizeBytes * 2
                && parts[2].Length > 0;
        }

        private byte[] DeriveKey(byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                _secretKey,
                salt,
                _iterations,
                HashAlgorithmName.SHA256,
                KeySizeBytes);
        }
    }
}
