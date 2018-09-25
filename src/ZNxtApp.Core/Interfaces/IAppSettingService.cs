using Newtonsoft.Json.Linq;

namespace ZNxtApp.Core.Interfaces
{
    public interface IAppSettingService
    {
        JObject GetAppSetting(string key);

        JArray GetAppSettings();

        string GetAppSettingData(string key);

        void SetAppSetting(string key, JObject data, string module = null);
    }
}