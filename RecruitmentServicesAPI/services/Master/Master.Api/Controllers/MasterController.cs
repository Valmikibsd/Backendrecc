using ClosedXML.Excel;
using CrudOperations;
using DocumentFormat.OpenXml.Spreadsheet;
using Master.Api.Models;
using Master.Application.Interfaces;
using Master.Domain.Master;
using Master.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using Shared.Kernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace Master.Api.Controllers;

[ApiController]
[Route("api/master")]
public class MasterController(IMasterRepository masterRepository) : BaseController
{
    private readonly IMasterRepository _master = masterRepository;

    [HttpGet, Route("GetDepartmentDetails")]
    public async Task<IActionResult> GetDepartmentDetails()
    {
        var response = await _master.GetDepartmentDetails().ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost, Route("GetDepartmentAdvertisements")]
    public async Task<IActionResult> GetDepartmentAdvertisements([FromBody] DepartmentRequest request)
    {
        var response = await _master.GetDepartmentAdvertisements(request.DepartmentId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost, Route("GetDepartmentPosts")]
    public async Task<IActionResult> GetDepartmentPosts([FromBody] DepartmentPostRequest request)
    {
        var response = await _master.GetDepartmentPosts(request.DepartmentId, request.AdvertisementId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveAdvertisementPost")]
    public async Task<IActionResult> SaveAdvertisementPost([FromBody] DtoAdvertisementPost dtoAdvertisementPost)
    {
        var invalid = InvalidAdvertisementPost(dtoAdvertisementPost);
        if (invalid != null)
            return invalid;

        dtoAdvertisementPost.Qualification = dtoAdvertisementPost.Qualification?.Trim();
        dtoAdvertisementPost.PayServiceId = dtoAdvertisementPost.PayServiceId?.Trim();
        dtoAdvertisementPost.CreatedBy = UserId;
        var response = await _master.SaveAdvertisementPost(dtoAdvertisementPost).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateAdvertisementPost")]
    public async Task<IActionResult> UpdateAdvertisementPost([FromBody] DtoAdvertisementPost dtoAdvertisementPost)
    {
        var invalid = InvalidAdvertisementPost(dtoAdvertisementPost);
        if (invalid != null)
            return invalid;

        dtoAdvertisementPost.Qualification = dtoAdvertisementPost.Qualification?.Trim();
        dtoAdvertisementPost.PayServiceId = dtoAdvertisementPost.PayServiceId?.Trim();
        dtoAdvertisementPost.UpdatedBy = UserId;
        var response = await _master.UpdateAdvertisementPost(dtoAdvertisementPost);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementPostList")]
    public async Task<IActionResult> GetAdvertisementPostList([FromBody] DtoAdvertisementPostList request)
    {
        var response = await _master.GetAdvertisementPostList(request).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("GetDepartmentPosts")]
    public async Task<IActionResult> GetDepartmentPosts()
    {
        var response = await _master.GetDepartmentPosts();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementPostById")]
    public async Task<IActionResult> GetAdvertisementPostById([FromBody] DtoGetAdvertisementPostById request)
    {
        var response = await _master.GetAdvertisementPostById(request.AdvertisementPostId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize]
    [HttpGet, Route("getmenu")]
    public async Task<IActionResult> GetMenuByRole()
    {
        var roleClaim = User?.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleClaim) || !int.TryParse(roleClaim, out var roleId))
        {
            return Unauthorized(new { Status = false, Message = "Invalid role in token." });
        }

        var response = await _master.GetMenuByRoleId(roleId).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize]
    [HttpGet, Route("getrolelist")]
    public async Task<IActionResult> GetRoleList()
    {
        var response = await _master.GetRoleList().ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.ApplicantOnly)]
    [HttpPost, Route("GetDepartmentCandidatePostDetails")]
    public async Task<IActionResult> GetDepartmentCandidatePostDetails([FromBody] DepartmentCandidatePostRequest request)
    {
        var response = await _master.GetDepartmentCandidatePostDetails(
            UserId, request.DepartmentId, request.AdvertisementId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveDepartment")]
    public async Task<IActionResult> SaveDepartment([FromBody] DtoDepartment dtoDepartment)
    {
        dtoDepartment.DepartmentCode = dtoDepartment.DepartmentCode?.Trim();
        dtoDepartment.DepartmentName = dtoDepartment.DepartmentName?.Trim();
        dtoDepartment.DepartmentName_Mr = dtoDepartment.DepartmentName_Mr?.Trim();
        dtoDepartment.DepartmentType = dtoDepartment.DepartmentType?.Trim();
        dtoDepartment.Description = dtoDepartment.Description?.Trim();
        dtoDepartment.CreatedBy = UserId;
        var response = await _master.SaveDepartment(dtoDepartment).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateDepartment")]
    public async Task<IActionResult> UpdateDepartment([FromBody] DtoDepartment dtoDepartment)
    {
        dtoDepartment.DepartmentCode = dtoDepartment.DepartmentCode?.Trim();
        dtoDepartment.DepartmentName = dtoDepartment.DepartmentName?.Trim();
        dtoDepartment.DepartmentName_Mr = dtoDepartment.DepartmentName_Mr?.Trim();
        dtoDepartment.DepartmentType = dtoDepartment.DepartmentType?.Trim();
        dtoDepartment.Description = dtoDepartment.Description?.Trim();
        dtoDepartment.UpdatedBy = UserId;
        var response = await _master.UpdateDepartment(dtoDepartment).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetDepartmentById")]
    public async Task<IActionResult> GetDepartmentById([FromBody] DtoGetDepartmentById request)
    {
        var response = await _master.GetDepartmentById(request.DepartmentId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetDepartmentList")]
    public async Task<IActionResult> GetDepartmentList([FromBody] DtoDepartmentList request)
    {
        var response = await _master.GetDepartmentList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpGet, Route("GetActiveDepartmentList")]
    public async Task<IActionResult> GetActiveDepartmentList()
    {
        var response = await _master.GetActiveDepartmentList();
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SavePost")]
    public async Task<IActionResult> SavePost([FromBody] DtoPost dtoPost)
    {
        dtoPost.PostName = dtoPost.PostName?.Trim();
        dtoPost.PostNameMarathi = dtoPost.PostNameMarathi?.Trim();
        dtoPost.CreatedBy = UserId;
        var response = await _master.SavePost(dtoPost).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdatePost")]
    public async Task<IActionResult> UpdatePost([FromBody] DtoPost dtoPost)
    {
        dtoPost.PostName = dtoPost.PostName?.Trim();
        dtoPost.PostNameMarathi = dtoPost.PostNameMarathi?.Trim();
        dtoPost.UpdatedBy = UserId;
        var response = await _master.UpdatePost(dtoPost).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetPostById")]
    public async Task<IActionResult> GetPostById([FromBody] DtoGetPostById request)
    {
        var response = await _master.GetPostById(request.PostId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetPostList")]
    public async Task<IActionResult> GetPostList([FromBody] DtoPostList request)
    {
        var response = await _master.GetPostList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveHelpDetails")]
    public async Task<IActionResult> SaveHelpDetails([FromBody] DtoHelpDetails dtoHelpDetails)
    {
        var invalidUser = InvalidUserResponse();

        if (invalidUser != null)
        {
            return invalidUser;
        }

        dtoHelpDetails.DepartmentContactNo = dtoHelpDetails.DepartmentContactNo?.Trim();
        dtoHelpDetails.DepartmentEmailId = dtoHelpDetails.DepartmentEmailId?.Trim();
        dtoHelpDetails.MahaITContactNo = dtoHelpDetails.MahaITContactNo?.Trim();
        dtoHelpDetails.MahaITEmailId = dtoHelpDetails.MahaITEmailId?.Trim();
        dtoHelpDetails.HelpDescription = dtoHelpDetails.HelpDescription?.Trim();
        dtoHelpDetails.UserId = UserId;

        var response = await _master.SaveHelpDetails(dtoHelpDetails).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("GetHelpDetailsById/{helpDetailsId}")]
    public async Task<IActionResult> GetHelpDetailsById(int helpDetailsId)
    {
        var response = await _master.GetHelpDetailsById(helpDetailsId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("GetHelpDetailsList")]
    public async Task<IActionResult> GetHelpDetailsList()
    {
        var response = await _master.GetHelpDetailsList();
        return response.Status ? Ok(response) : Conflict(response);
    }

    [AllowAnonymous]
    [HttpGet, Route("GetHelpDetailsByDepartment/{departmentId}")]
    public async Task<IActionResult> GetHelpDetailsByDepartment(int departmentId)
    {
        if (departmentId <= 0)
        {
            return BadRequest();
        }

        var response = await _master.GetHelpDetailsByDepartment(departmentId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    #region AdvertisementPdf
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveAdvertisementPdf")]
    public async Task<IActionResult> SaveAdvertisementPdf([FromForm] AdvertisementPdfRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        request.AdvertisementPdfId = null;
        var response = await _master.SaveAdvertisementPdf(await BuildAdvertisementPdfDto(request)).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateAdvertisementPdf")]
    public async Task<IActionResult> UpdateAdvertisementPdf([FromForm] AdvertisementPdfRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        if (!request.AdvertisementPdfId.HasValue || request.AdvertisementPdfId.Value <= 0)
        {
            return BadRequest(new { Status = false, Message = "Advertisement PDF Id is required." });
        }

        var response = await _master.SaveAdvertisementPdf(await BuildAdvertisementPdfDto(request)).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementPdfById")]
    public async Task<IActionResult> GetAdvertisementPdfById([FromBody] DtoGetAdvertisementPdfById request)
    {
        var response = await _master.GetAdvertisementPdfById(request.AdvertisementPdfId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementPdfList")]
    public async Task<IActionResult> GetAdvertisementPdfList([FromBody] DtoAdvertisementPdfList request)
    {
        var response = await _master.GetAdvertisementPdfList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("DownloadAdvertisementPdf")]
    public async Task<IActionResult> DownloadAdvertisementPdf([FromBody] DtoGetAdvertisementPdfById request)
    {
        var response = await _master.GetAdvertisementPdfFile(request.AdvertisementPdfId);
        if (!response.Status || response.Data == null)
        {
            return NotFound(new { Status = false, Message = "Advertisement PDF not found." });
        }

        return File(response.Data.FileData, response.Data.FileType, response.Data.FileName);
    }

    [AllowAnonymous]
    [HttpPost, Route("GetPublicAdvertisementPdfList")]
    public async Task<IActionResult> GetPublicAdvertisementPdfList([FromBody] DtoPublicAdvertisementPdfList request)
    {
        var response = await _master.GetPublicAdvertisementPdfList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [AllowAnonymous]
    [HttpPost, Route("DownloadPublicAdvertisementPdf")]
    public async Task<IActionResult> DownloadPublicAdvertisementPdf([FromBody] DtoGetAdvertisementPdfById request)
    {
        var response = await _master.GetPublicAdvertisementPdfFile(request.AdvertisementPdfId);
        if (!response.Status || response.Data == null)
        {
            return NotFound(new { Status = false, Message = "Advertisement PDF not found." });
        }

        return File(response.Data.FileData, response.Data.FileType, response.Data.FileName);
    }

    private async Task<DtoAdvertisementPdf> BuildAdvertisementPdfDto(AdvertisementPdfRequest request)
    {
        var dto = new DtoAdvertisementPdf
        {
            AdvertisementPdfId = request.AdvertisementPdfId,
            DepartmentId = request.DepartmentId,
            AdvertisementId = request.AdvertisementId,
            PostId = request.PostId,
            UnitId = string.IsNullOrWhiteSpace(request.UnitId) ? null : request.UnitId.Trim(),
            Description = request.Description?.Trim(),
            IsActive = request.IsActive,
            UserId = UserId
        };

        if (request.File != null && request.File.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            dto.FileData = memoryStream.ToArray();
            dto.FileName = Path.GetFileName(request.File.FileName);
            dto.FileType = request.File.ContentType;
        }

        return dto;
    }
    #endregion

    #region RulePdf
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveRulePdf")]
    public async Task<IActionResult> SaveRulePdf([FromForm] RulePdfRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        request.RuleId = null;
        var response = await _master.SaveRulePdf(await BuildRulePdfDto(request)).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateRulePdf")]
    public async Task<IActionResult> UpdateRulePdf([FromForm] RulePdfRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        if (!request.RuleId.HasValue || request.RuleId.Value <= 0)
        {
            return BadRequest(new { Status = false, Message = "Rule PDF Id is required." });
        }

        var response = await _master.SaveRulePdf(await BuildRulePdfDto(request)).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetRulePdfById")]
    public async Task<IActionResult> GetRulePdfById([FromBody] DtoGetRulePdfById request)
    {
        var response = await _master.GetRulePdfById(request.RuleId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetRulePdfList")]
    public async Task<IActionResult> GetRulePdfList([FromBody] DtoRulePdfList request)
    {
        var response = await _master.GetRulePdfList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("DownloadRulePdf")]
    public async Task<IActionResult> DownloadRulePdf([FromBody] DtoGetRulePdfById request)
    {
        var response = await _master.GetRulePdfFile(request.RuleId);
        if (!response.Status || response.Data == null)
        {
            return NotFound(new { Status = false, Message = "Rule PDF not found." });
        }

        return File(response.Data.FileData, response.Data.FileType, response.Data.FileName);
    }

    [AllowAnonymous]
    [HttpPost, Route("GetPublicRulePdfList")]
    public async Task<IActionResult> GetPublicRulePdfList([FromBody] DtoPublicRulePdfList request)
    {
        var response = await _master.GetPublicRulePdfList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [AllowAnonymous]
    [HttpPost, Route("DownloadPublicRulePdf")]
    public async Task<IActionResult> DownloadPublicRulePdf([FromBody] DtoGetRulePdfById request)
    {
        var response = await _master.GetPublicRulePdfFile(request.RuleId);
        if (!response.Status || response.Data == null)
        {
            return NotFound(new { Status = false, Message = "Rule PDF not found." });
        }

        return File(response.Data.FileData, response.Data.FileType, response.Data.FileName);
    }

    private async Task<DtoRulePdf> BuildRulePdfDto(RulePdfRequest request)
    {
        var dto = new DtoRulePdf
        {
            RuleId = request.RuleId,
            DepartmentId = request.DepartmentId,
            AdvertisementId = request.AdvertisementId,
            PostId = request.PostId,
            Description = request.Description?.Trim(),
            IsActive = request.IsActive,
            UserId = UserId
        };

        if (request.File != null && request.File.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            dto.FileData = memoryStream.ToArray();
            dto.FileName = Path.GetFileName(request.File.FileName);
            dto.FileType = request.File.ContentType;
        }

        return dto;
    }
    #endregion

    #region AgeCriteria
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveAgeCriteria")]
    public async Task<IActionResult> SaveAgeCriteria([FromBody] DtoAgeCriteria dtoAgeCriteria)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        var invalid = InvalidAgeCriteria(dtoAgeCriteria);
        if (invalid != null)
            return invalid;

        dtoAgeCriteria.CreatedBy = UserId;
        var response = await _master.SaveAgeCriteria(dtoAgeCriteria).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateAgeCriteria")]
    public async Task<IActionResult> UpdateAgeCriteria([FromBody] DtoAgeCriteria dtoAgeCriteria)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        if (!dtoAgeCriteria.AgeCriteriaId.HasValue || dtoAgeCriteria.AgeCriteriaId.Value <= 0)
        {
            return BadRequest(new { Status = false, Message = "Age Criteria Id is required." });
        }

        var invalid = InvalidAgeCriteria(dtoAgeCriteria);
        if (invalid != null)
            return invalid;

        dtoAgeCriteria.UpdatedBy = UserId;
        var response = await _master.UpdateAgeCriteria(dtoAgeCriteria).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAgeCriteriaById")]
    public async Task<IActionResult> GetAgeCriteriaById([FromBody] DtoGetAgeCriteriaById request)
    {
        var response = await _master.GetAgeCriteriaById(request.AgeCriteriaId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAgeCriteriaList")]
    public async Task<IActionResult> GetAgeCriteriaList([FromBody] DtoAgeCriteriaList request)
    {
        var response = await _master.GetAgeCriteriaList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("DownloadAgeCriteriaTemplate")]
    public async Task<IActionResult> DownloadAgeCriteriaTemplate()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        var fileBytes = await _master.GetAgeCriteriaTemplate();
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "AgeCriteriaTemplate.xlsx");
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("ImportAgeCriteria")]
    public async Task<IActionResult> ImportAgeCriteria(IFormFile file)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
            return invalidUser;

        if (file == null || file.Length == 0)
            return BadRequest(new { Status = false, Message = "Please select a file to upload." });

        var departments = (await _master.GetActiveDepartmentList()).Data ?? new List<DepartmentModel>();
        var advertisements = (await _master.GetAdvertisementList(new DtoAdvertisementList { Page = 1, PageSize = 5000 })).Data ?? new List<AdvertisementMasterModel>();
        var posts = (await _master.GetDepartmentPosts()).Data ?? new List<PostMasterModel>();
        var genders = (await _master.GetGender()).Data ?? new List<DDlList>();
        var categories = (await _master.GetCategory(null)).Data ?? new List<CategoryMasterModel>();
        var parallelReservations = (await _master.GetParallelReservation()).Data ?? new List<DDlList>();

        var departmentMap = departments.ToDictionary(d => (d.DepartmentName ?? string.Empty).Trim().ToLowerInvariant(), d => d.DepartmentId);
        var postMap = posts.ToDictionary(p => p.PostName.Trim().ToLowerInvariant(), p => p.PostId);
        var genderMap = genders.Where(g => g.Name != null).ToDictionary(g => g.Name!.Trim().ToLowerInvariant(), g => g.Id);
        var categoryMap = categories.ToDictionary(c => c.Category.Trim().ToLowerInvariant(), c => c.CategoryId);
        var parallelReservationMap = parallelReservations.Where(p => p.Name != null).ToDictionary(p => p.Name!.Trim().ToLowerInvariant(), p => p.Id);
        var advertisementMap = advertisements
            .Where(a => a.AdvertisementName != null)
            .ToDictionary(a => (a.DepartmentId, a.AdvertisementName!.Trim().ToLowerInvariant()), a => a.AdvertisementId);

        var table = new DataTable();
        table.Columns.Add("RowNumber", typeof(int));
        table.Columns.Add("DepartmentId", typeof(int));
        table.Columns.Add("AdvertisementId", typeof(int));
        table.Columns.Add("PostId", typeof(int));
        table.Columns.Add("GenderId", typeof(int));
        table.Columns.Add("CategoryId", typeof(int));
        table.Columns.Add("ParallelReservationId", typeof(int));
        table.Columns.Add("MinimumAge", typeof(int));
        table.Columns.Add("MaximumAge", typeof(int));
        table.Columns.Add("IsActive", typeof(bool));

        var parseErrors = new List<AgeCriteriaImportErrorModel>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheet(1);
            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;

            for (var rowIndex = 2; rowIndex <= lastRow; rowIndex++)
            {
                var row = sheet.Row(rowIndex);
                if (row.IsEmpty())
                    continue;

                var departmentName = row.Cell(1).GetString().Trim();
                var advertisementName = row.Cell(2).GetString().Trim();
                var postName = row.Cell(3).GetString().Trim();
                var genderName = row.Cell(4).GetString().Trim();
                var categoryName = row.Cell(5).GetString().Trim();
                var parallelReservationName = row.Cell(6).GetString().Trim();
                var minimumAgeText = row.Cell(7).GetString().Trim();
                var maximumAgeText = row.Cell(8).GetString().Trim();
                var isActiveText = row.Cell(9).GetString().Trim();

                if (!departmentMap.TryGetValue(departmentName.ToLowerInvariant(), out var departmentId))
                {
                    parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = $"Department '{departmentName}' not found." });
                    continue;
                }

                if (!advertisementMap.TryGetValue((departmentId, advertisementName.ToLowerInvariant()), out var advertisementId))
                {
                    parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = $"Advertisement '{advertisementName}' not found for Department '{departmentName}'." });
                    continue;
                }

                if (!postMap.TryGetValue(postName.ToLowerInvariant(), out var postId))
                {
                    parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = $"Post '{postName}' not found." });
                    continue;
                }

                if (!genderMap.TryGetValue(genderName.ToLowerInvariant(), out var genderId))
                {
                    parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = $"Gender '{genderName}' not found." });
                    continue;
                }

                if (!categoryMap.TryGetValue(categoryName.ToLowerInvariant(), out var categoryId))
                {
                    parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = $"Category '{categoryName}' not found." });
                    continue;
                }

                int? parallelReservationId = null;
                if (!string.IsNullOrWhiteSpace(parallelReservationName))
                {
                    if (!parallelReservationMap.TryGetValue(parallelReservationName.ToLowerInvariant(), out var prId))
                    {
                        parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = $"Parallel Reservation '{parallelReservationName}' not found." });
                        continue;
                    }
                    parallelReservationId = prId;
                }

                if (!int.TryParse(minimumAgeText, out var minimumAge) || !int.TryParse(maximumAgeText, out var maximumAge))
                {
                    parseErrors.Add(new AgeCriteriaImportErrorModel { RowNumber = rowIndex, Reason = "Minimum Age / Maximum Age must be numeric." });
                    continue;
                }

                var isActive = !string.Equals(isActiveText, "FALSE", StringComparison.OrdinalIgnoreCase);

                table.Rows.Add(rowIndex, departmentId, advertisementId, postId, genderId, categoryId, (object?)parallelReservationId ?? DBNull.Value, minimumAge, maximumAge, isActive);
            }
        }

        if (table.Rows.Count == 0)
        {
            return Ok(new ResponseList<AgeCriteriaImportErrorModel>
            {
                Status =false,
                Message = parseErrors.Count == 0 ? "No data rows found in the file." : "All rows failed validation.",
                Data = parseErrors,
                TotalRecords = parseErrors.Count
            });
        }

        var response = await _master.ImportAgeCriteria(table, UserId);
        response.Data = parseErrors.Concat(response.Data ?? new List<AgeCriteriaImportErrorModel>()).OrderBy(e => e.RowNumber).ToList();
        response.TotalRecords += parseErrors.Count;

        //return response.Status ? Ok(response) : Conflict(response);
        return Ok(response);
    }
    #endregion
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveNotification")]
    public async Task<IActionResult> SaveNotification([FromBody] DtoNotification dtoNotification)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        if (dtoNotification.StartDate.HasValue &&
            dtoNotification.EndDate.HasValue &&
            dtoNotification.EndDate.Value.Date < dtoNotification.StartDate.Value.Date)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "End Date cannot be before Start Date."
            });
        }

        dtoNotification.NotificationDetails = dtoNotification.NotificationDetails?.Trim();
        dtoNotification.CreatedBy = UserId;
        dtoNotification.UpdatedBy = UserId;
        var response = await _master.SaveNotification(dtoNotification);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetNotificationList")]
    public async Task<IActionResult> GetNotificationList([FromBody] DtoNotificationList request)
    {
        var response = await _master.GetNotificationList(request);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetNotificationDetails")]
    public async Task<IActionResult> GetNotificationDetails([FromBody] DtoGetNotificationDetails request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetNotificationDetails(request.SrNo, request.DepartmentId);
        return response.Status ? Ok(response) : Conflict(response);
    }
    [HttpPost, Route("GetNotification")]
    public async Task<IActionResult> GetNotification([FromBody] DtoGetNotification request)
    {
        var response = await _master.GetNotification(request.DepartmentId);
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpGet, Route("GetLatestNews")]
    public async Task<IActionResult> GetLatestNews()
    {
        var response = await _master.GetLatestNews();
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpGet, Route("GetHomeCountDetails")]
    public async Task<IActionResult> GetHomeCountDetails()
    {
        var response = await _master.GetHomeCountDetails();
        return response.Status ? Ok(response) : Conflict(response);
    }

    [HttpPost, Route("GetCategory")]
    public async Task<IActionResult> GetCategory([FromBody] DtoGetCategory request)
    {
        var response = await _master.GetCategory(request?.CategoryId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveFeesDetails")]
    public async Task<IActionResult> SaveFeesDetails([FromBody] List<DtoFeesMasterDetailsPost> dtoFeesMasterDetailsPost)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var invalid = InvalidFeesDetails(dtoFeesMasterDetailsPost);
        if (invalid != null)
            return invalid;

        foreach (var item in dtoFeesMasterDetailsPost)
        {
            item.CreatedBy = UserId;
        }

        var response = await _master.SaveFeesDetails(dtoFeesMasterDetailsPost).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetFeesMasterDetailsList")]
    public async Task<IActionResult> GetFeesMasterDetailsList([FromBody] DtoFeesMasterList request)
    {
        if (request.Page <= 0)
            request.Page = 1;
        if (request.PageSize <= 0)
            request.PageSize = 10;

        var response = await _master.GetFeesMasterDetailsList(request).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateFeesMasterDetails")]
    public async Task<IActionResult> UpdateFeesMasterDetails([FromBody] DtoFeesMasterDetailsPost dtoFeesMasterDetailsPost)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        if (dtoFeesMasterDetailsPost.FeesCriteriaId <= 0 || (dtoFeesMasterDetailsPost.Amount <= 0 && dtoFeesMasterDetailsPost.IsActive == null))
        {
            return BadRequest(new { Status = false, Message = "Invalid fees details." });
        }

        dtoFeesMasterDetailsPost.CreatedBy = UserId;
        var response = await _master.UpdateFeesMasterDetails(dtoFeesMasterDetailsPost).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveQualificationCriteria")]
    public async Task<IActionResult> SaveQualificationCriteria([FromBody] DtoQualificationCriteriaPost request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        request.CreatedBy = UserId;
        var response = await _master.SaveQualificationCriteria(request).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetQualificationCriteriaList")]
    public async Task<IActionResult> GetQualificationCriteriaList([FromBody] DtoQualificationCriteriaList request)
    {
        if (request.Page <= 0)
            request.Page = 1;
        if (request.PageSize <= 0)
            request.PageSize = 10;

        var response = await _master.GetQualificationCriteriaList(request).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateQualificationCriteria")]
    public async Task<IActionResult> UpdateQualificationCriteria([FromBody] DtoQualificationCriteriaPost request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        request.UpdatedBy = UserId;
        var response = await _master.UpdateQualificationCriteria(request).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetQualificationCriteriaByPost")]
    public async Task<IActionResult> GetQualificationCriteriaByPost([FromBody] DtoQualificationCriteriaByPostRequest request)
    {
        var response = await _master.GetQualificationCriteriaByPost(request.DepartmentId, request.AdvertisementId, request.PostId).ConfigureAwait(false);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }


    // added by valmiki 13082026 for Dropdown

    #region Dropdown Valmiki

    [HttpGet, Route("Getstate")]
    public async Task<IActionResult> GetState()
    {
        var response = await _master.GetState();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetDomicileState")]
    public async Task<IActionResult> GetDomicileState()
    {
        var response = await _master.GetDomicileState();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetParallelReservation")]
    public async Task<IActionResult> GetParallelReservation()
    {
        var response = await _master.GetParallelReservation();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetUserParallelReservation")]
    public async Task<IActionResult> GetUserParallelReservation()
    {
        var response = await _master.GetUserParallelReservation(UserId);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("Getdistrict")]
    public async Task<IActionResult> GetDistrict(int stateid)
    {
        var response = await _master.GetDistrict(stateid);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetDomicileDistrict")]
    public async Task<IActionResult> GetDomicileDistrict(int stateid)
    {
        var response = await _master.GetDomicileDistrict(stateid);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetTaluka")]
    public async Task<IActionResult> GetTaluka(int districtId)
    {
        var response = await _master.GetTaluka(districtId);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetVillage")]
    public async Task<IActionResult> GetVillage(int talukaid)
    {
        var response = await _master.GetVillage(talukaid);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetDegree")]
    public async Task<IActionResult> GetDegree()
    {
        var response = await _master.GetDegree();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetResult")]
    public async Task<IActionResult> GetResult()
    {
        var response = await _master.GetResult();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetCourse")]
    public async Task<IActionResult> GetCourse(int degreeId)
    {
        var response = await _master.GetCourse(degreeId);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetSubject")]
    public async Task<IActionResult> GetSubject(int courseId)
    {
        var response = await _master.GetSubject(courseId);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetBoardUniversity")]
    public async Task<IActionResult> GetBoardUniversity(int stateId)
    {
        var response = await _master.GetBoardUniversity(stateId);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetQualificationMode")]
    public async Task<IActionResult> GetQualificationMode()
    {
        var response = await _master.GetQualificationMode();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetMaritalStatus")]
    public async Task<IActionResult> GetMaritalStatus()
    {
        var response = await _master.GetMaritalStatus();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetGeneralWrittenExamCenters")]
    public async Task<IActionResult> GetGeneralWrittenExamCenters([FromBody] GeneralWrittenExamCenterRequest request)
    {
        var response = await _master.GetGeneralWrittenExamCenters(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetGender")]
    public async Task<IActionResult> GetGender()
    {
        var response = await _master.GetGender();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetMotherTongue")]
    public async Task<IActionResult> GetMotherTongue()
    {
        var response = await _master.GetMotherTongue();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    [HttpGet, Route("GetNationality")]
    public async Task<IActionResult> GetNationality()
    {
        var response = await _master.GetNationality();

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetDisabilityType")]
    public async Task<IActionResult> GetDisabilityType()
    {
        var response = await _master.GetDisabilityType();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpPost, Route("GetDisabilitySubType")]
    public async Task<IActionResult> GetDisabilitySubType([FromBody] DtoGetDisabilitySubType request)
    {
        var response = await _master.GetDisabilitySubType(request.DisabilityTypeId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetDisabilityCertificateType")]
    public async Task<IActionResult> GetDisabilityCertificateType()
    {
        var response = await _master.GetDisabilityCertificateType();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetAnathType")]
    public async Task<IActionResult> GetAnathType()
    {
        var response = await _master.GetAnathType();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [HttpGet, Route("GetUnit")]
    public async Task<IActionResult> GetUnit(int postid)
    {
        var response = await _master.GetUnits(postid);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    #endregion

    private IActionResult? InvalidAdvertisementPost(DtoAdvertisementPost dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PayServiceId))
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Pay Service Id is required."
            });
        }

        if (dto.RecruitmentStartDate.HasValue &&
            dto.RecruitmentEndDate.HasValue &&
            dto.RecruitmentEndDate.Value.Date < dto.RecruitmentStartDate.Value.Date)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Recruitment End Date cannot be before Recruitment Start Date."
            });
        }

        if (dto.PaymentStartDate.HasValue &&
            dto.PaymentEndDate.HasValue &&
            dto.PaymentEndDate.Value.Date < dto.PaymentStartDate.Value.Date)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Payment End Date cannot be before Payment Start Date."
            });
        }

        return null;
    }

    private IActionResult? InvalidAgeCriteria(DtoAgeCriteria dto)
    {
        if (dto.MaximumAge < dto.MinimumAge)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Maximum Age cannot be less than Minimum Age."
            });
        }

        return null;
    }

    private IActionResult? InvalidFeesDetails(List<DtoFeesMasterDetailsPost>? list)
    {
        if (list == null || list.Count == 0)
        {
            return BadRequest(new { Status = false, Message = "Please select at least one category." });
        }

        var first = list[0];
        if (list.Any(x =>
                x.DepartmentId != first.DepartmentId ||
                x.AdvertisementId != first.AdvertisementId ||
                x.PostId != first.PostId ||
                x.CategoryId <= 0 ||
                x.Amount <= 0))
        {
            return BadRequest(new { Status = false, Message = "Invalid fees details." });
        }

        return null;
    }


    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveAdvertisement")]
    public async Task<IActionResult> SaveAdvertisement( [FromBody] DtoAdvertisement dtoAdvertisement)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var invalid = InvalidAdvertisement(dtoAdvertisement, false);

        if (invalid != null)
            return invalid;


        dtoAdvertisement.AdvertisementName = dtoAdvertisement.AdvertisementName?.Trim();
        dtoAdvertisement.AdvertisementNameMarathi = dtoAdvertisement.AdvertisementNameMarathi?.Trim();
        dtoAdvertisement.AdvertisementDesc = dtoAdvertisement.AdvertisementDesc?.Trim();


        // Logged-in Super Admin Id
        dtoAdvertisement.CreatedBy = checked((int)UserId);

        var response = await _master.SaveAdvertisement(dtoAdvertisement).ConfigureAwait(false);


        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }


    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateAdvertisement")]
    public async Task<IActionResult> UpdateAdvertisement( [FromBody] DtoAdvertisement dtoAdvertisement)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var invalid = InvalidAdvertisement(dtoAdvertisement, true);

        if (invalid != null)
            return invalid;


        dtoAdvertisement.AdvertisementName = dtoAdvertisement.AdvertisementName?.Trim();
        dtoAdvertisement.AdvertisementNameMarathi = dtoAdvertisement.AdvertisementNameMarathi?.Trim();

        dtoAdvertisement.AdvertisementDesc = dtoAdvertisement.AdvertisementDesc?.Trim();


        // Logged-in Super Admin Id
        dtoAdvertisement.UpdatedBy = checked((int)UserId);


        var response = await _master.UpdateAdvertisement(dtoAdvertisement).ConfigureAwait(false);


        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementList")]
    public async Task<IActionResult> GetAdvertisementList( [FromBody] DtoAdvertisementList request)
    {
        // Default pagination
        if (request.Page <= 0)
            request.Page = 1;

        if (request.PageSize <= 0)
            request.PageSize = 10;


        var response = await _master.GetAdvertisementList(request).ConfigureAwait(false);


        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementById")]
    public async Task<IActionResult> GetAdvertisementById([FromBody] DtoGetAdvertisementById request)
    {
        var response = await _master.GetAdvertisementById(request.AdvertisementId).ConfigureAwait(false);

        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    private IActionResult? InvalidAdvertisement( DtoAdvertisement dto, bool isUpdate)
    {
        // AdvertisementId required only for Update
        if (isUpdate && dto.AdvertisementId <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Invalid Advertisement Id."
            });
        }


        // Department required
        if (dto.DepartmentId <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Department is required."
            });
        }


        // Exam Year required
        if (!dto.ExamYear.HasValue || dto.ExamYear.Value <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Exam Year is required."
            });
        }


        // Advertisement Name required
        if (string.IsNullOrWhiteSpace( dto.AdvertisementName))
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Name is required."
            });
        }


        // Max length according to DB
        if (dto.AdvertisementName.Trim().Length > 250)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Name cannot exceed 250 characters."
            });
        }

        // Advertisement Name (Marathi) required
        if (string.IsNullOrWhiteSpace(dto.AdvertisementNameMarathi))
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Name (Marathi) is required."
            });
        }

        if (dto.AdvertisementNameMarathi.Trim().Length > 250)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Name (Marathi) cannot exceed 250 characters."
            });
        }


        // Advertisement Description required
        if (string.IsNullOrWhiteSpace( dto.AdvertisementDesc))
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Description is required."
            });
        }


        // Your DB column is nvarchar(250)
        if (dto.AdvertisementDesc.Trim().Length > 250)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Description cannot exceed 250 characters."
            });
        }


        // Start Date required
        if (!dto.AdvertisementStartDate.HasValue)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement Start Date is required."
            });
        }


        // End Date required
        if (!dto.AdvertisementEndDate.HasValue)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement End Date is required."
            });
        }


        // End Date >= Start Date
        if (dto.AdvertisementEndDate.Value.Date <
            dto.AdvertisementStartDate.Value.Date)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Advertisement End Date cannot be before Advertisement Start Date."
            });
        }


        return null;
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("CloneAdvertisementTables")]
    public async Task<IActionResult> CloneAdvertisementTables([FromBody] DtoCloneAdvertisementTables request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.CloneAdvertisementTables(request, UserId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetAdvertisementTablesList")]
    public async Task<IActionResult> GetAdvertisementTablesList([FromBody] DtoAdvertisementTablesList request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetAdvertisementTablesList(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetSourceAdvertisementTables")]
    public async Task<IActionResult> GetSourceAdvertisementTables()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetSourceAdvertisementTables();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("ToggleAdvertisementTableStatus")]
    public async Task<IActionResult> ToggleAdvertisementTableStatus([FromBody] DtoToggleAdvertisementTableStatus request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.ToggleAdvertisementTableStatus(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("GetActiveUnitList")]
    public async Task<IActionResult> GetActiveUnitList()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetActiveUnitList();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveDepartmentUser")]
    public async Task<IActionResult> SaveDepartmentUser([FromBody] DtoDepartmentUserSave request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        if (request.UserId <= 0 && string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new Response { Status = false, Message = "Password is required for new department user." });
        }

        request.CreatedBy = UserId;
        var response = await _master.SaveDepartmentUser(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetDepartmentUserList")]
    public async Task<IActionResult> GetDepartmentUserList([FromBody] DtoDepartmentUserList request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetDepartmentUserList(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetDepartmentUserById")]
    public async Task<IActionResult> GetDepartmentUserById([FromBody] DtoGetDepartmentUserById request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetDepartmentUserById(request.UserId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("ToggleDepartmentUserStatus")]
    public async Task<IActionResult> ToggleDepartmentUserStatus([FromBody] DtoDepartmentUserStatusToggle request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.ToggleDepartmentUserStatus(request.UserId, request.IsActive, UserId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    #region Education Master (Qualification, Course, Subject)
    // ================= Qualification Type Master =================
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveQualification")]
    public async Task<IActionResult> SaveQualification([FromBody] DtoQualification request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        if (request.QualificationId <= 0)
        {
            request.CreatedBy = UserId;
        }
        else
        {
            request.UpdatedBy = UserId;
        }

        var response = await _master.SaveQualification(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetQualificationMasterList")]
    public async Task<IActionResult> GetQualificationMasterList([FromBody] DtoQualificationList request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetQualificationMasterList(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetQualificationById")]
    public async Task<IActionResult> GetQualificationById([FromBody] DtoGetQualificationById request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetQualificationById(request.QualificationId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("ToggleQualificationStatus")]
    public async Task<IActionResult> ToggleQualificationStatus([FromBody] DtoQualificationStatusToggle request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        request.UpdatedBy = UserId;
        var response = await _master.ToggleQualificationStatus(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    // ================= Name of Qualification (Course) Master =================
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveCourse")]
    public async Task<IActionResult> SaveCourse([FromBody] DtoCourse request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        if (request.CourseId <= 0)
        {
            request.CreatedBy = UserId;
        }
        else
        {
            request.UpdatedBy = UserId;
        }

        var response = await _master.SaveCourse(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetCourseMasterList")]
    public async Task<IActionResult> GetCourseMasterList([FromBody] DtoCourseList request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetCourseMasterList(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetCourseById")]
    public async Task<IActionResult> GetCourseById([FromBody] DtoGetCourseById request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetCourseById(request.CourseId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("ToggleCourseStatus")]
    public async Task<IActionResult> ToggleCourseStatus([FromBody] DtoCourseStatusToggle request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        request.UpdatedBy = UserId;
        var response = await _master.ToggleCourseStatus(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    // ================= Subject / Stream / Branch Master =================
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveSubject")]
    public async Task<IActionResult> SaveSubject([FromBody] DtoSubject request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        if (request.SubjectId <= 0)
        {
            request.CreatedBy = UserId;
        }
        else
        {
            request.UpdatedBy = UserId;
        }

        var response = await _master.SaveSubject(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetSubjectMasterList")]
    public async Task<IActionResult> GetSubjectMasterList([FromBody] DtoSubjectList request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetSubjectMasterList(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetSubjectById")]
    public async Task<IActionResult> GetSubjectById([FromBody] DtoGetSubjectById request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetSubjectById(request.SubjectId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("ToggleSubjectStatus")]
    public async Task<IActionResult> ToggleSubjectStatus([FromBody] DtoSubjectStatusToggle request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        request.UpdatedBy = UserId;
        var response = await _master.ToggleSubjectStatus(request);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    #endregion

    #region Role & Menu Management
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetRoleMasterList")]
    public async Task<IActionResult> GetRoleMasterList()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetRoleMasterList();
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveRoleMaster")]
    public async Task<IActionResult> SaveRoleMaster([FromBody] DtoSaveRoleRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.SaveRoleMaster(request, UserId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetMenuList")]
    public async Task<IActionResult> GetMenuList([FromBody] DtoMenuFilterRequest? request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetMenuList(request?.RoutePrefix);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetParentMenuDropdown")]
    public async Task<IActionResult> GetParentMenuDropdown([FromBody] DtoMenuFilterRequest? request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetParentMenuDropdown(request?.RoutePrefix);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveMenu")]
    public async Task<IActionResult> SaveMenu([FromBody] DtoSaveMenuRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.SaveMenu(request, UserId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("GetRoleMenuMapping")]
    public async Task<IActionResult> GetRoleMenuMapping([FromBody] DtoGetRoleMenuMappingRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.GetRoleMenuMappingByRoutePrefix(request.RoutePrefix);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }

    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveRoleMenuMapping")]
    public async Task<IActionResult> SaveRoleMenuMapping([FromBody] DtoSaveRoleMenuMappingRequest request)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _master.SaveRoleMenuMapping(request, UserId);
        return response.Status
            ? StatusCode(StatusCodes.Status200OK, response)
            : StatusCode(StatusCodes.Status409Conflict, response);
    }
    #endregion



    #region Written Hall Ticket Instructions

    // 1. SAVE
    //[Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("SaveWrittenHallTicketInstruction")]
    public async Task<IActionResult> SaveWrittenHallTicketInstruction( [FromBody] WrittenHallTicketInstructionModel request)
    {


        //// TEMPORARY DEBUGGING
        //var isAuthenticated = User.Identity?.IsAuthenticated;

        //var userId = UserId;

        //var claims = User.Claims
        //    .Select(x => new
        //    {
        //        x.Type,
        //        x.Value
        //    })
        //    .ToList();


        //var invalidUser = InvalidUserResponse();
        //if (invalidUser != null)
        //    return invalidUser;

        if (request.DepartmentId <= 0 ||
            request.AdvertisementId <= 0 ||
            request.PostId <= 0 ||
            string.IsNullOrWhiteSpace(request.Instruction))
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Department, Advertisement, Post and Instruction are required."
            });
        }

        request.SrNo = 0;
        request.Instruction = request.Instruction.Trim();

        var response = await _master
            .SaveWrittenHallTicketInstruction(request)
            .ConfigureAwait(false);

        return response.Status ? Ok(response) : Conflict(response);
    }


    // 2. UPDATE
    //[Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpPost, Route("UpdateWrittenHallTicketInstruction")]
    public async Task<IActionResult> UpdateWrittenHallTicketInstruction( [FromBody] WrittenHallTicketInstructionModel request)
    {
        //var invalidUser = InvalidUserResponse();
        //if (invalidUser != null)
        //    return invalidUser;

        if (request.SrNo <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Valid instruction ID is required."
            });
        }

        if (request.DepartmentId <= 0 ||
            request.AdvertisementId <= 0 ||
            request.PostId <= 0 ||
            string.IsNullOrWhiteSpace(request.Instruction))
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Department, Advertisement, Post and Instruction are required."
            });
        }

        request.Instruction = request.Instruction.Trim();

        var response = await _master
            .UpdateWrittenHallTicketInstruction(request)
            .ConfigureAwait(false);

        return response.Status ? Ok(response) : Conflict(response);
    }


    // 3. GET ALL
    //[Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("GetAllWrittenHallTicketInstructions")]
    public async Task<IActionResult> GetAllWrittenHallTicketInstructions()
    {
        var response = await _master
            .GetAllWrittenHallTicketInstructions()
            .ConfigureAwait(false);

        return response.Status ? Ok(response) : Conflict(response);
    }


    // 4. GET BY ID
    //[Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpGet, Route("GetWrittenHallTicketInstructionById/{srNo}")]
    public async Task<IActionResult> GetWrittenHallTicketInstructionById(int srNo)
    {
        if (srNo <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Invalid instruction ID."
            });
        }

        var response = await _master
            .GetWrittenHallTicketInstructionById(srNo)
            .ConfigureAwait(false);

        return response.Status ? Ok(response) : Conflict(response);
    }


    // 5. DELETE
    //[Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    [HttpDelete, Route("DeleteWrittenHallTicketInstruction/{srNo}")]
    public async Task<IActionResult> DeleteWrittenHallTicketInstruction(int srNo)
    {
        //var invalidUser = InvalidUserResponse();
        //if (invalidUser != null)
        //    return invalidUser;

        if (srNo <= 0)
        {
            return BadRequest(new
            {
                Status = false,
                Message = "Invalid instruction ID."
            });
        }

        var response = await _master
            .DeleteWrittenHallTicketInstruction(srNo)
            .ConfigureAwait(false);

        return response.Status ? Ok(response) : Conflict(response);
    }

    #endregion
    
}

