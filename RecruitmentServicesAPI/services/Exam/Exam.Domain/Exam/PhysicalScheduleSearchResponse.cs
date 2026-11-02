using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    public class PhysicalScheduleSearchResponse
    {
        public int TotalApplications { get; set; }

        public int AvailableApplications { get; set; }

        public int ScheduledApplications { get; set; }

        public int PendingApplications { get; set; }

        public List<PhysicalScheduleApplication> Applications { get; set; } = new();
    }

}