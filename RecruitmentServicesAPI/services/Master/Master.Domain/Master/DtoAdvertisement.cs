using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Domain.Master
{
    // Used for Insert and Update
    public class DtoAdvertisement : IValidatableObject
    {
        public int AdvertisementId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Exam Year is required.")]
        [Range(2000, 9999, ErrorMessage = "Enter a valid 4 digit exam year.")]
        public int? ExamYear { get; set; }

        [Required(ErrorMessage = "Advertisement Number is required.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed.")]
        public string? AdvertisementNo { get; set; }

        [Required(ErrorMessage = "Advertisement Name is required.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed.")]
        public string? AdvertisementName { get; set; }

        [Required(ErrorMessage = "Advertisement Name (Marathi) is required.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed.")]
        public string? AdvertisementNameMarathi { get; set; }

        [Required(ErrorMessage = "Advertisement Description is required.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed.")]
        public string? AdvertisementDesc { get; set; }

        [Required(ErrorMessage = "Advertisement Start Date is required.")]
        public DateTime? AdvertisementStartDate { get; set; }

        [Required(ErrorMessage = "Advertisement End Date is required.")]
        public DateTime? AdvertisementEndDate { get; set; }

        public string? AdvertisementImage { get; set; }

        public bool IsActive { get; set; }

        public bool IsUnitApplicable { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AdvertisementStartDate.HasValue && AdvertisementEndDate.HasValue && AdvertisementEndDate.Value < AdvertisementStartDate.Value)
            {
                yield return new ValidationResult(
                    "Advertisement End Date cannot be before Advertisement Start Date.",
                    new[] { nameof(AdvertisementEndDate) });
            }
        }
    }

    // Used for List API with pagination/search
    public class DtoAdvertisementList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }


    // Used for Get List and Get By Id response
    public class AdvertisementMasterModel
    {
        public int AdvertisementId { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int? ExamYear { get; set; }

        public string? AdvertisementNo { get; set; }

        public string? AdvertisementName { get; set; }

        public string? AdvertisementNameMarathi { get; set; }

        public string? AdvertisementDesc { get; set; }

        public DateTime? AdvertisementStartDate { get; set; }

        public DateTime? AdvertisementEndDate { get; set; }

        public string? AdvertisementImage { get; set; }

        public bool IsActive { get; set; }

        public bool IsUnitApplicable { get; set; }
    }
}
