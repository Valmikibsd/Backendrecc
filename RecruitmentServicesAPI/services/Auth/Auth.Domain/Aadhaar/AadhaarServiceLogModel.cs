using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Aadhaar
{
    public class AadhaarServiceLogModel
    {
        public string? AadhaarNo { get; set; }

        public string? ServiceName { get; set; }

        public string? ServiceUrl { get; set; }

        public string? Request { get; set; }

        public string? Response { get; set; }

        public bool? IsError { get; set; }

        public string? IPAddress { get; set; }
    }
}
