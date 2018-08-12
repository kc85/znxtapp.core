using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Proxies;

namespace ZNxtApp.Core.Web.Services
{
    public class SessionProvider :ISessionProvider
    {
        private IHttpContextProxy _httpProxy;
        private IDBService _dbProxy;
        private ILogger _logger;

        public SessionProvider(IHttpContextProxy httpProxy, IDBService dbProxy, ILogger logger )
        {
            _httpProxy = httpProxy;
            _logger = logger;
            _dbProxy = dbProxy;

        }
        public T GetValue<T>(string key)
        {
            JObject filter = new JObject();
            filter[CommonConst.CommonField.SESSION_ID] = _httpProxy.SessionID;
            var sessionData = _dbProxy.Get(CommonConst.Collection.SESSION_DATA,filter.ToString(), new List<string> { key });

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

        public void ResetSession()
        {
            string filter = "{'" + CommonConst.CommonField.SESSION_ID + "' : '" + _httpProxy.SessionID + "'}";
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
            JObject filter = new JObject();
            filter[CommonConst.CommonField.SESSION_ID] = _httpProxy.SessionID;
            JObject sessionData = new JObject();
            sessionData[CommonConst.CommonField.SESSION_ID] = _httpProxy.SessionID;
            sessionData[key] = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _dbProxy.Update(CommonConst.Collection.SESSION_DATA,filter.ToString(), sessionData, true, MergeArrayHandling.Union);
        }
    }
}
