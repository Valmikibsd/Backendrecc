using CrudOperations;
using RecruitmentPortal.Core.Registration;

namespace Auth.Application.Interfaces;

public interface IRegistrationRepository
{
    Task<Response> SaveRegistration(DtoRegistration? dtoRegistration);
    Task<Response> VerifyEmailAndMobileOtp(DtoEmailAndMobileOTP dto);
    Task<Response> SendEmailAndMobileOtp(DtoEmailAndMobileOTP dto);
}
