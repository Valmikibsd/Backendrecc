namespace Shared.Kernel.Helpers;

public static class SMSMessageHelper
{
    public const string CommonOtp =
        "OTP for registration is {#MobileOTP#}. OTP is valid for {#var#} minutes. MSSIDC - MAHGOV";

    public const string RegistrationOtp =
        "OTP for registration is {#MobileOTP#}. OTP is valid for {#var#} minutes. MSSIDC - MAHGOV";

    public const string LoginOtp =
    "Your OTP for login is {#MobileOTP#}. OTP is valid for {#var#} minutes. MSSIDC - MAHGOV";

    public const string ApplicationCancellationOtp =
        "Your OTP for application cancellation of application number {#ApplicationNo#} is {#MobileOTP#}. OTP is valid for {#var#} minutes. MSSIDC - MAHGOV";

    public const string ApplicationCancellation =
        "Your application {#ApplicationNo#} has been cancelled successfully. MSSIDC - MAHGOV";
}

public static class SMSTemplateIDHelper
{
    public const string CommonOtpTempID = "1307175206532471945";

    public const string RegistrationOtpTempID = "REPLACE_REGISTRATION_OTP_TEMPLATE_ID";

    public const string LoginOtpTempID =
    "REPLACE_LOGIN_OTP_TEMPLATE_ID";

    public const string ApplicationCancellationOtpTempID = "REPLACE_CANCELLATION_OTP_TEMPLATE_ID";

    public const string ApplicationCancellationTempID = "REPLACE_APPLICATION_CANCELLATION_TEMPLATE_ID";
}
