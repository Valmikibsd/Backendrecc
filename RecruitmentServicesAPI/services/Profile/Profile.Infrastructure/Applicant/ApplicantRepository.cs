using ClientsideEncryption;
using CrudOperations;
using Profile.Application.Interfaces;
using Profile.Domain.Applicant;

namespace Profile.Infrastructure.Applicant;

public class ApplicantRepository(ICrudOperationService crudOperationService) : IApplicantRepository
{
    private readonly ICrudOperationService _crudOperationService = crudOperationService;

    public async Task<Response<ApplicantCreation>> GetApplicantById(long id)
    {
        var response = await _crudOperationService
            .GetSingleRecord<ApplicantCreation>(
                "[dbo].[Usp_GetApplicantbyId]",
                new
                {
                    userid = id
                }, ConnectionNames.Registration);
        if (response != null && response.Data != null)
        {
            var applicant = response.Data as ApplicantCreation;
            // string base64Image = applicant.Image;

            applicant.Name = AESEncrytDecry.DecryptStringAES(applicant.Name);
            applicant.DOB = AESEncrytDecry.DecryptStringAES(applicant.DOB);
           // applicant.Mobile = AESEncrytDecry.DecryptStringAES(applicant.Mobile);
            applicant.Gender = AESEncrytDecry.DecryptStringAES(applicant.Gender);
            applicant.NameMarathi = AESEncrytDecry.DecryptStringAES(applicant.NameMarathi);
            applicant.AadhaarNo = MaskAadhaar(AESEncrytDecry.DecryptStringAES(applicant.AadhaarNo));

        }
        return response;
    }

    #region Photo-Sign
    public async Task<Response> UploadPhotoSign(photosign obj)
    {

        var res = await _crudOperationService
        .InsertUpdateDelete<Response>("[dbo].[USP_InsertPhotoSign]", new
        {
            obj.userid,
            obj.FileDetails,
            obj.FileType,
            obj.FileName,
            obj.File,
            obj.IPAddress
        }, ConnectionNames.Documents);

        return res;

    }

    public async Task<Response<PhotoSignData>> GetPhotoSign(long userid, string FileDetails)
    {
        var response = await _crudOperationService
            .GetSingleRecord<PhotoSignData>(
                "[dbo].[Usp_GetPhotoSign]",
                new
                {
                    userid,
                    FileDetails
                }, ConnectionNames.Documents);
        return response;
    }

    #endregion

   

    public async Task<Response> SaveAddress(
        AddressRequest obj)
    {
        try
        {
            var res = await _crudOperationService
                .InsertUpdateDelete<Response>(
                    "[dbo].[Usp_InsertUpdateAddressDetails]",
                    new
                    {
                        obj.UserId,

                        obj.PermanentAddress1,
                        obj.PermanentStateId,
                        obj.PermanentDistrictId,
                        obj.PermanentTalukaId,
                        obj.PermanentVillage,
                        obj.PermanentPinCode,

                        obj.IsPermanentSameasCorrespondence,

                        obj.CorrespondenceAddress1,
                        obj.CorrespondenceStateId,
                        obj.CorrespondenceDistrictId,
                        obj.CorrespondenceTalukaId,
                        obj.CorrespondenceVillage,
                        obj.CorrespondencePinCode,

                        obj.CreatedBy,
                        obj.IPAddress

                    },
                    ConnectionNames.Profile);

            return res;
        }
        catch (Exception ex)
        {
            throw;
           
        }
    }

    public async Task<Response<AddressRequest>> GetAddress(
    long userId)
    {
        var res = await _crudOperationService
            .GetSingleRecord<AddressRequest>(
                "[dbo].[Usp_GetAddressDetails]",
                new
                {
                    UserId = userId
                },
                ConnectionNames.Profile);

        return res;
    }

    public async Task<Response> SubmitCompleteProfile(long userId, int updatedBy, string? ipAddress)
    {
        return await _crudOperationService.InsertUpdateDelete<Response>(
            "[dbo].[SubmitCompleteProfile]",
            new
            {
                UserId = userId,
                UpdatedBy = updatedBy,
                IPAddress = ipAddress
            },
            ConnectionNames.Profile) ?? new Response { Status = false, Message = "Unable to confirm profile." };
    }

    public async Task<bool> IsProfileComplete(long userId)
    {
        var response = await _crudOperationService.GetSingleRecord<ProfileCompleteFlag>(
            "[dbo].[GetIsProfileComplete]",
            new { UserId = userId },
            ConnectionNames.Profile);

        return response?.Data != null && response.Data.IsProfileComplete == 1;
    }

    private string MaskAadhaar(string aadhaarNo)
    {
        if (string.IsNullOrEmpty(aadhaarNo) || aadhaarNo.Length < 4)
            return aadhaarNo;

        return "XXXX XXXX " + aadhaarNo.Substring(aadhaarNo.Length - 4);
    }
}
