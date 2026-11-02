using System.ComponentModel.DataAnnotations;

namespace Application.Domain.Application
{
    public class GeneralWrittenAttendancePdfRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId must be a valid value.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PostId must be a valid value.")]
        public int PostId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "FilterId must be a valid value.")]
        public int FilterId { get; set; }

        [Required(ErrorMessage = "PdfPath is required.")]
        [StringLength(500)]
        public string PdfPath { get; set; } = string.Empty;
    }
}
