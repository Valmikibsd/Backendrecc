using System.ComponentModel.DataAnnotations;

namespace Exam.Domain.Exam
{
    public class GeneralWrittenScheduleSearchRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Gender is required.")]
        public int Gender { get; set; }
    }
}
