using CrudOperations;
using Profile.Domain.Applicant;

namespace Profile.Application.Interfaces;

public interface IApplicantRepository
{
    Task<Response<ApplicantCreation>> GetApplicantById(long id);
    Task<Response> UploadPhotoSign(photosign obj);
    Task<Response<PhotoSignData>> GetPhotoSign(long userid, string FileDetails);

   

    Task<Response> SaveAddress(AddressRequest obj);
    Task<Response<AddressRequest>> GetAddress(long userId);
    Task<Response> SubmitCompleteProfile(long userId, int updatedBy, string? ipAddress);
    Task<bool> IsProfileComplete(long userId);

    


}
