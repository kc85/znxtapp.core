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
            string filter = "{'" + CommonConst.CommonField.SESSION_ID + "' : '" + _httpProxy.SessionID + "','" + CommonConst.CommonField.DATA_KEY + "':'" + key + "'}";
            
            var sessionData = _dbProxy.Get(CommonConst.Collection.SESSION_DATA,filter);
            if (sessionData.Count == 1)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sessionData[0][CommonConst.CommonField.DATA].ToString());
            }
            else
            {
                return default(T);
            }
        }

        public void ResetSession()
        {
            string filter = "{'" + CommonConst.CommonField.SESSION_ID + "' : '" + _httpProxy.SessionID + "'}";
            
            _dbProxy.Delete(CommonConst.Collection.SESSION_DATA,filter);
            (_httpProxy as HttpContextProxy).ResetSession();
        }

        public void SetValue<T>(string key, T value)
        {
            string filter = "{'" + CommonConst.CommonField.SESSION_ID + "' : '" + _httpProxy.SessionID + "','" + CommonConst.CommonField.DATA_KEY + "':'" + key + "'}";
            
            JObject sessionData = new JObject();
            sessionData[CommonConst.CommonField.SESSION_ID] = _httpProxy.SessionID;
            sessionData[CommonConst.CommonField.DATA_KEY] = key;
            sessionData[CommonConst.CommonField.DATA] = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            _dbProxy.Update(CommonConst.Collection.SESSION_DATA,filter, sessionData, true);
        }
    }
}
