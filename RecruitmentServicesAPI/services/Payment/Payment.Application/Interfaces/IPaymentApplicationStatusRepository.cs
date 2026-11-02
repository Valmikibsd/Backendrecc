namespace Payment.Application.Interfaces
{
    public interface IPaymentApplicationStatusRepository
    {
        Task<(bool Status, string Message)> UpdatePayStatusAsync(
            string applicationNo,
            int departmentId,
            int advertisementId,
            int postId,
            int payStatus,
            DateTime payDate);

        Task<bool> IsApplicationPaidAsync(
            string applicationNo,
            int departmentId,
            int advertisementId,
            int postId);
    }
}
