using System;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Services;

namespace ZNxtApp.Core.Web.Routings
{
    public class Routings : IRoutings
    {
        private static object _lock = new object();

        private static Routings _routs;
        private List<RoutingModel> _routsModules;

        private IDBService _dbProxy;
        private ILogger _logger;

        private Routings()
        {
            _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            _logger = Logger.GetLogger(this.GetType().Name, string.Empty);
            LoadRoutes();
        }

        public void LoadRoutes()
        {
            try
            {
                _routsModules = new List<RoutingModel>();

                var dataResponse = _dbProxy.Get(CommonConst.Collection.SERVER_ROUTES, CommonConst.Filters.IS_OVERRIDE_FILTER);
                foreach (var item in dataResponse)
                {
                    _routsModules.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<RoutingModel>(item.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
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