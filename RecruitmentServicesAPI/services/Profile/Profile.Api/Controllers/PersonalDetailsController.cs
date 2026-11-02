using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using Profile.Infrastructure.Applicant;
using Shared.Auth;
using Shared.Kernel;

namespace Profile.Api.Controllers
{
    [Route("api/applicant/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.ApplicantOnly)]
    public class PersonalDetailsController(IPersonalDetailsRepository personalDetailsRepository, IApplicantRepository applicantRepository) : BaseController
    {
        public readonly IPersonalDetailsRepository _personalDetailsRepository = personalDetailsRepository;
        private readonly IApplicantRepository _applicant = applicantRepository;

        [HttpPost]
        [Route("SavePersonalDetails")]
        public async Task<IActionResult> SavePersonalDetails([FromBody] PersonalDetailsRequest obj)
        {

            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid UserId."
                });
            }

            obj.UserId = UserId;
            obj.IPAddress = IpAddress;
            obj.CreatedBy = Convert.ToInt32(UserId);
            obj.isDraft = 0;
            var response = await _personalDetailsRepository.SavePersonalDetails(obj);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost]
        [Route("SavePersonalDraftDetails")]
        public async Task<IActionResult> SavePersonalDraftDetails([FromBody] PersonalDetailsDraftRequest obj)
        {

            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid UserId."
                });
            }

            obj.UserId = UserId;
            obj.IPAddress = IpAddress;
            obj.CreatedBy = Convert.ToInt32(UserId);
            obj.isDraft = 1;
            var response = await _personalDetailsRepository.SavePersonalDraftDetails(obj);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet("GetPersonalDetails")]
        public async Task<IActionResult> GetPersonalDetails()

        {
            
            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid UserId."
                });
            }

            var response =
                await _personalDetailsRepository.GetPersonalDetails(UserId);

            if (response == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Details Not Found"
                });
            }

            if (response.Data != null)
            {
                var isComplete = await _applicant.IsProfileComplete(UserId).ConfigureAwait(false);
                response.Data.IsProfileComplete = isComplete ? 1 : 0;
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("GetAadhaarDetails")]
        public async Task<IActionResult> GetAadhaarDetails()
        {
            
            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid UserId."
                });
            }
            var response = await _personalDetailsRepository.GetAadhaarDetails(UserId);

            return response.Status
                ? Ok(response)
                : Conflict(response);
        }
    }
}
