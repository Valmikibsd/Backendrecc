using CrudOperations;
using Payment.Application.Interfaces;
using Payment.Domain.Payment;

namespace Payment.Infrastructure.Payment
{
    public class PaymentRequestLogRepository : IPaymentRequestLogRepository
    {
        private readonly ICrudOperationService _crud;

        public PaymentRequestLogRepository(ICrudOperationService crud)
        {
            _crud = crud;
        }

        public async Task<long> SaveRequestAsync(PaymentRequestLog log)
        {
            var result = await _crud.InsertUpdateDelete<SaveIdResult>(
                "[dbo].[SavePaymentRequestLog]",
                new
                {
                    log.AppTranID,
                    log.ServiceID,
                    log.ClientID,
                    log.Amount,
                    log.IsReceipt,
                    log.Email,
                    log.Mobile,
                    log.Name,
                    log.Udf1,
                    log.Udf2,
                    log.Udf3,
                    log.Udf4,
                    log.Udf5,
                    log.ReturnUrl,
                    log.Timestamp,
                    log.Token,
                    log.Checksum,
                    log.RequestUrl,
                    log.RequestData,
                    log.ReqStatus
                },
                ConnectionNames.Payment);

            return result?.Id ?? 0;
        }

        public async Task<bool> UpdateResponseAsync(
            string appTranId,
            string gatewayResponse,
            string status,
            string? errorMessage = null)
        {
            var result = await _crud.InsertUpdateDelete<UpdateResult>(
                "[dbo].[UpdatePaymentRequestLog]",
                new
                {
                    AppTranID = appTranId,
                    ReqResMsg = gatewayResponse,
                    ReqStatus = status,
                    ReqErrMsg = errorMessage
                },
                ConnectionNames.Payment);

            return result?.Result == 1;
        }

        public async Task<PaymentRequestLog?> GetPaymentRequestAsync(string appTranId)
        {
            return await _crud.InsertUpdateDelete<PaymentRequestLog>(
                "[dbo].[GetPaymentRequest]",
                new { AppTranID = appTranId },
                ConnectionNames.Payment);
        }

        private sealed class SaveIdResult
        {
            public long Id { get; set; }
        }

        private sealed class UpdateResult
        {
            public int Result { get; set; }
        }
    }
}
