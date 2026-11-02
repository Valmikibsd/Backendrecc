using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    public class PhysicalScheduleSearchRequest
    {
        public int DepartmentId { get; set; }
        public int AdvertisementId { get; set; }
        public int PostId { get; set; }

        public string? UnitId { get; set; }

        public int Gender { get; set; }

        public string? Category { get; set; }

        public int ExService { get; set; }

        public int SportMan { get; set; }

        public int Capacity { get; set; }

        public DateTime ScheduledDate { get; set; }
    }


}
