using CrudOperations;
using Payment.Application.Interfaces;
using Payment.Domain.Payment;

namespace Payment.Infrastructure.Payment
{
    public class PaymentCallbackRepository : IPaymentCallbackRepository
    {
        private readonly ICrudOperationService _crud;

        public PaymentCallbackRepository(ICrudOperationService crud)
        {
            _crud = crud;
        }

        public async Task<long> SaveCallbackAsync(
            PaymentCallbackResponse response,
            string rawResponse)
        {
            var result = await _crud.InsertUpdateDelete<SaveIdResult>(
                "[dbo].[SavePaymentCallback]",
                new
                {
                    response.AppTranId,
                    response.CallbackStatus,
                    response.TransactionRefNo,
                    response.BankTranRefNo,
                    response.BankId,
                    response.Amount,
                    response.Ud1,
                    response.Ud2,
                    response.Ud3,
                    response.Ud4,
                    response.Ud5,
                    response.TransactionNo,
                    response.PayeeName,
                    response.TotalAmount,
                    response.PortalFee,
                    response.CallbackErrMsg,
                    response.ReceiptNo,
                    IsSuccess = response.Success,
                    CallbackMsg = rawResponse,
                    response.CallbackResult,
                    response.ChecksumValid,
                    response.PayStatusResult
                },
                ConnectionNames.Payment);

            return result?.Id ?? 0;
        }

        public async Task<bool> HasApplicationUpdatedAsync(string appTranId)
        {
            var count = await _crud.InsertUpdateDelete<int?>(
                "[dbo].[HasPaymentUpdated]",
                new { AppTranId = appTranId },
                ConnectionNames.Payment);

            return (count ?? 0) > 0;
        }

        public async Task<PaymentCallbackResponse?> GetPaymentCallbackAsync(string appTranId)
        {
            return await _crud.InsertUpdateDelete<PaymentCallbackResponse>(
                "[dbo].[GetPaymentCallback]",
                new { AppTranId = appTranId },
                ConnectionNames.Payment);
        }

        private sealed class SaveIdResult
        {
            public long Id { get; set; }
        }
    }
}
