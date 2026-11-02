using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RecruitmentPortal.Core.Public
{
    public class DtoVerifyAadhaarOTP
    {
        public string TransactionId { get; set; } = string.Empty;

        public string OTP { get; set; } = string.Empty;

        public string Captcha { get; set; } = string.Empty;
    }
}