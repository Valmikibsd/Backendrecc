using Payment.Domain.Payment;

namespace Payment.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> InitiatePaymentAsync(PaymentRequest request);
    }
}
