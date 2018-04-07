using Newtonsoft.Json.Linq;
using System;
namespace ZNxtApp.Core.Interfaces
{
    public interface IAppSettingService
    {
        JObject GetAppSetting(string key);
        JArray GetAppSettings();
        string GetAppSettingData(string key);
        void SetAppSetting(string key, JObject data,string module = null);
    }
}
