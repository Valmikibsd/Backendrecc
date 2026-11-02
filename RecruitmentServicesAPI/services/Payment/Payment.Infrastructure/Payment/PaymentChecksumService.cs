using Payment.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Payment.Infrastructure.Payment
{
    public class PaymentChecksumService : IPaymentChecksumService
    {
        public string GenerateToken(string password, string timestamp)
        {
            string md5Password = GenerateMD5(password).ToUpperInvariant();
            string tokenInput = md5Password + timestamp;
            return GenerateSHA256(tokenInput);
        }

        public uint GenerateChecksum(string rawMessage)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(rawMessage);
            return CRC32.Compute(bytes);
        }

        private static string GenerateMD5(string value)
        {
            using MD5 md5 = MD5.Create();
            byte[] input = Encoding.UTF8.GetBytes(value);
            byte[] hash = md5.ComputeHash(input);
            return Convert.ToHexString(hash);
        }

        private static string GenerateSHA256(string value)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] input = Encoding.UTF8.GetBytes(value);
            byte[] hash = sha256.ComputeHash(input);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
