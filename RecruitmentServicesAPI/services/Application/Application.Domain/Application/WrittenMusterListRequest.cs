using System.ComponentModel.DataAnnotations;

namespace Application.Domain.Application
{
    public class WrittenMusterListRequest
    {
        [Required(ErrorMessage = "FilterId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "FilterId must be a valid value.")]
        public int FilterId { get; set; }

        public long? UserId { get; set; }
    }
}
