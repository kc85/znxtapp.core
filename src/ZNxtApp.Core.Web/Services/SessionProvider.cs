using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Proxies;

namespace ZNxtApp.Core.Web.Services
{
    public class SessionProvider : ISessionProvider
    {
        private IHttpContextProxy _httpProxy;
        private IDBService _dbProxy;
        private ILogger _logger;

        public SessionProvider(IHttpContextProxy httpProxy, IDBService dbProxy, ILogger logger)
        {
            _httpProxy = httpProxy;
            _logger = logger;
            _dbProxy = dbProxy;
        }

        public T GetValue<T>(string key)
        {
            var tempData = _httpProxy.GetTempValue<T>(key);
            if (tempData == null)
            {
                JObject filter = new JObject();
                filter[CommonConst.CommonField.SESSION_ID] = _httpProxy.SessionID;
                var sessionData = _dbProxy.Get(CommonConst.Collection.SESSION_DATA, filter.ToString(), new List<string> { key });

                if (sessionData.Count == 1 && sessionData[0][key] != null)
                {
                    var value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sessionData[0][key].ToString());
                    return value;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return tempData;
            }
        }

        public void ResetSession()
        {
            string filter = "{'" + CommonConst.CommonField.ID + "' : '" + _httpProxy.SessionID + "'}";
            try
            {
                _dbProxy.Delete(CommonConst.Collection.SESSION_DATA, filter);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                (_httpProxy as HttpContextProxy).ResetSession();
            }
        }

        public void SetValue<T>(string key, T value)
        {
            if (!string.IsNullOrEmpty(_httpProxy.SessionID))
            {
                JObject filter = new JObject();
                filter[CommonConst.CommonField.ID] = _httpProxy.SessionID;
                JObject sessionData = new JObject();
                sessionData[CommonConst.CommonField.ID] = _httpProxy.SessionID;
                sessionData[CommonConst.CommonField.SESSION_ID] = _httpProxy.SessionID;
                sessionData[key] = Newtonsoft.Json.JsonConvert.SerializeObject(value);

                _dbProxy.Update(CommonConst.Collection.SESSION_DATA, filter.ToString(), sessionData, true, MergeArrayHandling.Union);
            }
        }
    }
}