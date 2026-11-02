using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Public
{
    public class DtoGenerateAadhaarOTP
    {
        public string AadhaarNumber { get; set; }
        public string Captcha { get; set; }
    }
}
