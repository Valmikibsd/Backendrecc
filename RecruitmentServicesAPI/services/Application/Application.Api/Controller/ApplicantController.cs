using Application.Application.Interfaces;
using Application.Domain.Application;
using Application.Domain.Model;
using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using Shared.Kernel;
using Shared.Kernel.Helpers;
using Shared.Kernel.Interfaces;
using System.Linq;
using System.Security.Claims;

namespace Application.Api.Controller
{

    [ApiController]
    [Route("api/Applications")]
    [Authorize]
    public class ApplicantController(
        IApplicantRepository ApplicantRepository,
        Shared.Kernel.Interfaces.ICaptchaService captchaService,
        IPostRecruitmentDateRepository postRecruitmentDateRepository) : BaseController
    {
        private readonly IApplicantRepository _objApplicantRepository = ApplicantRepository;
        private readonly Shared.Kernel.Interfaces.ICaptchaService _captchaService = captchaService;
        private readonly IPostRecruitmentDateRepository _postRecruitmentDateRepository = postRecruitmentDateRepository;

        [HttpPost, Route("GetCandidateAppliedPosts")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> GetCandidateAppliedPosts([FromBody] CandidateAppliedPostRequest request)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            var response = await _objApplicantRepository.GetCandidateAppliedPosts(UserId);
            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost("GetCancelledApplications")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> GetCancelledApplications()
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            var response = await _objApplicantRepository.GetCancelledApplications(UserId);
            return response.Status
                ? Ok(response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost, Route("GetReceiptDetails")]
        [Authorize(Policy = AuthPolicies.ViewApplicationAccess)]
        public async Task<IActionResult> GetReceiptDetails([FromBody] ReceiptRequest request)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            if (string.Equals(UserRoutePrefix, RolePrefixes.Applicant, StringComparison.OrdinalIgnoreCase))
            {
                request.UserId = UserId;
            }
            else if (string.Equals(UserRoutePrefix, RolePrefixes.DeptUser, StringComparison.OrdinalIgnoreCase) && UserDepartmentId > 0)
            {
                request.DepartmentId = UserDepartmentId;
            }

            var response = await _objApplicantRepository.GetReceiptDetails(request);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost, Route("GetPhysicalHallTicket")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> GetPhysicalHallTicket([FromBody] HallTicketRequest request)
        {
            var response = await _objApplicantRepository.GetPhysicalHallTicket(request);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost, Route("GetWrittenHallTicket")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> GetWrittenHallTicket([FromBody] WrittenHallTickRequest request)
        {
            var response = await _objApplicantRepository.GetWrittenHallTicket(request);

            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost("SendCancellationOtp")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> SendCancellationOtp([FromBody] SendCancellationOtpRequest request)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            if (!_captchaService.ValidateToken(request.CaptchaToken))
            {
                return BadRequest(new Response
                {
                    Status = false,
                    Message = "CAPTCHA verification expired. A new CAPTCHA is shown. Please enter it."
                });
            }

            var response = await _objApplicantRepository.SendCancellationOtp(UserId, request.AppNo);
            return response.Status
                ? Ok(response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost("CancelApplication")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> CancelApplication([FromBody] CancelApplicationRequest request)
        {
            var invalidUser = InvalidUserResponse();

            if (invalidUser != null)
            {
                return invalidUser;
            }

            request.UserId = UserId;
            request.CancelledIpAddress = IpAddress;
            var response = await _objApplicantRepository.CancelApplication(request);

            return response.Status
                ? Ok(response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpGet("CheckEligibilityCriteria")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> CheckEligibilityCriteria([FromQuery] EligibilityCriteriaRequest obj)
        {
            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid applicant."
                });
            }
            obj.UserId = UserId;

            var dateCheckResponse = await _postRecruitmentDateRepository.CheckPostRecruitmentDates(
                obj.DepartmentId ?? 0,
                obj.AdvertisementId ?? 0,
                obj.PostId ?? 0);

            var dateCheck = dateCheckResponse.Data?.FirstOrDefault();

            if (dateCheck == null || !dateCheck.Status)
            {
                return BadRequest(new
                {
                    Status = false,
                    dateCheck.Message
                });
            }

            var result = await _objApplicantRepository.CheckEligibilityCriteria(obj);

            if (!result.Status)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No data found."
                });
            }
            
            return Ok(result);
        }
        [HttpGet("CheckApplicationEligibilityCriteria")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> CheckApplicationEligibilityCriteria([FromQuery] ApplicationEligibilityCriteriaRequest obj)
        {
            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid applicant."
                });
            }
            obj.UserId = UserId;

            var result = await _objApplicantRepository.CheckApplicationEligibilityCriteria(obj);

            if (!result.Status)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No data found."
                });
            }
            
            return Ok(result);
        }

        [HttpPost("SubmitApplication")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> SubmitApplication(
        [FromBody] PostApplicationRequest request)
        {
            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid UserId."
                });
            }
            if (string.IsNullOrWhiteSpace(request.Sign) || string.IsNullOrWhiteSpace(request.Photo))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Photo and Signature are required."
                });
            }

            if (request.IsUnit)
            {
                if (request.UnitId == null)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Unit required."
                    });
                }
            }
            var isFemale = IsFemaleApplicant(request);
            OtherDetailsValidator.ClearHiddenFields(request.OtherDetails, isFemale);

            var errors = OtherDetailsValidator.ValidateForSave(request.OtherDetails, isFemale);
            if (errors.Count > 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid Records details.",
                    Errors = errors
                });
            }

            var dateCheckResponse = await _postRecruitmentDateRepository.CheckPostRecruitmentDates(
                request.DepartmentId,
                request.AdvertisementId,
                request.Postid);

            var dateCheck = dateCheckResponse.Data?.FirstOrDefault();

            if (dateCheck == null || !dateCheck.Status)
            {
                return BadRequest(new
                {
                    Status = false,
                    dateCheck.Message
                });
            }
            ApplicationEligibilityCriteriaRequest obj = new()
            {
                UserId = UserId,
                PostId=request.Postid,
                Unit=request.UnitId,
                DepartmentId=request.DepartmentId,
                AdvertisementId=request.AdvertisementId,
                ParallelReservationId=request.ParelledReservationId,
                CategoryId=request.OtherDetails.CasteCategory,

            };
            
            var result = await _objApplicantRepository.CheckApplicationEligibilityCriteria(obj);
            var eligibility = result.Data?.LastOrDefault();
            if (!eligibility.IsEligible)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message="Candidate is not eligible"
                });
            }

            request.PersonalInformation.DepartmentId = request.DepartmentId;
            request.PersonalInformation.Postid = request.Postid;
            request.PersonalInformation.AdvertisementId = request.AdvertisementId;
            request.PersonalInformation.UnitId = request.UnitId;
            request.PersonalInformation.ParelledReservationId = request.ParelledReservationId;
            request.PersonalInformation.UserId = UserId;
            request.PersonalInformation.IPAddress = IpAddress;
            request.PersonalInformation.Category = request.OtherDetails.CasteCategory;
            var response = await _objApplicantRepository.SaveApplication(request);

            if (!response.Status)
            {
                return StatusCode(StatusCodes.Status409Conflict, response);
            }

            return Ok(response);
        }

        [HttpGet("CheckApplicationExists")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> CheckApplicationExists([FromQuery] CheckApplicantionRequest obj)
        {
            if (UserId <= 0)
            {
                return (BadRequest(new
                {
                    Status = false,
                    Message = "Invalid applicant."
                }));
            }
            obj.UserId = UserId;
            var response =
              await _objApplicantRepository.CheckApplicationExists(obj);
            return response.Status
                ? StatusCode(
                    StatusCodes.Status200OK,
                    response)
                : StatusCode(
                    StatusCodes.Status404NotFound,
                    response);
        }

        [HttpGet("ViewApplication")]
        [Authorize(Policy = AuthPolicies.ViewApplicationAccess)]
        public async Task<IActionResult> ViewApplication([FromQuery] ViewApplicantionRequest obj)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }
            // 1. If Applicant: Force UserId to logged-in user's token ID
            if (string.Equals(UserRoutePrefix, RolePrefixes.Applicant, StringComparison.OrdinalIgnoreCase))
            {
                obj.UserId = UserId;
            }
            // 2. If Department User: Force DepartmentId to logged-in user's department claim
            else if (string.Equals(UserRoutePrefix, RolePrefixes.DeptUser, StringComparison.OrdinalIgnoreCase) && UserDepartmentId > 0)
            {
                obj.DepartmentId = UserDepartmentId;
            }

            var response = await _objApplicantRepository.ViewApplication(obj);

            if (!response.Status)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No data found."
                });
            }

            return Ok(response);
        }

        [HttpGet("ViewCancelledApplicationDetails")]
        [Authorize(Policy = AuthPolicies.ApplicantOnly)]
        public async Task<IActionResult> ViewCancelledApplicationDetails([FromQuery] ViewApplicantionRequest obj)
        {
            var invalidUser = InvalidUserResponse();
            if (invalidUser != null)
            {
                return invalidUser;
            }

            if (string.Equals(UserRoutePrefix, RolePrefixes.Applicant, StringComparison.OrdinalIgnoreCase))
            {
                obj.UserId = UserId;
            }
            else if (string.Equals(UserRoutePrefix, RolePrefixes.DeptUser, StringComparison.OrdinalIgnoreCase) && UserDepartmentId > 0)
            {
                obj.DepartmentId = UserDepartmentId;
            }

            var response = await _objApplicantRepository.ViewCancelledApplicationDetails(obj);

            if (!response.Status)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "No data found."
                });
            }

            return Ok(response);
        }

        private static bool IsFemaleApplicant(PostApplicationRequest request) =>
            request.PersonalInformation?.Gender == 2;



        [HttpPost("SaveApplicationNo")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> SaveApplicationNo( [FromBody] DtoApplicationNo request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new Response
                    {
                        Status = false,
                        Message = "Invalid request."
                    });
                }


                // Year must be exactly 2 digits
                if (request.Year < 10 ||
                    request.Year > 99)
                {
                    return BadRequest(new Response
                    {
                        Status = false,
                        Message = "Year must be exactly 2 digits."
                    });
                }


             
                // Department
                if (request.DepartmentId <= 0)
                {
                    return BadRequest(new Response
                    {
                        Status = false,
                        Message = "Department is required."
                    });
                }


               
                // Advertisement             
                if (request.AdvertisementId <= 0)
                {
                    return BadRequest(new Response
                    {
                        Status = false,
                        Message = "Advertisement is required."
                    });
                }

           
                // Post       
                if (request.PostId <= 0)
                {
                    return BadRequest(new Response
                    {
                        Status = false,
                        Message = "Post is required."
                    });
                }


                
                // AppNo must initially be exactly 0             
                if (request.AppNo != 0)
                {
                    return BadRequest(new Response
                    {
                        Status = false,
                        Message = "Application Number must be 0."
                    });
                }


                var response = await _objApplicantRepository.SaveApplicationNo(request);


                return response.Status
                    ? StatusCode(
                        StatusCodes.Status200OK,
                        response
                    )
                    : StatusCode(
                        StatusCodes.Status409Conflict,
                        response
                    );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response
                    {
                        Status = false,
                        Message = ex.Message
                    }
                );
            }
        }

        [HttpGet("GetApplicationNoList")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> GetApplicationNoList()
        {
            var response =  await _objApplicantRepository.GetApplicationNoList();

            return 
                response.Status
                ?StatusCode(StatusCodes.Status200OK, response)
                :StatusCode( StatusCodes.Status409Conflict,response );
        }


    }
}
