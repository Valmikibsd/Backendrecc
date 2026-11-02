using Payment.Domain.Payment;
namespace Payment.Application.Interfaces
{
    public interface IPaymentRequestLogRepository
    {
        Task<long> SaveRequestAsync(PaymentRequestLog log);
        Task<bool> UpdateResponseAsync(string appTranId,string gatewayResponse,string status,string? errorMessage = null);
        Task<PaymentRequestLog?> GetPaymentRequestAsync(string appTranId);
    }
}
