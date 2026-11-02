using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Payment
{
    public class PaymentResult
    {
        public string? AppId { get; set; }

        public string? PaymentTransactionId { get; set; }

        public string? BankId { get; set; }

        public string? BankRefId { get; set; }

        public string? BankRefNo { get; set; }

        public string? PaymentMessage { get; set; }

        public int? PayStatus { get; set; }

        public string? PayMode { get; set; }

        public string? TransPayStatus { get; set; }

        public decimal? Amount { get; set; }

        public string? PaymentDate { get; set; }

        public string? Checksum { get; set; }

        public bool Success { get; set; }

        public string? Message { get; set; }
    }
}
