using CrudOperations;
using Payment.Application.Interfaces;

namespace Payment.Infrastructure.Payment
{
    public class PaymentApplicationStatusRepository : IPaymentApplicationStatusRepository
    {
        private readonly ICrudOperationService _crud;

        public PaymentApplicationStatusRepository(ICrudOperationService crud)
        {
            _crud = crud;
        }

        public async Task<(bool Status, string Message)> UpdatePayStatusAsync(
            string applicationNo,
            int departmentId,
            int advertisementId,
            int postId,
            int payStatus,
            DateTime payDate)
        {
            var result = await _crud.InsertUpdateDelete<PayStatusResult>(
                "[dbo].[Usp_UpdateApplicationPayStatus]",
                new
                {
                    ApplicationNo = applicationNo,
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId,
                    PostId = postId,
                    PayStatus = payStatus,
                    PayDate = payDate
                },
                ConnectionNames.Applications);

            if (result == null)
            {
                return (false, "Application payment update returned no result.");
            }

            return (result.Status, result.Message ?? string.Empty);
        }

        public async Task<bool> IsApplicationPaidAsync(
            string applicationNo,
            int departmentId,
            int advertisementId,
            int postId)
        {
            var payStatus = await _crud.InsertUpdateDelete<int?>(
                "[dbo].[Usp_GetApplicationPayStatus]",
                new
                {
                    ApplicationNo = applicationNo,
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId,
                    PostId = postId
                },
                ConnectionNames.Applications);

            return (payStatus ?? 0) == 1;
        }

        private sealed class PayStatusResult
        {
            public bool Status { get; set; }
            public string? Message { get; set; }
        }
    }
}
