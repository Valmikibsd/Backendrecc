using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Payment
{
    public class PaymentSyncRequest
    {
        [Required(ErrorMessage = "Application number is required.")]
        [StringLength(50, ErrorMessage = "Application number cannot exceed 50 characters.")]
        public string? ApplicationNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be greater than zero.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId must be greater than zero.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than zero.")]
        public int PostId { get; set; }
    }
}
