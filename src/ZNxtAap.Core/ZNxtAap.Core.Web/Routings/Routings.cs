using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Module;

namespace ZNxtAap.Core.Web.Routings
{
    public class Routings : IRoutings
    {
        private static object _lock = new object();

        private static Routings _routs;
        private List<RoutingModel> _routsModules;

        //private WebDBProxy _dbProxy;
        private ILogger _logger;

        private Routings()
        {
            //_logger = Logger.GetLogger(this.GetType().FullName);
            //_dbProxy = new WebDBProxy(_logger);
            ResetRouts();
        }

        public void ResetRouts()
        {
            _routsModules = new List<RoutingModel>();

            GetRoutsDataFromDB();
        }

        private void GetRoutsDataFromDB()
        {
            try
            {
                LoadRouts();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void LoadRouts()
        {
            //_routsModules = new List<RoutingModel>();
            //List<RoutingModel> dbRoutsModules = new List<RoutingModel>();
            //var dataResponse = _dbProxy.GetData(Common.CommonConsts.DB_SERVER_ROUTE_FIELD_NAME, CommonConsts.EMPTY_JSON_OBJECT);
            //if (dataResponse != null && dataResponse[CommonConsts.GETDATA_DATA_NODE_KEY] != null)
            //{
            //    if ((dataResponse[CommonConsts.GETDATA_DATA_NODE_KEY] as JArray).Count > 0)
            //    {
            //        foreach (JObject item in (dataResponse[CommonConsts.GETDATA_DATA_NODE_KEY] as JArray))
            //        {
            //            dbRoutsModules.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<RoutingModel>(item.ToString()));
            //        }
            //    }
            //}
            //if (dbRoutsModules.Count != 0)
            //{
            //    _routsModules = dbRoutsModules;
            //}
        }

        public static Routings GetRoutings()
        {
            if (_routs == null)
            {
                lock (_lock)
                {
                    _routs = new Routings();

                    return _routs;
                }
            }
            else
            {
                return _routs;
            }
        }

        public RoutingModel GetRoute(string Method, string url)
        {
            url = url.ToLower();
            var route = _routsModules.Where(f => f.Method == Method.ToUpper()).Where(f => url.LastIndexOf(f.Route.ToLower()) != -1 && url.LastIndexOf(f.Route.ToLower()) == (url.Length - f.Route.Length)).FirstOrDefault();
            if (route == null)
            {
                route = _routsModules.Where(f => f.Method == Method.ToUpper()).Where(f => url.IndexOf(f.Route.Replace("*", "").ToLower()) == 0).FirstOrDefault();
            }
            return route;
        }
    }
}
