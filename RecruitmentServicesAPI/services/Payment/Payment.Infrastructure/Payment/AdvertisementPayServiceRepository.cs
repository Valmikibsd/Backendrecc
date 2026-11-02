using CrudOperations;
using Payment.Application.Interfaces;

namespace Payment.Infrastructure.Payment
{
    public class AdvertisementPayServiceRepository : IAdvertisementPayServiceRepository
    {
        private readonly ICrudOperationService _crud;

        public AdvertisementPayServiceRepository(ICrudOperationService crud)
        {
            _crud = crud;
        }

        public async Task<string?> GetPayServiceIdAsync(
            int departmentId,
            int advertisementId,
            int postId)
        {
            if (departmentId <= 0 || advertisementId <= 0 || postId <= 0)
            {
                return null;
            }

            string? payServiceId = await _crud.InsertUpdateDelete<string>(
                "[dbo].[GetAdvertisementPayServiceId]",
                new
                {
                    DepartmentId = departmentId,
                    AdvertisementId = advertisementId,
                    PostId = postId
                },
                ConnectionNames.Master);

            if (string.IsNullOrWhiteSpace(payServiceId))
            {
                return null;
            }

            return payServiceId.Trim();
        }
    }
}
