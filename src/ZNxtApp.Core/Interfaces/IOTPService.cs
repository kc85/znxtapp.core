using Newtonsoft.Json.Linq;
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
        bool Send(string phoneNumber, string smsTemplate, OTPType otpType);
        OTPData Read(string phoneNumber, OTPType otpType);
    }
}
