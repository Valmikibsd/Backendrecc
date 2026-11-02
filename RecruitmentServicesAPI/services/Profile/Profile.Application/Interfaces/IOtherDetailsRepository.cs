using CrudOperations;
using Profile.Domain.Applicant;

namespace Profile.Application.Interfaces
{
    public interface IOtherDetailsRepository
    {
        Task<Response> SaveOtherDetails(OtherDetailsRequest obj);
        Task<Response<OtherDetailsRequest>> GetOtherDetails(long userId);
    }
}
