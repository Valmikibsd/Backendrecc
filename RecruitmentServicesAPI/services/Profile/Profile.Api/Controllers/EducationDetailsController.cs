using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using Shared.Auth;
using Shared.Kernel;
using System.Linq;
using System.Security.Cryptography;

namespace Profile.Api.Controllers
{
    [Route("api/applicant/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.ApplicantOnly)]
    public class EducationDetailsController(IEducationRepository edicationRepository, IApplicantRepository applicantRepository) :  BaseController
    {
        public readonly IEducationRepository _educationRepository = edicationRepository;
        private readonly IApplicantRepository _applicant = applicantRepository;

        [HttpPost]
        [Route("SaveEducation")]
        public async Task<IActionResult> SaveEducation([FromBody] EducationRequest obj)
        {

            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid UserId."
                });
            }

            var check =
              await _educationRepository.CheckDegreeCourse(UserId, obj.DegreeID, obj.CourseId, obj.Eid);
            if (check.Data.available)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Degree and Course Allready Exists.!",
                    
                });
            }
            obj.UserId = UserId;
            obj.IPAddress = IpAddress;
            obj.CreatedBy = Convert.ToInt32(UserId);
            var response = await _educationRepository.SaveEducation(obj);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet]
        [Route("GetEducationDetails")]
        public async Task<IActionResult> GetEducationDetails()
        {
            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid UserId."
                });
            }

            

            var response =
                await _educationRepository.GetEducationDetails(UserId);

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

        [HttpGet]
        [Route("DeleteEducation")]
        public async Task<IActionResult> DeleteEducation(long eid)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            if (eid <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid EducationId."
                });
            }

            var existing = await _educationRepository.GetEducationDetails(UserId);
            var belongsToUser = existing?.Data?.Any(e => e.Eid == eid) == true;
            if (!belongsToUser)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Education Detail Not Found."
                });
            }

            var response =
                await _educationRepository.DeleteEducation(eid, UserId, IpAddress);

            return Ok(response);
        }

        [HttpGet("CheckDegreeCourse")]
        public async Task<IActionResult> CheckDegreeCourse(int degreeId, int courseId, long Eid)
        {
            if (UserId <= 0)
            {
                return (BadRequest(new
                {
                    Status = false,
                    Message = "Invalid UserId."
                }));
            }
            var response =
              await _educationRepository.CheckDegreeCourse(UserId, degreeId, courseId, Eid);
            return response.Status
                ? StatusCode(
                    StatusCodes.Status200OK,
                    response)
                : StatusCode(
                    StatusCodes.Status404NotFound,
                    response);
        }
    }
}
