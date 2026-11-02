using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using Shared.Auth;
using Shared.Kernel;

namespace Profile.Api.Controllers
{

    [Route("api/applicant/[controller]")]
    [ApiController]
    [Authorize(Policy = AuthPolicies.ApplicantOnly)]
    public class ViewProfileController(IViewProfileRepository viewProfileRepository) : BaseController

    {
        private readonly IViewProfileRepository _viewProfileRepository = viewProfileRepository;
       
        #region View -Profile
        [HttpGet("ViewProfile")]
        public async Task<IActionResult> ViewProfile()
        {
            


            if (UserId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    Message = "Invalid applicant."
                });
            }

            var response = await _viewProfileRepository.ViewProfile(
                UserId
            );
                      
            return Ok(response);
        }
        #endregion
    }
}
