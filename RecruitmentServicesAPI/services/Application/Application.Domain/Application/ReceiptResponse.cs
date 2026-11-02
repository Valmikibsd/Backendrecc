using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Application
{
    public class ReceiptResponse
    {
        public string? Post { get; set; }

        public string? ApplicationNumber { get; set; }

        public string? CandidateName { get; set; }

       

        public decimal? Amount { get; set; }

        public string? Paystatus { get; set; }

        public string? PayDate { get; set; }

        public string? FeesInWords { get; set; }

        public string? ServiceName { get; set; }

      
    }
}
