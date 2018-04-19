using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using System;

namespace ZNxtApp.Core.Web.Services
{
    public class OTPService : IOTPService
    {
        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailService;
        private readonly IAppSettingService _appSettingService;


        public OTPService(ILogger logger,
                          IDBService dbService,
                          ISMSService smsService, 
                          IEmailService emailService,
                          IAppSettingService appSettingService)
        {
            _logger = logger;
            _smsService = smsService;
            _dbService = dbService;
            _emailService = emailService;
            _appSettingService = appSettingService;
        }

        public bool Validate(string phoneNumber,string otp, OTPType otpType, string securityToken= null)
        {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter[CommonConst.CommonField.OTP] = otp;
            filter[CommonConst.CommonField.PHONE] = phoneNumber;
            filter[CommonConst.CommonField.STATUS] = OTPStatus.New.ToString();
            filter[CommonConst.CommonField.OTP_TYPE] = otpType.ToString();

            var otpData = _dbService.FirstOrDefault(CommonConst.Collection.OTPs, filter);
            if (otpData != null)
            {
                otpData[CommonConst.CommonField.STATUS] = OTPStatus.Used.ToString();
                if (_dbService.Write(CommonConst.Collection.OTPs, otpData, filter))
                {
                    if (!string.IsNullOrEmpty(securityToken))
                    {
                        return otpData[CommonConst.CommonField.SECURITY_TOKEN].ToString() == securityToken;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    _logger.Error("Error updating OTP status on DB");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Send(string phoneNumber, string smsTemplate, OTPType otpType, string securityToken = null)
        {
            var otpData = CreateOTPData(otpType,securityToken);
            otpData[CommonConst.CommonField.PHONE] = phoneNumber;

            if (_dbService.Write(CommonConst.Collection.OTPs, otpData))
            {
                return _smsService.Send(phoneNumber, smsTemplate, otpData, false);
            }
            else
            {
                return false;
            }
        }

        private JObject CreateOTPData(OTPType otpType, string securityToken)
        {
            var otp = CommonUtility.RandomNumber(4);
            JObject otpData = new JObject();
            otpData[CommonConst.CommonField.ID] = CommonUtility.GetNewID();
            otpData[CommonConst.CommonField.OTP] = otp;
            otpData[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;
            otpData[CommonConst.CommonField.OTP_TYPE] = otpType.ToString();
            otpData[CommonConst.CommonField.DURATION] = 15;
            otpData[CommonConst.CommonField.STATUS] = OTPStatus.New.ToString();
            return otpData;

        }
        public bool SendEmail(string email, string emailTemplate, string subject, OTPType otpType, string securityToken)
        {
            var otpData = CreateOTPData(otpType, securityToken);
            otpData[CommonConst.CommonField.EMAIL] = email;

            if (_dbService.Write(CommonConst.Collection.OTPs, otpData))
            {
                List<string> to = new List<string>() { email };
                var fromEmail = _appSettingService.GetAppSettingData(CommonConst.CommonField.FROM_EMAIL_ID);
                return _emailService.Send(to, fromEmail, null, emailTemplate, subject, otpData);
            }
            else
            {
                return false;
            }
        }

        public bool ValidateEmail(string email, string otp, OTPType otpType, string securityToken)
        {
            _logger.Error("TODO ValidateEmail");
            return true;
        }
    }
}
