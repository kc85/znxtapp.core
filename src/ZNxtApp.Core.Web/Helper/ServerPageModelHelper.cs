using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Services;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.Helper
{
    public static class ServerPageModelHelper
    {
        public static string ServerSidePageHandler(string requestUriPath, IDBService dbProxy, IHttpContextProxy httpProxy, IViewEngine viewEngine, IActionExecuter actionExecuter, ILogger logger, Dictionary<string, dynamic> pageModel = null)
        {
            var fi = new FileInfo(requestUriPath);
            var data = StaticContentHandler.GetStringContent(dbProxy, logger, requestUriPath);
            if (data != null)
            {
                if (pageModel == null)
                {
                    pageModel = new Dictionary<string, dynamic>();
                }
                var folderPath = requestUriPath.Replace(fi.Name, "");

                UpdateBaseModel(pageModel, requestUriPath, fi.Name);

                data = viewEngine.Compile(data, requestUriPath, ServerPageModelHelper.SetDefaultModel(dbProxy, httpProxy, logger, viewEngine, actionExecuter, pageModel, folderPath));
                if (pageModel.ContainsKey(CommonConst.CommonValue.PAGE_TEMPLATE_PATH))
                {
                    FileInfo fiTemplete = new FileInfo(pageModel[CommonConst.CommonValue.PAGE_TEMPLATE_PATH]);
                    var templateFileData = StaticContentHandler.GetStringContent(dbProxy, logger, pageModel[CommonConst.CommonValue.PAGE_TEMPLATE_PATH]);
                    pageModel[CommonConst.CommonValue.RENDERBODY_DATA] = data;
                    data = viewEngine.Compile(templateFileData, pageModel[CommonConst.CommonValue.PAGE_TEMPLATE_PATH],
                        ServerPageModelHelper.SetDefaultModel(dbProxy, httpProxy, logger, viewEngine, actionExecuter, pageModel, pageModel[CommonConst.CommonValue.PAGE_TEMPLATE_PATH].Replace(fiTemplete.Name, "")));
                }
                return data;
            }
            else
            {
                return string.Empty;
            }
        }
        private static void UpdateBaseModel(Dictionary<string, dynamic> pageModel, string requestUriPath, string pageName)
        {
            var uri = StaticContentHandler.UnmappedUriPath(requestUriPath);
            if (StaticContentHandler.IsAdminPage(requestUriPath))
            {
                pageModel[CommonConst.CommonField.BASE_URI] = string.Format("{0}{1}", ApplicationConfig.AppPath, ApplicationConfig.AppBackendPath);
            }
            else
            {
                pageModel[CommonConst.CommonField.BASE_URI] = ApplicationConfig.AppPath;
            }
            pageModel[CommonConst.CommonField.PAGE_NAME] = pageName;
            pageModel[CommonConst.CommonField.URI] = uri;
            AddBaseData(pageModel);

        }
        private static Dictionary<string, dynamic> SetDefaultModel(IDBService dbProxy, IHttpContextProxy httpProxy, ILogger logger, IViewEngine viewEngine, IActionExecuter actionExecuter, Dictionary<string, dynamic> model, string folderPath = null)
        {
            ISessionProvider sessionProvider = new SessionProvider(httpProxy, dbProxy, logger);

            if (model == null)
            {
                model = new Dictionary<string, dynamic>();
            }
            model[CommonConst.CommonValue.METHODS] = new Dictionary<string, dynamic>();

            Func<string, string, JArray> getData =
                (string collection, string filter) =>
            {
                dbProxy.Collection = collection;
                return dbProxy.Get(filter);

            };
            Func<string, string> getAppSetting =
               (string key) =>
               {
                   var response = AppSettingService.Instance.GetAppSettingData(key);
                   if (string.IsNullOrEmpty(response))
                   {
                       response = ConfigurationManager.AppSettings[key];
                   }
                 return response;

               };
            Func<string, JObject> getSessionValue =
               (string key) =>
               {
                   return sessionProvider.GetValue<JObject>(key);

               };
            Func<string, string> includeTemplete = (string templatePath) =>
            {

                FileInfo fi = new FileInfo(string.Format("c:\\{0}{1}", folderPath, templatePath));
                string path = fi.FullName.Replace("c:", "");
                model[CommonConst.CommonValue.PAGE_TEMPLATE_PATH] = path;
                return string.Empty;
            };
            Func<string, bool> authorized = (string authGroups) =>
            {
                var sessionUser = sessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (sessionUser == null)
                {
                    return false;
                }

                if (!authGroups.Split(',').Where(i => sessionUser.groups.Contains(i)).Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            };
            Func<string, JObject, JObject> ActionExecute =
               (string actionPath, JObject data) =>
               {
                   var param = ActionExecuterHelper.CreateParamContainer(null, httpProxy, logger, actionExecuter);

                   if (data != null)
                   {
                       foreach (var item in data)
                       {
                           Func<dynamic> funcValue = () => { return item.Value; };
                           param.AddKey(item.Key, funcValue);
                       }
                   }
                   return actionExecuter.Exec<JObject>(actionPath, dbProxy, param);

               };

            Func<string, JObject, Dictionary<string, dynamic>> IncludeModel =
               (string includeModelPath, JObject data) =>
               {
                   try
                   {
                       
                       var param = ActionExecuterHelper.CreateParamContainer(null, httpProxy, logger, actionExecuter);

                       Dictionary<string, dynamic> modelData = new Dictionary<string, dynamic>();

                       if (data != null)
                       {
                           foreach (var item in data)
                           {
                               Func<dynamic> funcValue = () => { return item.Value; };
                               param.AddKey(item.Key, funcValue);
                           }
                       }

                       object response = actionExecuter.Exec(includeModelPath, dbProxy, param);
                       if (response is Dictionary<string, dynamic>)
                       {

                           return response as Dictionary<string, dynamic>;
                       }
                       else
                       {
                           throw new InvalidCastException(string.Format("Invalid respone from {0}", includeModelPath));
                       }
                   }
                   catch (UnauthorizedAccessException ex)
                   {
                       logger.Error(string.Format("Error While executing Route : {0}, Error : {1}", includeModelPath, ex.Message), ex);
                       throw;
                   }
               };
            model[CommonConst.CommonValue.METHODS]["IncludeModel"] = IncludeModel;

            model[CommonConst.CommonValue.METHODS]["ExecuteAction"] = ActionExecute;

            model[CommonConst.CommonValue.METHODS]["InclueTemplate"] = includeTemplete;

            model[CommonConst.CommonValue.METHODS]["GetData"] = getData;

            Func<JObject> requestBody = () => httpProxy.GetRequestBody<JObject>();
            model[CommonConst.CommonValue.METHODS]["RequestBody"] = requestBody;

            Func<string, string> queryString = (string key) => httpProxy.GetQueryString(key);
            model[CommonConst.CommonValue.METHODS]["QueryString"] = queryString;

            model[CommonConst.CommonValue.METHODS]["AppSetting"] = getAppSetting;

            model[CommonConst.CommonValue.METHODS]["GetSessionData"] = getSessionValue;

            model[CommonConst.CommonValue.METHODS]["Authorized"] = authorized;


            Func<string, JObject, string> includeBlock =
                (string blockPath, JObject blockModel) =>
                {
                    var inputBlockModel = new Dictionary<string, dynamic>();
                    if (blockModel != null)
                    {
                        foreach (var item in blockModel)
                        {
                            inputBlockModel[item.Key] = item.Value;
                        }
                    }
                    if (model != null)
                    {
                        foreach (var item in model)
                        {
                            inputBlockModel[item.Key] = item.Value;
                        }

                    }
                    FileInfo fi = new FileInfo(string.Format("c:\\{0}{1}", folderPath, blockPath));
                    string path = fi.FullName.Replace("c:", "");
                    var data = StaticContentHandler.GetStringContent(dbProxy, logger, path);
                    data = viewEngine.Compile(data, path, ServerPageModelHelper.SetDefaultModel(dbProxy, httpProxy, logger, viewEngine, actionExecuter, inputBlockModel, path.Replace(fi.Name, "")));
                    return data;
                };
            model[CommonConst.CommonValue.METHODS]["Include"] = includeBlock;

            Func<string> randerBody = () =>
            {
                if (model.ContainsKey(CommonConst.CommonValue.RENDERBODY_DATA))
                {
                    return model[CommonConst.CommonValue.RENDERBODY_DATA];
                }
                else
                {
                    return string.Empty;
                }
            };

            model[CommonConst.CommonValue.METHODS]["RenderBody"] = randerBody;

            return model;
        }

        public static void AddBaseData(Dictionary<string,dynamic> baseData)
        {

            baseData[CommonConst.CommonField.APP_NAME] = ApplicationConfig.AppName;
        }
    }
}
