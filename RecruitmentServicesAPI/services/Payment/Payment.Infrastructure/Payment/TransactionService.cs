using Payment.Application.Interfaces;

namespace Payment.Infrastructure.Payment
{
    public class TransactionService : ITransactionService
    {
        public string GenerateTransactionId()
        {
            return $"PAY{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
        }
    }
}
