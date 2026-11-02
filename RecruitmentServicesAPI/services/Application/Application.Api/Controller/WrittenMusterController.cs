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
    public class WrittenMusterController(
        IWrittenMusterRepository writtenMusterRepository,
        IAttendancePdfStorageService attendancePdfStorageService) : BaseController
    {
        private readonly IWrittenMusterRepository _writtenMusterRepository = writtenMusterRepository;
        private readonly IAttendancePdfStorageService _attendancePdfStorageService = attendancePdfStorageService;

        [HttpPost, Route("GetWrittenScheduleDetails")]
        public async Task<IActionResult> GetWrittenScheduleDetails(
            [FromBody] WrittenMusterPaginationRequest request)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }
            if (string.IsNullOrWhiteSpace(UserUnitId))
            {
                return Ok(new
                {
                    Status = false,
                    Message = "Invalid Unit."
                });
            }

            var response = await _writtenMusterRepository.GetWrittenScheduleDetails(
                UserUnitId,
                request.Page,
                request.PageSize,
                request.Search?.Trim() ?? string.Empty);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet, Route("DownloadWrittenAttendancePdf")]
        public async Task<IActionResult> DownloadWrittenAttendancePdf([FromQuery] int filterId, [FromQuery] string pdfPath)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
               return invalidUser;
            }

            if (filterId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid FilterId."
                });
            }

            if (string.IsNullOrWhiteSpace(pdfPath))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "PdfPath is required."
                });
            }

            var response = await _writtenMusterRepository.GetWrittenAttendancePdfData(filterId);

            if (!response.Status || response.Data == null || response.Data.Count == 0)
            {
                return StatusCode(StatusCodes.Status409Conflict, response);
            }

            var document = new AttendancePdfDocument(response.Data);

            var pdfBytes = document.GeneratePdf();

            var fileName = Path.GetFileName(pdfPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"WrittenMuster_{filterId}.pdf";
            }

            _attendancePdfStorageService.SavePdf(pdfBytes, pdfPath);

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
