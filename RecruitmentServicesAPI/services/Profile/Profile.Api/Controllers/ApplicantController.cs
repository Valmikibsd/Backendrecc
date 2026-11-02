using System;
using CrudOperations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;
using Shared.Auth;
using Shared.Kernel;

namespace Profile.Api.Controllers;

/// <summary>
/// Applicant profile APIs (get profile, photo/sign upload).
/// Additional Profile controllers can be added beside this file (same pattern as Auth).
/// </summary>
[Route("api/applicant")]
[ApiController]
[Authorize(Policy = AuthPolicies.ApplicantOnly)]
public class ApplicantController(IApplicantRepository applicantRepository) : BaseController
{
    private readonly IApplicantRepository _applicant = applicantRepository;

    [HttpGet]
    [Route("GetApplicantById")]
    public async Task<IActionResult> GetApplicantById()
    {
        long id = UserId;
        if (id <= 0)
        {
            return BadRequest(new
            {
                status = false,
                message = "Invalid UserId."
            });
        }
        var response = await _applicant.GetApplicantById(id);

        return response.Status
            ? Ok(response)
            : Conflict(response);
    }

    #region Photo-Sign

    [HttpPost("UploadPhotoSign")]
    public async Task<IActionResult> UploadDocument([FromForm] photosign obj)
    {
            long applicantId = UserId;
            if (obj.PhotoSign == null || obj.PhotoSign.Length == 0 || obj.documenttype == null)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Please select a file."
                });
            }

            if (applicantId <= 0)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid applicant."
                });
            }


            var allowedExtensions = new[] { ".jpg" };

            var extension = Path.GetExtension(obj.PhotoSign.FileName).ToLowerInvariant();
            string fileName = $"{applicantId}_{obj.PhotoSign.FileName}";

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Only JPG files are allowed.",
                });
            }

            using var memoryStream = new MemoryStream();

            await obj.PhotoSign.CopyToAsync(memoryStream);

            byte[] fileBytes = memoryStream.ToArray();


            var response = await _applicant.UploadPhotoSign(new photosign
            {
                userid = applicantId,
                FileName = fileName,
                File = fileBytes,
                FileType = extension,
                FileDetails = Convert.ToInt32(obj.documenttype),
                IPAddress = IpAddress
            });

            if (response.Status)
            {
                return StatusCode(StatusCodes.Status201Created, response);
            }
            else if (!response.Status)
            {
                return StatusCode(StatusCodes.Status409Conflict, response);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    "Something went wrong, Please contact system administrator");
            }
    }



    [HttpGet("GetPhotoSign")]
    public async Task<IActionResult> GetPhotoSign([FromQuery] int id)
    {
        long applicantId = UserId;

        if (applicantId <= 0)
        {
            return BadRequest(new
            {
                success = false,
                Message = "Invalid applicant."
            });
        }
        var response = await _applicant.GetPhotoSign(
            applicantId,
            id.ToString()
        );


        if (!response.Status || response.Data == null)
        {
            return Ok(new
            {
                success = true,
                data = (object?)null
            });
        }

        var photoSignData = response.Data;


        if (photoSignData.BinaryFiles == null ||
            photoSignData.BinaryFiles.Length == 0)
        {
            return Ok(new
            {
                success = true,
                data = (object?)null
            });
        }


        return Ok(response);
    }

    [HttpGet("IsProfileComplete")]
    public async Task<IActionResult> GetIsProfileComplete()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var isComplete = await _applicant.IsProfileComplete(UserId).ConfigureAwait(false);
        return Ok(new CrudOperations.Response<ProfileCompleteFlag>
        {
            Status = true,
            Data = new ProfileCompleteFlag { IsProfileComplete = isComplete ? 1 : 0 }
        });
    }

    [HttpPost("ConfirmProfile")]
    public async Task<IActionResult> ConfirmProfile()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var photo = await _applicant.GetPhotoSign(UserId, "1").ConfigureAwait(false);
        var signature = await _applicant.GetPhotoSign(UserId, "2").ConfigureAwait(false);

        if (photo.Data?.BinaryFiles == null || photo.Data.BinaryFiles.Length == 0
            || signature.Data?.BinaryFiles == null || signature.Data.BinaryFiles.Length == 0)
        {
            return Conflict(new CrudOperations.Response { Status = false, Message = "Please upload photo and signature." });
        }

        var response = await _applicant.SubmitCompleteProfile(UserId, Convert.ToInt32(UserId), IpAddress).ConfigureAwait(false);
        return response.Status ? Ok(response) : Conflict(response);
    }

    #endregion

    #region Address-Details
    

    [HttpPost("SaveAddress")]
    public async Task<IActionResult> SaveAddress([FromBody] AddressRequest obj)
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        obj.UserId = UserId;
        obj.IPAddress = IpAddress;
        var res = await _applicant.SaveAddress(obj);

        if (res == null)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    Status = false,
                    Message = "No response received."
                });
        }

        return Ok(res);
    }

    [HttpGet("GetAddress")]
    public async Task<IActionResult> GetAddress()
    {
        var invalidUser = InvalidUserResponse();
        if (invalidUser != null)
        {
            return invalidUser;
        }

        var response = await _applicant.GetAddress(UserId);

        if (response == null)
        {
            return NotFound(new
            {
                status = false,
                message = "Address Detail Not Found"
            });
        }

        return Ok(response);
    }

    #endregion




}
