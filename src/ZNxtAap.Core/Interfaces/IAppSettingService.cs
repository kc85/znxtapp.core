using Newtonsoft.Json.Linq;
using System;
namespace ZNxtAap.Core.Interfaces
{
    public interface IAppSettingService
    {
        JObject GetAppSetting(string key);
        JArray GetAppSettings();
        void SetAppSetting(string key, JObject data,string module = null);
    }
}
