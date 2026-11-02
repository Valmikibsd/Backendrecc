using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    
  public class GeneratePhysicalScheduleRequest
    {
        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public string PostId { get; set; } = string.Empty;

        public string UnitId { get; set; } = string.Empty;

        public int Gender { get; set; }

        public string? Category { get; set; } 

        public int ExService { get; set; }

        public int SportMan { get; set; }

        public int Capacity { get; set; }

        public string CenterName { get; set; } = string.Empty;

        public DateTime ScheduledDate { get; set; }

        public string ScheduledTime { get; set; } = string.Empty;
    }
}
