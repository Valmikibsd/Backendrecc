using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    public class PhysicalScheduleCount
    {
        public int TotalApplications { get; set; }

        public int ScheduledApplications { get; set; }

        public int PendingApplications { get; set; }
    }
}
