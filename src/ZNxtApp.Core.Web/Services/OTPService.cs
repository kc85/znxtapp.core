using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Web.Services
{
    public class OTPService : IOTPService
    {
        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailService;


        public OTPService(ILogger logger,
                          IDBService dbService,
                          ISMSService smsService, 
                          IEmailService emailService)
        {
            _logger = logger;
            _smsService = smsService;
            _dbService = dbService;
            _emailService = emailService;
        }

        public bool Validate(string phoneNumber,string otp, OTPType otpType, string securityToken= null)
        {
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter[CommonConst.CommonField.OTP] = otp;
            filter[CommonConst.CommonField.PHONE_FIELD] = phoneNumber;
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
            var otp = CommonUtility.RandomNumber(4);
            JObject otpData = new JObject();
            otpData[CommonConst.CommonField.ID] = CommonUtility.GetNewID();
            otpData[CommonConst.CommonField.OTP] = otp;
            otpData[CommonConst.CommonField.PHONE_FIELD] = phoneNumber;
            otpData[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;
            otpData[CommonConst.CommonField.OTP_TYPE] = otpType.ToString();
            otpData[CommonConst.CommonField.DURATION] = 15;
            otpData[CommonConst.CommonField.STATUS] = OTPStatus.New.ToString();
            if (_dbService.Write(CommonConst.Collection.OTPs, otpData))
            {
                return _smsService.Send(phoneNumber, smsTemplate, otpData, false);
            }
            else
            {
                return false;
            }
        }
    }
}
