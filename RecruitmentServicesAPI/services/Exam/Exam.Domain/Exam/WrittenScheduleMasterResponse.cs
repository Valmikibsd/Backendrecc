using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    public class WrittenScheduleMasterResponse
    {
      
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? AdvertisementId { get; set; }
        public string? AdvertisementNo { get; set; }
        public string? AdvertisementName { get; set; }
        public string? ExamYear { get; set; }
        public int? PostId { get; set; }
        public string? PostName { get; set; }
        public string? PostNameMarathi { get; set; }
        public string? UnitId { get; set; }
        public string? UnitName { get; set; }
        public int? GenderId { get; set; }
        public string? Gender { get; set; }
        public int? CenterId { get; set; }
        public string? CenterName { get; set; }
    }
}
