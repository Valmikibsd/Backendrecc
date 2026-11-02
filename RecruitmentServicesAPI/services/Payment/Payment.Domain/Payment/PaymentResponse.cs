namespace Payment.Domain.Payment
{
    public class PaymentResponse
    {
        public bool Success { get; set; }

        public string? AppTranId { get; set; }

        public string? PayId { get; set; }

        public string? PaymentUrl { get; set; }

        public string? Message { get; set; }

        public string? RawResponse { get; set; }
    }
}
