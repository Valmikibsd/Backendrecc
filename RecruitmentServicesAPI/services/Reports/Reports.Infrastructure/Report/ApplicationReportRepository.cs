using CrudOperations;
using Reports.Application.Interfaces;
using Reports.Domain.Report;
using Reports.Infrastructure.Helpers;
using Shared.Kernel;

namespace Reports.Infrastructure.Report;

public class ApplicationReportRepository(ICrudOperationService crudOperationService) : IApplicationReportRepository
{
    public async Task<ResponseList<DropdownItemModel>> GetDepartmentListAsync(int roleId, int userDeptId)
    {
        return await crudOperationService.GetList<DropdownItemModel>(
            "[dbo].[Usp_GetApplicationReportDepartments]",
            new
            {
                RoleId = roleId,
                UserDepartmentId = userDeptId
            },
            ConnectionNames.Applications);
    }

    public async Task<ResponseList<DropdownItemModel>> GetAdvertisementListAsync(int roleId, int userDeptId, int departmentId)
    {
        return await crudOperationService.GetList<DropdownItemModel>(
            "[dbo].[Usp_GetApplicationReportAdvertisements]",
            new
            {
                RoleId = roleId,
                UserDepartmentId = userDeptId,
                DepartmentId = departmentId
            },
            ConnectionNames.Applications);
    }

    public async Task<ResponseList<DropdownItemModel>> GetPostListAsync(int roleId, int userDeptId, string? unitId, int departmentId, int advertisementId)
    {
        return await crudOperationService.GetList<DropdownItemModel>(
            "[dbo].[Usp_GetApplicationReportPosts]",
            new
            {
                RoleId = roleId,
                UserDepartmentId = userDeptId,
                UnitId = string.IsNullOrWhiteSpace(unitId) ? null : unitId.Trim(),
                DepartmentId = departmentId,
                AdvertisementId = advertisementId
            },
            ConnectionNames.Applications);
    }

    public async Task<ResponseList<ApplicationReportItemModel>> GetApplicationReportListAsync(int roleId, int userDeptId, DtoApplicationReportSearchRequest request)
    {
        int effectiveDeptId = userDeptId > 0 ? userDeptId : request.DepartmentId;
        bool hasAllFilters = effectiveDeptId > 0 && request.AdvertisementId > 0 && request.PostId > 0 && !string.IsNullOrWhiteSpace(request.ApplicationNo);

        if (!hasAllFilters)
        {
            return new ResponseList<ApplicationReportItemModel>
            {
                Status = false,
                Message = "Please select all 4 filters (Department, Advertisement, Post, and Application No).",
                Data = new List<ApplicationReportItemModel>()
            };
        }

        return await crudOperationService.GetList<ApplicationReportItemModel>(
            "[dbo].[Usp_GetApplicationReportData]",
            new
            {
                RoleId = roleId,
                UserDepartmentId = userDeptId,
                DepartmentId = request.DepartmentId,
                AdvertisementId = request.AdvertisementId,
                PostId = request.PostId,
                ApplicationNo = request.ApplicationNo?.Trim() ?? string.Empty
            },
            ConnectionNames.Applications);
    }

    public async Task<byte[]> ExportApplicationReportExcelAsync(int roleId, int userDeptId, DtoApplicationReportSearchRequest request)
    {
        var exportRequest = new DtoApplicationReportSearchRequest
        {
            DepartmentId = request.DepartmentId,
            AdvertisementId = request.AdvertisementId,
            PostId = request.PostId,
            ApplicationNo = request.ApplicationNo,
            Page = 1,
            PageSize = request.PageSize > 0 ? request.PageSize : 50000
        };

        var response = await GetApplicationReportListAsync(roleId, userDeptId, exportRequest);
        var dataList = response?.Data ?? new List<ApplicationReportItemModel>();

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Application Report");

        var headers = new string[]
        {
            "Sr. No.",
            "Application No",
            "Candidate Name (English)",
            "Candidate Name (Marathi)",
            "Department",
            "Advertisement",
            "Post",
            "Applied Date",
            "Payment Status"
        };

        ExcelReportHelper.ApplyHeaderStyle(worksheet, headers);

        int currentRow = 2;
        foreach (var item in dataList)
        {
            worksheet.Cell(currentRow, 1).Value = item.SrNo > 0 ? item.SrNo : (currentRow - 1);
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            var appNoCell = worksheet.Cell(currentRow, 2);
            appNoCell.Value = item.ApplicationNo ?? string.Empty;
            appNoCell.Style.NumberFormat.Format = "@";

            worksheet.Cell(currentRow, 3).Value = item.FullNameEnglish ?? string.Empty;
            worksheet.Cell(currentRow, 4).Value = item.FullNameMarathi ?? string.Empty;
            worksheet.Cell(currentRow, 5).Value = item.DepartmentName ?? string.Empty;
            worksheet.Cell(currentRow, 6).Value = item.AdvertisementName ?? string.Empty;
            worksheet.Cell(currentRow, 7).Value = item.PostName ?? string.Empty;

            var dateCell = worksheet.Cell(currentRow, 8);
            dateCell.Value = item.AppliedDate.HasValue ? item.AppliedDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            dateCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

            var payCell = worksheet.Cell(currentRow, 9);
            payCell.Value = item.PayStatusText ?? (item.PayStatus == 1 ? "Paid" : "Pending");
            payCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
            payCell.Style.Font.Bold = true;
            payCell.Style.Font.FontColor = item.PayStatus == 1
                ? ClosedXML.Excel.XLColor.FromHtml("#166534")
                : ClosedXML.Excel.XLColor.FromHtml("#854D0E");

            ExcelReportHelper.ApplyRowBorders(worksheet, currentRow, 9);
            currentRow++;
        }

        worksheet.Columns().AdjustToContents();
        return ExcelReportHelper.ToByteArray(workbook);
    }
}
