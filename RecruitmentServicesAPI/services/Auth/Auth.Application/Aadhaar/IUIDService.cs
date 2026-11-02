using CrudOperations;
using RecruitmentPortal.Core.Aadhaar;

namespace Auth.Application.Interfaces;

public interface IUIDService
{
    Task<AadhaarRefrenceResponseModel> GetAadhaarReferenceNumberbyUID(string aadharNumber);
    Task<AadhaarResponseModel> GetUIDbyReferenceNumber(string aadharRefrenceNumber);
    Task<Response> CheckAadhaarExists(DtoAadhaar model);
    Task<AadharOTPResponse> SendOtpAsync(string aadharNumber);
    Task<AadharVerifyOTPResponse> VerifyOTPAsync(string aadharNumber, string OTP, string OTPTxn);
    Task<Response> InsertAadharLogDetails(AadhaarServiceLogModel aadhaarServiceLogModel);
    Task<Response<DtoAadhaarProfile>> GetAadhaarDetailsAsync(DtoAadhaar model);
}
