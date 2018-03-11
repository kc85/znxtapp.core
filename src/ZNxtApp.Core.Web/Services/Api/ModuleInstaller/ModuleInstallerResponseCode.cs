using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services.Api.ModuleInstaller
{
    public class ModuleInstallerResponseCode : IMessageCodeContainer
    {
        public const int _MAINTANCE_MODE_OFF = 6001;
        public const int _MODULE_NAME_EMPTY = 6002;
        public const int _MODULE_NOT_FOUND = 6003;
        public const int _MODULE_CONFIG_MISSING = 6004;
        public const int _MODULE_INSTALL_ERROR = 6005;

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
        public ModuleInstallerResponseCode()
        {
            text[_MAINTANCE_MODE_OFF] = "MAINTANCE_MODE_OFF";
            text[_MODULE_NAME_EMPTY] = "MODULE_NAME_EMPTY";
            text[_MODULE_NOT_FOUND] = "MODULE_NOT_FOUND";
            text[_MODULE_CONFIG_MISSING] = "MODULE_CONFIG_MISSING";
            text[_MODULE_INSTALL_ERROR] = "MODULE_INSTALL_ERROR";
        }
    }
}
