using Application.Application.Interfaces;
using Application.Domain.Application;
using Application.Infrastructure.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using Shared.Auth;
using Shared.Kernel;
using System.IO;
using System.Threading.Tasks;

namespace Application.Api.Controller
{
    [Route("api/Applications/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.DeptUserOnly)]
    public class GeneralWrittenMusterController(
        IWrittenMusterRepository writtenMusterRepository,
        IAttendancePdfStorageService attendancePdfStorageService) : BaseController
    {
        private readonly IWrittenMusterRepository _writtenMusterRepository = writtenMusterRepository;
        private readonly IAttendancePdfStorageService _attendancePdfStorageService = attendancePdfStorageService;

        [HttpPost, Route("GetGeneralWrittenScheduleDetails")]
        public async Task<IActionResult> GetGeneralWrittenScheduleDetails(
            [FromBody] GeneralWrittenMusterPaginationRequest request)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            if (UserDepartmentId <= 0)
            {
                return Ok(new
                {
                    Status = false,
                    Message = "Invalid Department."
                });
            }

            var response = await _writtenMusterRepository.GetGeneralWrittenScheduleDetails(
                UserDepartmentId,
                request,
                request.Search?.Trim() ?? string.Empty);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet, Route("DownloadGeneralWrittenAttendancePdf")]
        public async Task<IActionResult> DownloadGeneralWrittenAttendancePdf(
            [FromQuery] GeneralWrittenAttendancePdfRequest request)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            if (UserDepartmentId <= 0)
            {
                return Ok(new
                {
                    Status = false,
                    Message = "Invalid Department."
                });
            }

            var response = await _writtenMusterRepository.GetGeneralWrittenAttendancePdfData(
                UserDepartmentId,
                request.AdvertisementId,
                request.PostId,
                request.FilterId);

            if (!response.Status || response.Data == null || response.Data.Count == 0)
            {
                return StatusCode(StatusCodes.Status409Conflict, response);
            }

            var pdfBytes = new GeneralAttendancePdfDocument(response.Data).GeneratePdf();

            var fileName = Path.GetFileName(request.PdfPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"GeneralWrittenMuster_{request.FilterId}.pdf";
            }

            _attendancePdfStorageService.SavePdf(pdfBytes, request.PdfPath);

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
