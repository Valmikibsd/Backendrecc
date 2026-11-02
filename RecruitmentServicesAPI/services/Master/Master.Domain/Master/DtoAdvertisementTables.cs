using System;
using System.ComponentModel.DataAnnotations;

namespace Master.Domain.Models
{
    public class DtoCloneAdvertisementTables
    {
        [Range(1, int.MaxValue, ErrorMessage = "Source template is required.")]
        public int SourceSrNo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Advertisement is required.")]
        public int AdvertisementId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Post is required.")]
        public int PostId { get; set; }
    }

    public class DtoToggleAdvertisementTableStatus
    {
        [Range(1, int.MaxValue, ErrorMessage = "SrNo is required.")]
        public int SrNo { get; set; }

        public bool IsActive { get; set; }
    }

    public class DtoSourceAdvertisementTableDropdown
    {
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public int AdvertisementId { get; set; }
        public string AdvertisementNo { get; set; } = string.Empty;
        public string AdvertisementName { get; set; } = string.Empty;
        public int PostId { get; set; }
        public string PostName { get; set; } = string.Empty;
        public string PersonalDetailsTable { get; set; } = string.Empty;
        public string? PersonalDetailsHistoryTable { get; set; }
        public string? AddressDetailsTable { get; set; }
        public string? AddressDetailsHistoryTable { get; set; }
        public string? EducationDetailsTable { get; set; }
        public string? EducationDetailsHistoryTable { get; set; }
        public string? ImageDetailsTable { get; set; }
        public string? ImageDetailsHistoryTable { get; set; }
        public string? OtherDetailsTable { get; set; }
        public string? OtherDetailsHistoryTable { get; set; }
        public string? HallticketTable { get; set; }
        public int? ClonedFromSrNo { get; set; }
    }

    public class DtoAdvertisementTablesList
    {
        public string? SearchText { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Page Index must be 0 or greater.")]
        public int PageIndex { get; set; } = 0;

        [Range(1, 100, ErrorMessage = "Page Size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }

    public class AdvertisementTablesModel
    {
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public int AdvertisementId { get; set; }
        public string AdvertisementNo { get; set; } = string.Empty;
        public string AdvertisementName { get; set; } = string.Empty;
        public int PostId { get; set; }
        public string PostName { get; set; } = string.Empty;
        public string PersonalDetailsTable { get; set; } = string.Empty;
        public string AddressDetailsTable { get; set; } = string.Empty;
        public string EducationDetailsTable { get; set; } = string.Empty;
        public string ImageDetailsTable { get; set; } = string.Empty;
        public string OtherDetailsTable { get; set; } = string.Empty;
        public string HallticketTable { get; set; } = string.Empty;
        public string PersonalDetailsHistoryTable { get; set; } = string.Empty;
        public string AddressDetailsHistoryTable { get; set; } = string.Empty;
        public string EducationDetailsHistoryTable { get; set; } = string.Empty;
        public string ImageDetailsHistoryTable { get; set; } = string.Empty;
        public string OtherDetailsHistoryTable { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public int? ClonedFromSrNo { get; set; }
        public int TotalCount { get; set; }
    }
}
