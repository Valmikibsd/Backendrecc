using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Domain.Master
{
    public class WrittenHallTicketInstructionModel
    {
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int AdvertisementId { get; set; }
        public int PostId { get; set; }

        public string? DepartmentName { get; set; }
        public string? AdvertisementName { get; set; }
        public string? PostName { get; set; }
        public string Instruction { get; set; } = string.Empty;
    }
}
