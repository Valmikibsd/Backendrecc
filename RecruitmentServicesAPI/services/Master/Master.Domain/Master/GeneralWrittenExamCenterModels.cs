using System.ComponentModel.DataAnnotations;

namespace Master.Domain.Models
{
    public class GeneralWrittenExamCenterRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }
    }

    public class GeneralWrittenExamCenterModel
    {
        public int CenterId { get; set; }

        public string CenterName { get; set; } = string.Empty;
    }
}
