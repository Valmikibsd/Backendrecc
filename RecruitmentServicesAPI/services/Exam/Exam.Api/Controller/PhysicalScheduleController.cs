using Exam.Application.Interfaces;
using Exam.Domain.Exam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;

namespace Exam.Api.Controller
{
    [ApiController]
    [Route("api/Exam")]
    [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
    public class PhysicalScheduleController( IPhysicalScheduleRepository PhysicalScheduleRepository ) : ControllerBase
    {
        private readonly IPhysicalScheduleRepository _physicalScheduleRepository = PhysicalScheduleRepository;


        [HttpPost, Route("GetPhysicalSchedule")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> GetPhysicalScheduleMaster( [FromBody] PhysicalScheduleMasterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Action))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Action is required."
                });
            }

            var action = request.Action.Trim().ToUpper();

            string[] validActions =
            {
                "DEPARTMENT",
                "ADVERTISEMENT",
                "POST",
                "UNIT",
                "CATEGORY",
                "GENDER",
                "CENTER",
                "WRITTENCENTER"
            };

            if (!validActions.Contains(action))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid Action."
                });
            }

            if (action == "ADVERTISEMENT" && request.DepartmentId.GetValueOrDefault() <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "DepartmentId is required."
                });
            }

            if (action == "POST" &&
                (
                    request.DepartmentId.GetValueOrDefault() <= 0 ||
                    request.AdvertisementId.GetValueOrDefault() <= 0
                ))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "DepartmentId and AdvertisementId are required."
                });
            }

            if (action == "UNIT" && request.PostId.GetValueOrDefault() <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "PostId is required."
                });
            }

            if ((request.Action.Equals("CENTER", StringComparison.OrdinalIgnoreCase) ||
      request.Action.Equals("WRITTENCENTER", StringComparison.OrdinalIgnoreCase))
     && string.IsNullOrWhiteSpace(request.UnitId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Unit is required."
                });
            }

            request.Action = action;

            var response = await _physicalScheduleRepository.GetPhysicalScheduleMaster(request);
            return response.Status
                ? StatusCode(StatusCodes.Status200OK, response)
                : StatusCode(StatusCodes.Status409Conflict, response);
        }

        [HttpPost]
        [Route("SearchPhysicalScheduleApplications")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> SearchPhysicalScheduleApplications( [FromBody] PhysicalScheduleSearchRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid request."
                });
            }

            if (request.DepartmentId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Department is required."
                });
            }

            if (request.AdvertisementId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Advertisement is required."
                });
            }

            if (request.PostId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Post is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.UnitId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Unit is required."
                });
            }

            if (request.Capacity <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Capacity must be greater than 0."
                });
            }

            if (request.ScheduledDate == default)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Schedule date is required."
                });
            }

            var data = await _physicalScheduleRepository.SearchPhysicalScheduleApplications(request);

            return Ok(new
            {
                status = true,
                message = "Physical schedule count fetched successfully.",
                data
            });
        }


        [HttpPost]
        [Route("GeneratePhysicalSchedule")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult>  GeneratePhysicalSchedule( [FromBody] GeneratePhysicalScheduleRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid request."
                });
            }


            if (request.DepartmentId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Department is required."
                });
            }


            if (request.AdvertisementId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Advertisement is required."
                });
            }


            if (string.IsNullOrWhiteSpace(request.PostId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Post is required."
                });
            }


            if (string.IsNullOrWhiteSpace(request.UnitId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Unit is required."
                });
            }


            if (request.Gender <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Gender is required."
                });
            }


            if (request.ExService != 0 &&
                request.ExService != 1)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid Ex-Serviceman value."
                });
            }


            if (request.SportMan != 0 &&
                request.SportMan != 1)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid Sportsman value."
                });
            }


            if (request.Capacity <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Capacity must be greater than 0."
                });
            }


            if (string.IsNullOrWhiteSpace(
                request.CenterName))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Physical Center is required."
                });
            }


            if (request.ScheduledDate == default)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Scheduled date is required."
                });
            }


            if (string.IsNullOrWhiteSpace(
                request.ScheduledTime))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Scheduled time is required."
                });
            }


            var result = await _physicalScheduleRepository.GeneratePhysicalSchedule(request);


            return Ok(new
            {
                status = result.Success,
                message = result.Message,
                data = result
            });
        }

        [HttpPost]
        [Route("SearchWrittenScheduleApplications")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> SearchWrittenScheduleApplications( [FromBody] WrittenScheduleSearchRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid request."
                });
            }


            if (request.DepartmentId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Department is required."
                });
            }


            if (request.AdvertisementId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Advertisement is required."
                });
            }


            if (request.PostId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Post is required."
                });
            }


            if (string.IsNullOrWhiteSpace(request.UnitId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Unit is required."
                });
            }


            if (request.Gender <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Gender is required."
                });
            }


            var result = await _physicalScheduleRepository.SearchWrittenScheduleApplications(request);


            return Ok(new
            {
                status = true,
                message = result.TotalCount > 0
                    ? "Written schedule applications found."
                    : "No records found.",
                data = result
            });
        }

        [HttpPost]
        [Route("GenerateWrittenSchedule")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> GenerateWrittenSchedule( [FromBody] GenerateWrittenScheduleRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid request."
                });
            }

            if (request.DepartmentId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Department is required."
                });
            }

            if (request.AdvertisementId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Advertisement is required."
                });
            }

            if (request.PostId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Post is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.UnitId))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Unit is required."
                });
            }

            if (request.Gender <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Gender is required."
                });
            }

            if (request.Capacity <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Capacity must be greater than 0."
                });
            }

            if (string.IsNullOrWhiteSpace(request.CenterName))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Written exam center is required."
                });
            }

            if (request.ScheduledDate == default)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Scheduled date is required."
                });
            }

            if (string.IsNullOrWhiteSpace(request.ScheduledTime))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Scheduled time is required."
                });
            }

            var result = await _physicalScheduleRepository.GenerateWrittenSchedule(request);

            return Ok(new
            {
                status = result.Success,
                message = result.Message,
                data = result
            });
        }

        [HttpPost]
        [Route("SearchGeneralWrittenScheduleApplications")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> SearchGeneralWrittenScheduleApplications([FromBody] GeneralWrittenScheduleSearchRequest request)
        {
            var result = await _physicalScheduleRepository.SearchGeneralWrittenScheduleApplications(request);

            return Ok(new
            {
                status = true,
                message = result.TotalCount > 0
                    ? "General written schedule applications found."
                    : "No records found.",
                data = result
            });
        }

        [HttpPost]
        [Route("GenerateGeneralWrittenSchedule")]
        [Authorize(Policy = AuthPolicies.SuperAdminOnly)]
        public async Task<IActionResult> GenerateGeneralWrittenSchedule([FromBody] GenerateGeneralWrittenScheduleRequest request)
        {
            var result = await _physicalScheduleRepository.GenerateGeneralWrittenSchedule(request);

            return Ok(new
            {
                status = result.Success,
                message = result.Message,
                data = result
            });
        }
    }
}