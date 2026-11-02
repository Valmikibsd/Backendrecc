using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.Application.Applicant;
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
    public class OtherDetailsController(
        ICrudOperationService crudOperationService,
        IApplicantRepository applicantRepository,
        IPersonalDetailsRepository personalDetailsRepository) : BaseController
    {
        private readonly IOtherDetailsRepository _otherDetailsRepository = new OtherDetailsRepository(crudOperationService);
        private readonly IApplicantRepository _applicant = applicantRepository;
        private readonly IPersonalDetailsRepository _personalDetailsRepository = personalDetailsRepository;

        [HttpPost]
        [Route("SaveOtherDetails")]
        public async Task<IActionResult> SaveOtherDetails([FromBody] OtherDetailsRequest obj)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            obj.UserId = UserId;
            obj.IPAddress = IpAddress;
            obj.CreatedBy = UserId;
            obj.IsDraft = 0;

            var isFemale = await IsFemaleApplicant();
            OtherDetailsValidator.ClearHiddenFields(obj, isFemale);

            var errors = OtherDetailsValidator.ValidateForSave(obj, isFemale);
            if (errors.Count > 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid Records details.",
                    Errors = errors
                });
            }

            var response = await _otherDetailsRepository.SaveOtherDetails(obj);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost]
        [Route("SaveOtherDraftDetails")]
        public async Task<IActionResult> SaveOtherDraftDetails([FromBody] OtherDetailsRequest obj)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            obj.UserId = UserId;
            obj.IPAddress = IpAddress;
            obj.CreatedBy = UserId;
            obj.IsDraft = 1;

            var isFemale = await IsFemaleApplicant();
            OtherDetailsValidator.ClearHiddenFields(obj, isFemale);

            var response = await _otherDetailsRepository.SaveOtherDetails(obj);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet]
        [Route("GetOtherDetails")]
        public async Task<IActionResult> GetOtherDetails()
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            var response = await _otherDetailsRepository.GetOtherDetails(UserId);

            if (response == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Details Not Found"
                });
            }

            return Ok(response);
        }

        private async Task<bool> IsFemaleApplicant()
        {
            var personal = await _personalDetailsRepository.GetPersonalDetails(UserId);
            return personal?.Data?.Gender == 2;
        }
    }
}
