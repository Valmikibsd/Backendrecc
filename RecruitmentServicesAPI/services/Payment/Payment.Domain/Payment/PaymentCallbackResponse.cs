namespace Payment.Domain.Payment
{
    public class PaymentCallbackResponse
    {
        public long CallbackId { get; set; }

        public bool Success { get; set; }

        public string? Message { get; set; }

        public string? CallbackStatus { get; set; }

        public string? AppTranId { get; set; }

        public string? TransactionRefNo { get; set; }

        public string? BankTranRefNo { get; set; }

        public string? BankId { get; set; }

        public decimal? Amount { get; set; }

        public string? Ud1 { get; set; }
        public string? Ud2 { get; set; }
        public string? Ud3 { get; set; }
        public string? Ud4 { get; set; }
        public string? Ud5 { get; set; }

        public string? TransactionNo { get; set; }

        public string? PayeeName { get; set; }

        public decimal? TotalAmount { get; set; }

        public decimal? PortalFee { get; set; }

        public string? CallbackErrMsg { get; set; }

        public string? ReceiptNo { get; set; }

        public string? CallbackResult { get; set; }

        public bool? ChecksumValid { get; set; }

        public string? PayStatusResult { get; set; }
    }
}
