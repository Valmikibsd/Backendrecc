using Shared.Kernel.Interfaces;
using Auth.Application.Interfaces;
using ClientsideEncryption;
using CrudOperations;
using RecruitmentPortal.Core.Account;
using RecruitmentPortal.Core.Registration;
using RecruitmentPortal.InfraHelper;
using Shared.Kernel.Helpers;

namespace RecruitmentPortal.Infrastructure.Registration;

public class RegistrationRepository(
    ITokenService tokenService,
    IAuthLoginRepository authLogin,
    ICrudOperationService crudOperationService,
    ISmsService smsService,
    IEmailService emailService,
    WebHelper webHelper) : IRegistrationRepository
{
    private readonly ICrudOperationService _crudOperationService = crudOperationService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IAuthLoginRepository _authLogin = authLogin;
    private readonly ISmsService _sms = smsService;
    private readonly IEmailService _email = emailService;
    private readonly WebHelper _webHelper = webHelper;

    public async Task<Response> SaveRegistration(DtoRegistration? dtoRegistration)
    {
        dtoRegistration ??= new DtoRegistration();

        if (!string.IsNullOrEmpty(dtoRegistration.Password))
        {
            dtoRegistration.Password = AESEncrytDecry.EncryptStringAES(dtoRegistration.Password);
        }

        dtoRegistration.IPAddress = _webHelper.GetCurrentIpAddress();
        var res = await _crudOperationService.InsertUpdateDelete<Response>(
            "[dbo].[ManageRegistrationReg]",
            new
            {
                Type = 1,
                dtoRegistration.UserId,
                dtoRegistration.Username,
                dtoRegistration.Password,
                dtoRegistration.EmailId,
                dtoRegistration.MobileNo,
                dtoRegistration.AlternateMobileNo,
                dtoRegistration.IsWhatsapp,
                dtoRegistration.IPAddress
            },
            ConnectionNames.Registration).ConfigureAwait(false);

        if (res == null)
        {
            return new Response
            {
                Status = false,
                Message = "Registration failed."
            };
        }

        return res;
    }

    public async Task<Response> SendEmailAndMobileOtp(DtoEmailAndMobileOTP dtoEmailAndMobileOTP)
    {
        var otpMinutes = await _authLogin.GetOtpExpiryMinutes().ConfigureAwait(false);
        dtoEmailAndMobileOTP.OTP = _tokenService.GenerateOtp(6);

        var otpRequest = new DtoSendAuthOtp
        {
            Email = dtoEmailAndMobileOTP.Email,
            Mobile = dtoEmailAndMobileOTP.Mobile,
            Type = dtoEmailAndMobileOTP.Type,
            Purpose = "registration"
        };

        if (dtoEmailAndMobileOTP.Type?.ToLower() == "email")
        {
            var response = await _authLogin.SendAuthOtp(otpRequest, dtoEmailAndMobileOTP.OTP).ConfigureAwait(false);
            if (!response.Status)
            {
                return response;
            }

            string body;
            using (var reader = new StreamReader(Path.Combine("Providers", "ContentFiles", "EmailTemplate", "RegistrationOtpMail.html")))
            {
                body = await reader.ReadToEndAsync();
            }

            body = body.Replace("{{otp}}", dtoEmailAndMobileOTP.OTP)
                       .Replace("{{EXPIRY_TIME}}", otpMinutes.ToString());

            var emailResponse = await _email.SendMail(
                dtoEmailAndMobileOTP.Email ?? "",
                "Registration OTP",
                body,
                true);

            response.Status = emailResponse.Status;
            response.Message = emailResponse.Status
                ? "Email OTP sent successfully."
                : emailResponse.Message;

            return response;
        }

        if (dtoEmailAndMobileOTP.Type?.ToLower() == "mobile")
        {
            var response = await _authLogin.SendAuthOtp(otpRequest, dtoEmailAndMobileOTP.OTP).ConfigureAwait(false);
            if (!response.Status)
            {
                return response;
            }

            // REPLACE: Use RegistrationOtp template when separate template is configured in DLT
            // var message = SMSMessageHelper.RegistrationOtp
            //     .Replace("{#MobileOTP#}", dtoEmailAndMobileOTP.OTP)
            //     .Replace("{#var#}", otpMinutes.ToString());
            // var smsResponse = await _sms.SendAsync(
            //     dtoEmailAndMobileOTP.Mobile ?? "",
            //     message,
            //     SMSTemplateIDHelper.RegistrationOtpTempID);

            var message = SMSMessageHelper.CommonOtp
                .Replace("{#MobileOTP#}", dtoEmailAndMobileOTP.OTP)
                .Replace("{#var#}", otpMinutes.ToString());

            var smsResponse = await _sms.SendAsync(
                dtoEmailAndMobileOTP.Mobile ?? "",
                message,
                SMSTemplateIDHelper.CommonOtpTempID);

            response.Status = smsResponse.Status;
            response.Message = smsResponse.Status
                ? "Mobile OTP sent successfully."
                : smsResponse.Message;

            return response;
        }

        return new Response
        {
            Status = false,
            Message = "Invalid OTP type."
        };
    }

    public Task<Response> VerifyEmailAndMobileOtp(DtoEmailAndMobileOTP dtoEmailAndMobileOTP)
    {
        return _authLogin.VerifyAuthOtp(
            new DtoSendAuthOtp
            {
                Email = dtoEmailAndMobileOTP.Email,
                Mobile = dtoEmailAndMobileOTP.Mobile,
                Type = dtoEmailAndMobileOTP.Type,
                Purpose = "registration"
            },
            dtoEmailAndMobileOTP.OTP ?? "");
    }
}
