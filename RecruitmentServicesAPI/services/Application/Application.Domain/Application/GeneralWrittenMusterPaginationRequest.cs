using System.ComponentModel.DataAnnotations;

namespace Application.Domain.Application
{
    public class GeneralWrittenMusterPaginationRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId must be a valid value.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PostId must be a valid value.")]
        public int PostId { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [StringLength(100)]
        public string? Search { get; set; }
    }
}
