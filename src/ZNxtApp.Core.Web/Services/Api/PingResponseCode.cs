using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services.Api
{
    public class PingResponseCode : IMessageCodeContainer
    {
        public const int _PING_FAIL = 5001; 
        private Dictionary<int, string> text = new Dictionary<int, string>();

        public string Get(int code)
        {
            if(text.ContainsKey(code))
            {
                return text[code];
            }
            else
            {
                return string.Empty;
            }
        }
        public PingResponseCode()
        {
            text[_PING_FAIL] = "PING_FAIL";
        }
    }
}
