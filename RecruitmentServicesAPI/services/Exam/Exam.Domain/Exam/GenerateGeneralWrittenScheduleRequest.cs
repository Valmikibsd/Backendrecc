using System;
using System.ComponentModel.DataAnnotations;

namespace Exam.Domain.Exam
{
    public class GenerateGeneralWrittenScheduleRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Gender is required.")]
        public int Gender { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Written exam center is required.")]
        [StringLength(250, ErrorMessage = "Center name must not exceed 250 characters.")]
        public string CenterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Scheduled date is required.")]
        public DateTime? ScheduledDate { get; set; }

        [Required(ErrorMessage = "Scheduled time is required.")]
        [StringLength(100, ErrorMessage = "Scheduled time must not exceed 100 characters.")]
        public string ScheduledTime { get; set; } = string.Empty;
    }
}
