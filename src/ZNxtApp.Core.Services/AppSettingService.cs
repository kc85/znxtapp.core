using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Linq;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Services
{
    public class AppSettingService : IAppSettingService
    {
        private static AppSettingService _appSettings;

        private JArray _settings;
        private static object _lockObj = new object();
        private IDBService _dbService;

        private AppSettingService()
        {
            _dbService = ApplicationConfig.DependencyResolver.GetInstance<IDBService>();
        }

        public static AppSettingService Instance
        {
            get
            {
                if (_appSettings == null)
                {
                    lock (_lockObj)
                    {
                        _appSettings = new AppSettingService();
                    }
                }
                return _appSettings;
            }
        }

        public void ReloadSettings(bool forceReload = false)
        {
            if (_settings == null || forceReload)
            {
                lock (_lockObj)
                {
                    _settings = _dbService.Get(CommonConst.Collection.APP_SETTING, CommonConst.Filters.IS_OVERRIDE_FILTER);
                }
            }
        }

        public JObject GetAppSetting(string key)
        {
            ReloadSettings();
            return _settings.FirstOrDefault(f => f[CommonConst.CommonField.DATA_KEY].ToString() == key) as JObject;
        }

        public void SetAppSetting(string key, JObject data, string module = null)
        {
            lock (_lockObj)
            {
                string filter = "{" + CommonConst.CommonField.DATA_KEY + " : '" + key + "'}";
                JObject setting = new JObject();
                setting[CommonConst.CommonField.DATA_KEY] = key;
                setting[CommonConst.CommonField.DISPLAY_ID] = Guid.NewGuid().ToString();
                setting[CommonConst.CommonField.DATA] = data;
                setting[CommonConst.CommonField.ÌS_OVERRIDE] = false;
                setting[CommonConst.CommonField.OVERRIDE_BY] = CommonConst.CommonValue.NONE;
                setting[CommonConst.CommonField.MODULE_NAME] = module;
                var dbresponse = _dbService.Update(CommonConst.Collection.APP_SETTING, filter, setting, true);
                _settings = null;
                ReloadSettings();
            }
        }

        public string GetAppSettingData(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key];
            }

            var data = GetAppSetting(key);
            if (data != null && data[CommonConst.CommonField.DATA] != null)
            {
                return data[CommonConst.CommonField.DATA].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public JArray GetAppSettings()
        {
            ReloadSettings();
            return _settings;
        }
    }
}