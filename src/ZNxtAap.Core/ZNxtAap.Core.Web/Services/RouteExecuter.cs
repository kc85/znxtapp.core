using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class RouteExecuter : IRouteExecuter
    {
         
        public void Exec(RoutingModel route,IHttpContextProxy httpProxy)
        {
            ILogger loggerController = Logger.GetLogger(route.ExecuteType);

            try
            {
                IDBService dbService = new MongoDBService(ApplicationConfig.DataBaseName);
                IActionExecuter actionExecuter = new ActionExecuter(loggerController);
                loggerController.Info(string.Format("{0}::{1}", "RouteExecuter.Exec", route.ToString()));

                ParamContainer pamamContainer = new ParamContainer();
                pamamContainer.AddKey(CommonConst.CommonValue.PARAM_ROUTE, () => { return route; });
                pamamContainer.AddKey(CommonConst.CommonValue.PARAM_DBPROXY, () => { return dbService; });
                pamamContainer.AddKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY, () => { return httpProxy; });
                pamamContainer.AddKey(CommonConst.CommonValue.PARAM_LOGGER, () => { return loggerController; });
                pamamContainer.AddKey(CommonConst.CommonValue.PARAM_ACTIONEXECUTER, () => { return actionExecuter; });

                var objResult = actionExecuter.Exec(route, pamamContainer);
                httpProxy.ContentType = route.ContentType;

                if (objResult == null)
                {
                    httpProxy.SetResponse(CommonConst._500_SERVER_ERROR);
                }
                else if (objResult is byte[])
                {
                    httpProxy.SetResponse(CommonConst._200_OK, (byte[])objResult);
                }
                else if (objResult is string)
                {
                    httpProxy.SetResponse(CommonConst._200_OK, objResult as string);
                }
                else
                {
                    httpProxy.SetResponse(CommonConst._200_OK, Encoding.UTF8.GetBytes((objResult as JObject).ToString()));
                }

                //    List<string> userGroups = _httpProxy.GetSessionUserGroups();
                //    if (route.auth_users.Count == 0 || userGroups.Intersect(route.auth_users).Any())
                //    {
                //        var objResult = routeExecuter.Exec(route, helper);
                //        if (objResult == null)
                //        {
                //            SetInternalServerError();
                //            return null;
                //        }
                //        else if (objResult is byte[])
                //        {
                //            return (byte[])objResult;
                //        }
                //        else
                //        {
                //            return Encoding.UTF8.GetBytes((objResult as JObject).ToString());
                //        }
                //    }
                //    else
                //    {
                //        SetUnauthorized();
                //        return Encoding.UTF8.GetBytes((ZApp.Common.HttpUtility.GetFullResponse((int)ResponseCodes.Unauthorized, ResponseCodes.Unauthorized.ToString(), null)).ToString());
                //    }

            }
            catch (Exception ex)
            {
                httpProxy.SetResponse(CommonConst._500_SERVER_ERROR);
                loggerController.Error(string.Format("Error While executing Route : {0}, Error : {1}", route.ToString(), ex.Message), ex);
            }
        }

    }
}
