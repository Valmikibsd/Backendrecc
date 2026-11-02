using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Domain.Models
{
    public class DepartmentDetailsModel
    {
        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public string? DepartmentName_Mr { get; set; }

        // NEW - Base64 Department Logo
        public string? Logo { get; set; }

        public int ActiveAdvertisements { get; set; }

        public int Vacancies { get; set; }
    }

    public class DepartmentAdvertisementModel
    {
        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
        public string? DepartmentName_Mr { get; set; }

        public int AdvertisementId { get; set; }

        public string? AdvertisementName { get; set; }
        public string? AdvertisementNameMarathi { get; set; }

        public bool IsUnitApplicable { get; set; }
        public string? AdvertisementImage { get; set; }
    }

    public class DepartmentPostModel
    {
        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
        public string? DepartmentName_Mr { get; set; }

        public int AdvertisementId { get; set; }

        public string? AdvertisementNo { get; set; }

        public string? AdvertisementName { get; set; }
        public string? AdvertisementNameMarathi { get; set; }

        public int PostId { get; set; }

        public string? PostName { get; set; }
        public string? PostNameMarathi { get; set; }

        public DateTime RecruitmentStartDate { get; set; }

        public DateTime RecruitmentEndDate { get; set; }
        public DateTime? AgeOnDate { get; set; }
        public int Vacancies { get; set; }
        public string? Qualification { get; set; }
        public bool IsUnit { get; set; }
    }
    public class DepartmentRequest
    {
        public int DepartmentId { get; set; }
    }

    public class DepartmentPostRequest
    {
        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }
    }

    public class DtoAdvertisementPost
    {
        public int AdvertisementPostId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Recruitment Start Date is required.")]
        public DateTime? RecruitmentStartDate { get; set; }

        [Required(ErrorMessage = "Recruitment End Date is required.")]
        public DateTime? RecruitmentEndDate { get; set; }

        [Required(ErrorMessage = "Payment Start Date is required.")]
        public DateTime? PaymentStartDate { get; set; }

        [Required(ErrorMessage = "Payment End Date is required.")]
        public DateTime? PaymentEndDate { get; set; }

        public DateTime? CancellationEndDate { get; set; }

        [Required(ErrorMessage = "Age On Date is required.")]
        public DateTime? AgeOnDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vacancies must be at least 1.")]
        public int Vacancies { get; set; }

        [Required(ErrorMessage = "Qualification is required.")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters allowed.")]
        public string? Qualification { get; set; }

        [Required(ErrorMessage = "Pay Service Id is required.")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters allowed.")]
        public string? PayServiceId { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }
    }

    public class DtoAgeCriteria
    {
        public long? AgeCriteriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Gender is required.")]
        public int GenderId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public int? ParallelReservationId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum Age is required.")]
        public int MinimumAge { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Maximum Age is required.")]
        public int MaximumAge { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }
    }

    public class DtoAgeCriteriaList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }

        public int? DepartmentId { get; set; }

        public int? AdvertisementId { get; set; }

        public int? PostId { get; set; }

        public int? GenderId { get; set; }

        public int? CategoryId { get; set; }
    }

    public class AgeCriteriaImportErrorModel
    {
        public int RowNumber { get; set; }

        public string Reason { get; set; } = string.Empty;
    }

    public class AgeCriteriaModel
    {
        public long AgeCriteriaId { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int AdvertisementId { get; set; }

        public string AdvertisementName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;

        public int GenderId { get; set; }

        public string GenderName { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int? ParallelReservationId { get; set; }

        public string? ParallelReservationName { get; set; }

        public int MinimumAge { get; set; }

        public int MaximumAge { get; set; }

        public bool IsActive { get; set; }
    }

    public class DtoAdvertisementPostList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class AdvertisementPostModel
    {
        public int AdvertisementPostId { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int AdvertisementId { get; set; }

        public string AdvertisementName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;

        public DateTime RecruitmentStartDate { get; set; }

        public DateTime RecruitmentEndDate { get; set; }

        public DateTime PaymentStartDate { get; set; }

        public DateTime PaymentEndDate { get; set; }

        public DateTime? CancellationEndDate { get; set; }

        public DateTime AgeOnDate { get; set; }

        public int Vacancies { get; set; }

        public string Qualification { get; set; } = string.Empty;

        public string PayServiceId { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }


    public class PostMasterModel
    {
        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;
    }

    public class DtoMenuItem
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public string? MenuNameMarathi { get; set; }

        public string? RouteUrl { get; set; }

        public int? ParentMenuId { get; set; }

        public string? Icon { get; set; }

        public int SortOrder { get; set; }
    }

    public class DtoRoleItem
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string RoutePrefix { get; set; } = string.Empty;
    }

    public class DepartmentCandidatePostModel
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
        public string? DepartmentName_Mr { get; set; }

        public int AdvertisementId { get; set; }

        public string? AdvertisementNo { get; set; }

        public string? AdvertisementName { get; set; }
        public string? AdvertisementNameMarathi { get; set; }

        public int PostId { get; set; }

        public string? PostName { get; set; }
        public string? PostNameMarathi { get; set; }

        public DateTime RecruitmentStartDate { get; set; }

        public DateTime RecruitmentEndDate { get; set; }

        public DateTime? AgeOnDate { get; set; }

        public int Vacancies { get; set; }

        public string? Qualification { get; set; }

        public string? IsEligible { get; set; }
        public bool IsUnit { get; set; }
    }

    public class DepartmentCandidatePostRequest
    {
        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }
    }

    public class AppliedPostModel
    {
        public string? AppNo { get; set; }
        public string? DepartmentName { get; set; }
        public string? AdvertisementName { get; set; }
        public string? PostName { get; set; }
        public string? CandidateName { get; set; }
        public DateTime AppliedDate { get; set; }
        public string? PaymentStatus { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public decimal Fees { get; set; }

        public string? EmailId { get; set; }

        public string? MobileNo { get; set; }
    }

    public class CandidateAppliedPostRequest
    {
        public long UserId { get; set; }
    }

    public class DtoDepartment
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department Code is required.")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters allowed.")]
        public string? DepartmentCode { get; set; }

        [Required(ErrorMessage = "Department Name is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "Department Name (Marathi) is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string? DepartmentName_Mr { get; set; }

        [Required(ErrorMessage = "Department Type is required.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed.")]
        public string? DepartmentType { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed.")]
        public string? Description { get; set; }

        public string? Logo { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }
    }

    public class DtoDepartmentList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class DepartmentModel
    {
        public int DepartmentId { get; set; }

        public string? DepartmentCode { get; set; }

        public string? DepartmentName { get; set; }

        public string? DepartmentName_Mr { get; set; }

        public string? DepartmentType { get; set; }

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }

    public class DtoPost
    {
        public int PostId { get; set; }
        public string? PostCode { get; set; }

        [Required(ErrorMessage = "Post Name is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string? PostName { get; set; }

        [Required(ErrorMessage = "Post Name (Marathi) is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string? PostNameMarathi { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }

    public class DtoPostList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class DtoHelpDetails
    {
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Help Details.")]
        public int HelpDetailsId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public string? DepartmentName_Mr { get; set; }

        [Required(ErrorMessage = "Department Contact No is required.")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Department Contact No must be 10 to 15 digits.")]
        public string? DepartmentContactNo { get; set; }

        [Required(ErrorMessage = "Department Email Id is required.")]
        [EmailAddress(ErrorMessage = "Invalid Department Email Id.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string? DepartmentEmailId { get; set; }

        [Required(ErrorMessage = "MahaIT Contact No is required.")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "MahaIT Contact No must be 10 to 15 digits.")]
        public string? MahaITContactNo { get; set; }

        [Required(ErrorMessage = "MahaIT Email Id is required.")]
        [EmailAddress(ErrorMessage = "Invalid MahaIT Email Id.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string? MahaITEmailId { get; set; }

        [Required(ErrorMessage = "Help Description is required.")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters allowed.")]
        public string? HelpDescription { get; set; }

        public bool IsActive { get; set; }

        public long UserId { get; set; }
    }

    public class NotificationModel
    {
        public int SrNo { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public string? DepartmentName_Mr { get; set; }

        public string? NotificationDetails { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? NotificationPdf { get; set; }

        public string? NotificationFileName { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }

    public class LatestNewsModel
    {
        public string? NotificationDetails { get; set; }

        public string? NotificationDetailsMarathi { get; set; }
    }

    public class HomeCountDetailsModel
    {
        public long RegistrationCount { get; set; }

        public long AdvertisementCount { get; set; }

        public long CompletedAdvertisementCount { get; set; }

        public long DepartmentCount { get; set; }
    }

    public class DtoNotification
    {
        public int SrNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Notification Details are required.")]
        [StringLength(500, ErrorMessage = "Maximum 500 characters allowed.")]
        public string? NotificationDetails { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        public DateTime? EndDate { get; set; }

        public string? NotificationPdf { get; set; }

        public string? NotificationFileName { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }
    }

    public class DtoNotificationList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class DtoGetNotificationDetails
    {
        [Range(1, int.MaxValue, ErrorMessage = "SrNo is required.")]
        public int SrNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
    }


    //Added By K.P on 08.08.2026
    public class CategoryMasterModel
    {
        public int CategoryId { get; set; }

        public string Category { get; set; } = string.Empty;
    }

    public class DtoFeesMasterList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class DtoFeesMasterDetailsPost
    {
        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public int CategoryId { get; set; }

        public decimal Amount { get; set; }

        public long CreatedBy { get; set; }

        public int FeesCriteriaId { get; set; }

        public bool? IsActive { get; set; }
    }

    public class FeesMasterDEtailsModel
    {
        public int FeesCriteriaId { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int AdvertisementId { get; set; }

        public string AdvertisementName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; } = string.Empty;

        public decimal Fees { get; set; }
    }


    // Added by valmiki 13082026 

    public class DDlList
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    public class UnitDDlList
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    // Qualification Criteria Master Models
    public class DtoQualificationCriteriaList
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequireAtLeastOnePositiveIdAttribute : ValidationAttribute
    {
        public RequireAtLeastOnePositiveIdAttribute()
            : base("Please select at least one qualification.")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IEnumerable<int> list)
            {
                var enumerable = list as IList<int> ?? list.ToList();
                if (enumerable.Count > 0 && enumerable.Any(x => x > 0))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessageString);
        }
    }

    public class DtoQualificationCriteriaPost : IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }

        [RequireAtLeastOnePositiveId(ErrorMessage = "Please select at least one qualification.")]
        public List<int> QualificationIds { get; set; } = [];

        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (QualificationIds == null || QualificationIds.Count == 0 || QualificationIds.All(x => x <= 0))
            {
                yield return new ValidationResult("Please select at least one qualification.", [nameof(QualificationIds)]);
            }
        }
    }

    public class DtoQualificationCriteriaStatusUpdate
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid qualification criteria.")]
        public int SrNo { get; set; }

        public bool IsActive { get; set; }
    }

    public class DtoQualificationCriteriaByPostRequest
    {
        [Range(0, int.MaxValue, ErrorMessage = "Invalid Department.")]
        public int DepartmentId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid Advertisement.")]
        public int AdvertisementId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid Post.")]
        public int PostId { get; set; }
    }

    public class QualificationCriteriaModel
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int AdvertisementId { get; set; }

        public string AdvertisementName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;

        public string QualificationNames { get; set; } = string.Empty;

        public string QualificationIds { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    public class QualificationCriteriaDetailModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Selected { get; set; }

        public bool AlreadyConfigured { get; set; }

        public bool IsActive { get; set; }
    }

    public class UnitModel
    {
        public string UnitID { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public int? SortOrder { get; set; }
    }

    public class DtoDepartmentUserSave
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public int? AdvertisementId { get; set; }

        [StringLength(10)]
        public string? UnitID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
        public string Username { get; set; } = string.Empty;

        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(100)]
        public string? EmailId { get; set; }

        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Mobile Number.")]
        [StringLength(10)]
        public string? MobileNo { get; set; }

        public bool IsActive { get; set; } = true;

        public long CreatedBy { get; set; }
    }

    public class DepartmentUserModel
    {
        public long UserId { get; set; }
        public string? Username { get; set; }
        public string? EmailId { get; set; }
        public string? MobileNo { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? AdvertisementId { get; set; }
        public string? AdvertisementName { get; set; }
        public string? UnitID { get; set; }
        public string? UnitName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class DtoDepartmentUserList
    {
        public int? DepartmentId { get; set; }
        public int? AdvertisementId { get; set; }
        public string? UnitID { get; set; }
        public string? SearchText { get; set; }
    }

    public class DtoDepartmentUserStatusToggle
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Valid UserId is required.")]
        public long UserId { get; set; }

        public bool IsActive { get; set; }
    }

    // Advertisement PDF Master Models
    public class DtoAdvertisementPdf
    {
        public long? AdvertisementPdfId { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public string? UnitId { get; set; }

        public string? FileName { get; set; }

        public string? FileType { get; set; }

        public byte[]? FileData { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public long UserId { get; set; }
    }

    public class DtoAdvertisementPdfList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class AdvertisementPdfModel
    {
        public long AdvertisementPdfId { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int AdvertisementId { get; set; }

        public string AdvertisementName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;

        public string? UnitId { get; set; }

        public string? UnitName { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    public class AdvertisementPdfFileModel
    {
        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public byte[] FileData { get; set; } = Array.Empty<byte>();
    }

    // Rule PDF Master Models
    public class DtoRulePdf
    {
        public long? RuleId { get; set; }

        public int DepartmentId { get; set; }

        public int AdvertisementId { get; set; }

        public int PostId { get; set; }

        public string? FileName { get; set; }

        public string? FileType { get; set; }

        public byte[]? FileData { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public long UserId { get; set; }
    }

    public class DtoRulePdfList
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public string? Search { get; set; }
    }

    public class RulePdfModel
    {
        public long RuleId { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public int AdvertisementId { get; set; }

        public string AdvertisementName { get; set; } = string.Empty;

        public int PostId { get; set; }

        public string PostName { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    public class RulePdfFileModel
    {
        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public byte[] FileData { get; set; } = Array.Empty<byte>();
    }

    public class DtoGetRulePdfById
    {
        [Range(1, long.MaxValue, ErrorMessage = "RuleId is required.")]
        public long RuleId { get; set; }
    }

    public class DtoGetPostById
    {
        [Range(1, int.MaxValue, ErrorMessage = "PostId is required.")]
        public int PostId { get; set; }
    }

    public class DtoGetNotification
    {
        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }
    }

    public class DtoGetDepartmentUserById
    {
        [Range(1, long.MaxValue, ErrorMessage = "Valid UserId is required.")]
        public long UserId { get; set; }
    }

    public class DtoGetAdvertisementPostById
    {
        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementPostId is required.")]
        public int AdvertisementPostId { get; set; }
    }

    public class DtoGetAdvertisementPdfById
    {
        [Range(1, long.MaxValue, ErrorMessage = "AdvertisementPdfId is required.")]
        public long AdvertisementPdfId { get; set; }
    }

    public class DtoPublicRulePdfList
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Department.")]
        public int? DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Advertisement.")]
        public int? AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Post.")]
        public int? PostId { get; set; }
    }

    public class DtoPublicAdvertisementPdfList
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Department.")]
        public int? DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Advertisement.")]
        public int? AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Post.")]
        public int? PostId { get; set; }
    }

    public class DtoGetAgeCriteriaById
    {
        [Range(1, long.MaxValue, ErrorMessage = "AgeCriteriaId is required.")]
        public long AgeCriteriaId { get; set; }
    }

    public class DtoGetCategory
    {
        public int? CategoryId { get; set; }
    }

    public class DtoGetDisabilitySubType
    {
        [Range(1, int.MaxValue, ErrorMessage = "DisabilityTypeId is required.")]
        public int DisabilityTypeId { get; set; }
    }

    public class DtoGetDepartmentById
    {
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required.")]
        public int DepartmentId { get; set; }
    }

    public class DtoGetAdvertisementById
    {
        [Range(1, int.MaxValue, ErrorMessage = "AdvertisementId is required.")]
        public int AdvertisementId { get; set; }
    }

    #region Education Master (Qualification, Course, Subject)
    public class DtoQualification
    {
        public int QualificationId { get; set; }

        [Required(ErrorMessage = "Qualification Type is required.")]
        [StringLength(50, ErrorMessage = "Qualification Type cannot exceed 50 characters.")]
        public string Qualification { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class QualificationMasterModel
    {
        public int QualificationId { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class DtoQualificationList
    {
        public string? Search { get; set; }
    }

    public class DtoQualificationStatusToggle
    {
        [Range(1, int.MaxValue, ErrorMessage = "QualificationId is required.")]
        public int QualificationId { get; set; }
        public bool IsActive { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class DtoGetQualificationById
    {
        [Range(1, int.MaxValue, ErrorMessage = "QualificationId is required.")]
        public int QualificationId { get; set; }
    }

    public class DtoCourse
    {
        public int CourseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Qualification Type is required.")]
        public int QualificationId { get; set; }

        [Required(ErrorMessage = "Name of Qualification is required.")]
        [StringLength(50, ErrorMessage = "Name of Qualification cannot exceed 50 characters.")]
        public string CourseName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class CourseMasterModel
    {
        public int CourseId { get; set; }
        public int QualificationId { get; set; }
        public string? Qualification { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class DtoCourseList
    {
        public int? QualificationId { get; set; }
        public string? Search { get; set; }
    }

    public class DtoCourseStatusToggle
    {
        [Range(1, int.MaxValue, ErrorMessage = "CourseId is required.")]
        public int CourseId { get; set; }
        public bool IsActive { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class DtoGetCourseById
    {
        [Range(1, int.MaxValue, ErrorMessage = "CourseId is required.")]
        public int CourseId { get; set; }
    }

    public class DtoSubject
    {
        public int SubjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Name of Qualification is required.")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Subject / Stream / Branch is required.")]
        [StringLength(100, ErrorMessage = "Subject / Stream / Branch cannot exceed 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class SubjectMasterModel
    {
        public int SubjectId { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int QualificationId { get; set; }
        public string? Qualification { get; set; }
        public string Subject { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class DtoSubjectList
    {
        public int? QualificationId { get; set; }
        public int? CourseId { get; set; }
        public string? Search { get; set; }
    }

    public class DtoSubjectStatusToggle
    {
        [Range(1, int.MaxValue, ErrorMessage = "SubjectId is required.")]
        public int SubjectId { get; set; }
        public bool IsActive { get; set; }
        public long? UpdatedBy { get; set; }
    }

    public class DtoGetSubjectById
    {
        [Range(1, int.MaxValue, ErrorMessage = "SubjectId is required.")]
        public int SubjectId { get; set; }
    }
    #endregion
}

