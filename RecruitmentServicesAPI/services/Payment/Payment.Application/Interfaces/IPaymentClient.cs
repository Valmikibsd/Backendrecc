using Payment.Domain.Payment;

namespace Payment.Application.Interfaces
{
    public interface IPaymentClient
    {
        Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request, string transactionId);
    }
}
