namespace Payment.Application.Interfaces
{
    public interface IAdvertisementPayServiceRepository
    {
        Task<string?> GetPayServiceIdAsync(
            int departmentId,
            int advertisementId,
            int postId);
    }
}
