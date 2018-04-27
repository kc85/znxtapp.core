﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Interfaces
{
    public interface IOTPService
    {
        bool Send(string phoneNumber, string smsTemplate, OTPType otpType,string securityToken);
        bool SendEmail(string email, string smsTemplate,string subject, OTPType otpType, string securityToken);
        bool Validate(string phoneNumber,string otp, OTPType otpType,string securityToken);
        bool ValidateEmail(string email, string otp, OTPType otpType, string securityToken);
    }
}