namespace Payment.Application.Interfaces
{
    public interface IPaymentChecksumService
    {
        string GenerateToken(string password, string timestamp);
        uint GenerateChecksum(string rawMessage);
    }
}
