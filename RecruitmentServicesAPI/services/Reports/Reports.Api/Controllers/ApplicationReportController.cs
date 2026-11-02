using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reports.Application.Interfaces;
using Reports.Domain.Report;
using Shared.Auth;
using Shared.Kernel;

namespace Reports.Api.Controllers;

[ApiController]
[Route("api/reports/application")]
[Authorize(Policy = AuthPolicies.ReportAccess)]
public class ApplicationReportController(IApplicationReportRepository repository) : BaseController
{
    private int NumericRoleId => int.TryParse(UserRoleId, out var roleId) ? roleId : 0;

    [HttpPost("getdepartments")]
    public async Task<IActionResult> GetDepartmentList()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await repository.GetDepartmentListAsync(NumericRoleId, UserDepartmentId);
        return response.Status
            ? Ok(response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost("getadvertisements")]
    public async Task<IActionResult> GetAdvertisementList([FromBody] DtoReportFilterRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await repository.GetAdvertisementListAsync(NumericRoleId, UserDepartmentId, request.DepartmentId);
        return response.Status
            ? Ok(response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost("getposts")]
    public async Task<IActionResult> GetPostList([FromBody] DtoReportFilterRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await repository.GetPostListAsync(NumericRoleId, UserDepartmentId, UserUnitId, request.DepartmentId, request.AdvertisementId);
        return response.Status
            ? Ok(response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost("getapplicationreport")]
    public async Task<IActionResult> GetApplicationReportList([FromBody] DtoApplicationReportSearchRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await repository.GetApplicationReportListAsync(NumericRoleId, UserDepartmentId, request);
        return response.Status
            ? Ok(response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost("exportexcel")]
    public async Task<IActionResult> ExportApplicationReportExcel([FromBody] DtoApplicationReportSearchRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var fileBytes = await repository.ExportApplicationReportExcelAsync(NumericRoleId, UserDepartmentId, request);
        var fileName = $"Application_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
