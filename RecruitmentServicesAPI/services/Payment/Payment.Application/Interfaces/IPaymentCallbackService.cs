using Payment.Domain.Payment;

namespace Payment.Application.Interfaces
{
    public interface IPaymentCallbackService
    {
        Task<PaymentCallbackResponse> ProcessCallbackAsync(string msg);
        Task<PaymentCallbackResponse> SyncApplicationPaymentAsync(PaymentSyncRequest request);
    }
}
