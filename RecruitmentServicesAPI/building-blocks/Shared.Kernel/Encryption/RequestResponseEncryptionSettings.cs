namespace Shared.Kernel.Encryption
{
    /// <summary>
    /// Config for API request/response payload encrypt-decrypt (AES).
    /// Bound from appsettings section <c>RequestResponseEncryption</c>.
    /// </summary>
    public class RequestResponseEncryptionSettings
    {
        public const string SectionName = "RequestResponseEncryption";

        /// <summary>Master switch for request/response payload encryption.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>Logical key version/name for rotation (e.g. MahaRec-ReqRes-AES256-v1).</summary>
        public string KeyId { get; set; } = string.Empty;

        /// <summary>
        /// Shared secret used with PBKDF2.
        /// Must match Angular <c>requestResponseEncryptionKey</c> in app-config.json.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>PBKDF2 iteration count (must match Angular RequestResponseCryptoService).</summary>
        public int Iterations { get; set; } = 10000;
    }
}
