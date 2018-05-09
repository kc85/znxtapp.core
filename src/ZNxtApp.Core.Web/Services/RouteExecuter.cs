using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Helper;
using ZNxtApp.Core.Web.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class RouteExecuter : IRouteExecuter
    {
         
        public void Exec(RoutingModel route,IHttpContextProxy httpProxy)
        {
            ILogger loggerController = Logger.GetLogger(route.ExecuteType, httpProxy.TransactionId);
            RouteEventHandler routeEventHandler = new RouteEventHandler();
            
            try
            {
                
                loggerController.Info(string.Format("{0}:: Route: [{1}]", "RouteExecuter.Exec", route.ToString()));
                IActionExecuter actionExecuter = new ActionExecuter(loggerController);
                ParamContainer paramContainer = ActionExecuterHelper.CreateParamContainer(route, httpProxy, loggerController, actionExecuter);
                WriteStartTransaction(loggerController, httpProxy, route);
                // Execute before Events 
                routeEventHandler.ExecBeforeEvent(actionExecuter, route, paramContainer);
                
                var objResult = actionExecuter.Exec(route, paramContainer);
                httpProxy.ContentType = route.ContentType;

                // Add response in param 
                paramContainer.AddKey(CommonConst.CommonValue.PARAM_API_RESPONSE, () => { return objResult; });
               
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
                    httpProxy.SetResponse(CommonConst._200_OK, responseData);
                }

                // Execute after Events 
                routeEventHandler.ExecAfterEvent(actionExecuter, route, paramContainer, objResult);

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
            catch (UnauthorizedAccessException ex)
            {
                loggerController.Error(string.Format("Error While executing Route : {0}, Error : {1}", route.ToString(), ex.Message), ex);
                httpProxy.SetResponse(CommonConst._401_UNAUTHORIZED);
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
    }
}
