using ZNxtApp.Core.Enums;

namespace ZNxtApp.Core.Interfaces
{
    public interface IOTPService
    {
        bool Send(string phoneNumber, string smsTemplate, OTPType otpType, string securityToken);

        bool SendEmail(string email, string smsTemplate, string subject, OTPType otpType, string securityToken);

        bool Validate(string phoneNumber, string otp, OTPType otpType, string securityToken);

        bool ValidateEmail(string email, string otp, OTPType otpType, string securityToken);
    }
}