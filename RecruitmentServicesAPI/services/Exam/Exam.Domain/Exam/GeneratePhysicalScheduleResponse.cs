using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam.Domain.Exam
{
    public class GeneratePhysicalScheduleResponse
    {
        public bool Success { get; set; }

        public int GeneratedCount { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
