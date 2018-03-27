using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.Services
{
    public class ActionExecuter : IActionExecuter
    {
        private ILogger _logger;
        private AssemblyLoader _assemblyLoader;
        public ActionExecuter(ILogger logger)
        {
            _logger = logger;
            _assemblyLoader = AssemblyLoader.GetAssemblyLoader();

        }
        public T Exec<T>(string action, IDBService dbProxy, ParamContainer helper)
        {
            var result = Exec(action, dbProxy, helper);
            return JObjectHelper.Deserialize<T>(result.ToString());
        }
        public object Exec(string action, IDBService dbProxy, ParamContainer helper)
        {
            dbProxy.Collection = CommonConst.Collection.SERVER_ROUTES;
            JObject filter = JObject.Parse(CommonConst.Filters.IS_OVERRIDE_FILTER);
            filter[CommonConst.CommonField.METHOD] = CommonConst.ActionMethods.ACTION;
            filter[CommonConst.CommonField.ROUTE] = action;

            var data = dbProxy.Get(filter.ToString());
            if (data.Count == 0)
            {
                throw new KeyNotFoundException(string.Format("Not Found: {0}", filter.ToString()));
            }
            RoutingModel route = JObjectHelper.Deserialize<RoutingModel>(data[0].ToString());

            Func<dynamic> routeAction  = () => { return route; };
            helper[CommonConst.CommonValue.PARAM_ROUTE] = routeAction;

            return Exec(route,helper);
        }

        public object Exec(RoutingModel route, ParamContainer helper)
        {
            return Exec(route.ExecultAssembly, route.ExecuteType, route.ExecuteMethod, helper);
        }

        public object Exec(string execultAssembly, string executeType, string executeMethod, ParamContainer helper)
        {
            Type exeType = _assemblyLoader.GetType(execultAssembly, executeType, _logger);
            if (exeType == null)
            {
                string error = string.Format("Execute Type is null for {0} :: {1}", execultAssembly, executeType);
                _logger.Error(error, null);
                throw new Exception(error);
            }
            Type[] argTypes = new Type[] { typeof(string) };
            object[] argValues = new object[] { helper };
            dynamic obj = Activator.CreateInstance(exeType, argValues);
            var methodInfo = exeType.GetMethod(executeMethod);
            if (methodInfo != null)
            {
                return methodInfo.Invoke(obj, null);
            }
            else
            {
                return null;
            }
        }
    }
}
