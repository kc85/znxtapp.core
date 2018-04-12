using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Module.App.Services.Api
{
    public class AppResponseCode : IMessageCodeContainer
    {
        public const int _CAPTCHA_VALIDATION_FAIL = 8001;
        public const int _USER_EXISTS = 8002;
        public const int _OTP_SEND_ERROR = 8003;
        private Dictionary<int, string> text = new Dictionary<int, string>();

        public string Get(int code)
        {
            if (text.ContainsKey(code))
            {
                return text[code];
            }
            else
            {
                return string.Empty;
            }
        }
        public AppResponseCode()
        {
            text[_CAPTCHA_VALIDATION_FAIL] = "CAPTCHA_VALIDATION_FAIL";
            text[_USER_EXISTS] = "USER_EXISTS";
            text[_OTP_SEND_ERROR] = "OTP_SEND_ERROR";

        }
    }
}
