using CrudOperations;
using Profile.Domain.Applicant;


namespace Profile.Application.Interfaces
{
    public interface IViewProfileRepository
    {
        //Added By K.P on 14.08.2026
        // Task<Response<ViewProfileModel>> ViewProfile(long userid);

        //Task<ResponseList<ViewProfileModel>> ViewProfile(long userid);
        Task<Response<ApplicantViewProfileResponse>> ViewProfile(long userid);
    }
}
