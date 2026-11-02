using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentPortal.Core.Common
{
    public class MessageRequest
    {
        public string? appid { get; set; }
        public string? userId { get; set; }

        public string? pass { get; set; }

        public string? contenttype { get; set; }

        public string? from { get; set; }
        public string? to { get; set; }

        public string? alert { get; set; }


        public string? selfid { get; set; }

        public string? intflag { get; set; }



        public string? text { get; set; }


    }
    public class EnterpriseMessageRequest
    {
        public string? appid { get; set; }
        public string? userId { get; set; }

        public string? pass { get; set; }

        public string? contenttype { get; set; }

        public string? from { get; set; }
        public string? to { get; set; }

        public string? alert { get; set; }


        public string? selfid { get; set; }

        public string? intflag { get; set; }



        public string? text { get; set; }


    }
}
