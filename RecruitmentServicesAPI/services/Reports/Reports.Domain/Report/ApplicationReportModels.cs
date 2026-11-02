using System.ComponentModel.DataAnnotations;

namespace Reports.Domain.Report;

public class DropdownItemModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameMr { get; set; }
}

public class DtoReportFilterRequest
{
    public int DepartmentId { get; set; }
    public int AdvertisementId { get; set; }
    public int PostId { get; set; }
    
    [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters.")]
    public string? SearchText { get; set; }
}

public class DtoApplicationReportSearchRequest
{
    public int DepartmentId { get; set; }
    public int AdvertisementId { get; set; }
    public int PostId { get; set; }

    [StringLength(100, ErrorMessage = "Application number cannot exceed 100 characters.")]
    public string? ApplicationNo { get; set; }

    [StringLength(100, ErrorMessage = "Search text cannot exceed 100 characters.")]
    public string? SearchText { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(0, 500000, ErrorMessage = "PageSize must be between 0 and 500000.")]
    public int PageSize { get; set; } = 10;
}

public class ApplicationReportItemModel
{
    public long SrNo { get; set; }
    public long UserId { get; set; }
    public string ApplicationNo { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int AdvertisementId { get; set; }
    public string AdvertisementName { get; set; } = string.Empty;
    public int PostId { get; set; }
    public string PostName { get; set; } = string.Empty;
    public string FullNameEnglish { get; set; } = string.Empty;
    public string FullNameMarathi { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string EmailId { get; set; } = string.Empty;
    public int PayStatus { get; set; }
    public string PayStatusText { get; set; } = string.Empty;
    public DateTime? AppliedDate { get; set; }
    public int TotalRecords { get; set; }
}
