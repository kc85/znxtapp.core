using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class RouteExecuter : IRouteExecuter
    {
         
        public void Exec(RoutingModel route,IHttpContextProxy httpProxy)
        {
            ILogger loggerController = Logger.GetLogger(route.ExecuteType, httpProxy.TransactionId);


            try
            {
                loggerController.Info(string.Format("{0}:: Route: [{1}]", "RouteExecuter.Exec", route.ToString()));
                IActionExecuter actionExecuter = new ActionExecuter(loggerController);
                ParamContainer pamamContainer = CreateParamContainer(route, httpProxy, loggerController, actionExecuter);
                WriteStartTransaction(loggerController, httpProxy, route);
                var objResult = actionExecuter.Exec(route, pamamContainer);
                httpProxy.ContentType = route.ContentType;

                if (objResult == null)
                {
                    httpProxy.SetResponse(CommonConst._500_SERVER_ERROR);
                }
                else if (objResult is byte[])
                {
                    WriteEndTransaction(loggerController, "*** Binary Data ***");
                    httpProxy.SetResponse(CommonConst._200_OK, (byte[])objResult);
                }
                else if (objResult is string)
                {
                    WriteEndTransaction(loggerController, (objResult as string));
                    httpProxy.SetResponse(CommonConst._200_OK, objResult as string);
                }
                else
                {
                    var responseData = (objResult as JObject).ToString();
                    WriteEndTransaction(loggerController, responseData);
                    httpProxy.SetResponse(CommonConst._200_OK, Encoding.UTF8.GetBytes(responseData));
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

        private void WriteStartTransaction(ILogger loggerController, IHttpContextProxy httpProxy, RoutingModel route)
        {
            JObject objTxnStartData = new JObject();
            objTxnStartData[CommonConst.CommonField.URL] = httpProxy.GetURIAbsolutePath();
            objTxnStartData[CommonConst.CommonField.ROUTE] = JObject.Parse(route.GetJson());
            string strPayload = httpProxy.GetRequestBody(); ;
            JObject payload = null;
            if (JObjectHelper.TryParseJson(strPayload, ref payload))
            {
                objTxnStartData[CommonConst.CommonField.PAYLOAD] = payload;
            }
            else
            {
                objTxnStartData[CommonConst.CommonField.PAYLOAD] = strPayload;
            }
            objTxnStartData[CommonConst.CommonField.USER] = httpProxy.GetRequestBody();
            loggerController.Transaction(objTxnStartData, TransactionState.Start);
        }
        private void WriteEndTransaction(ILogger loggerController, string response )
        {
            
            JObject objTxnStartData = new JObject();
            JObject payload = null;
            if (JObjectHelper.TryParseJson(response, ref payload))
            {
                payload.Remove(CommonConst.CommonField.HTTP_RESPONE_DEBUG_INFO);
                objTxnStartData[CommonConst.CommonField.PAYLOAD] = payload;
            }
            else
            {
                objTxnStartData[CommonConst.CommonField.PAYLOAD] = response;
            }
            loggerController.Transaction(objTxnStartData, TransactionState.Finish);
        }

        private ParamContainer CreateParamContainer(RoutingModel route, IHttpContextProxy httpProxy, ILogger loggerController, IActionExecuter actionExecuter)
        {

            ILogReader logReader = Logger.GetLogReader();
            ResponseBuilder responseBuilder = new ResponseBuilder(loggerController, logReader, httpProxy);
            IDBService dbService = new MongoDBService(ApplicationConfig.DataBaseName);
            IPingService pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.PING));
            ParamContainer pamamContainer = new ParamContainer();
            IAppSettingService appSettingService = AppSettingService.Instance;

            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_ROUTE, () => { return route; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_DBPROXY, () => { return dbService; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY, () => { return httpProxy; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_LOGGER, () => { return loggerController; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_ACTIONEXECUTER, () => { return actionExecuter; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_PING_SERVICE, () => { return pingService; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_PING_SERVICE, () => { return pingService; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_RESPONBUILDER, () => { return responseBuilder; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_APP_SETTING, () => { return appSettingService; });
           
            return pamamContainer;
        }

    }
}
