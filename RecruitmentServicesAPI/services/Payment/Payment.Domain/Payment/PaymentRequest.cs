using System.ComponentModel.DataAnnotations;

namespace Payment.Domain.Payment
{
    public class PaymentRequest
    {
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public string? ServiceID { get; set; }

        public string Mobile { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool IsReceipt { get; set; }

        public string? Ud1 { get; set; }
        public string? Ud2 { get; set; }
        public string? Ud3 { get; set; }
        public string? Ud4 { get; set; }
        public string? Ud5 { get; set; }
        public string? Ud6 { get; set; }

        [Required(ErrorMessage = "Application number is required.")]
        [StringLength(50, ErrorMessage = "Application number cannot exceed 50 characters.")]
        public string? ApplicationNo { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
