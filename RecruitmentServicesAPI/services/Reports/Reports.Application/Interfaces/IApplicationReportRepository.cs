using CrudOperations;
using Shared.Kernel;
using Reports.Domain.Report;

namespace Reports.Application.Interfaces;

public interface IApplicationReportRepository
{
    Task<ResponseList<DropdownItemModel>> GetDepartmentListAsync(int roleId, int userDeptId);
    Task<ResponseList<DropdownItemModel>> GetAdvertisementListAsync(int roleId, int userDeptId, int departmentId);
    Task<ResponseList<DropdownItemModel>> GetPostListAsync(int roleId, int userDeptId, string? unitId, int departmentId, int advertisementId);
    Task<ResponseList<ApplicationReportItemModel>> GetApplicationReportListAsync(int roleId, int userDeptId, DtoApplicationReportSearchRequest request);
    Task<byte[]> ExportApplicationReportExcelAsync(int roleId, int userDeptId, DtoApplicationReportSearchRequest request);
}
