using Payment.Domain.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Interfaces
{
    public interface IPaymentCallbackRepository
    {
        Task<long> SaveCallbackAsync(PaymentCallbackResponse response,string rawResponse);
        Task<bool> HasApplicationUpdatedAsync(string appTranId);
        Task<PaymentCallbackResponse?> GetPaymentCallbackAsync(string appTranId);
    }
}
