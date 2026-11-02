namespace Payment.Domain.Payment
{
    public class PaymentRequestLog
    {
        public long Id { get; set; }

        public string AppTranID { get; set; } = string.Empty;

        public string ServiceID { get; set; } = string.Empty;

        public string SID { get; set; } = string.Empty;

        public string ClientID { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string IsReceipt { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Udf1 { get; set; }
        public string? Udf2 { get; set; }
        public string? Udf3 { get; set; }
        public string? Udf4 { get; set; }
        public string? Udf5 { get; set; }

        public string? ReturnUrl { get; set; }

        public string? Timestamp { get; set; }

        public string? Token { get; set; }

        public string? Checksum { get; set; }

        public string? RequestUrl { get; set; }

        public string? RequestData { get; set; }

        public string? ReqStatus { get; set; }

        public string? ReqResMsg { get; set; }

        public string? ReqErrMsg { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ResponseOn { get; set; }
    }
}
