using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    public class WrittenScheduleSearchRequest
    {
        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public string UnitId { get; set; } = string.Empty;

        public int Gender { get; set; }
    }
}
