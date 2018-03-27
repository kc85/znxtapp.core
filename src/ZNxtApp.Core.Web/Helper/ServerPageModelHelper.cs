using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
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
            pageModel[CommonConst.CommonField.APP_NAME] = ApplicationConfig.AppName;

        }
        private static Dictionary<string, dynamic> SetDefaultModel(IDBService dbProxy, IHttpContextProxy httpProxy, ILogger logger, IViewEngine viewEngine, IActionExecuter actionExecuter, Dictionary<string, dynamic> model, string folderPath = null)
        {
            if (model == null)
            {
                model = new Dictionary<string, dynamic>();
            }
            model["Methods"] = new Dictionary<string, dynamic>();

            Func<string, string, JArray> getData =
                (string collection, string filter) =>
            {
                dbProxy.Collection = collection;
                return dbProxy.Get(filter);

            };
            Func<string, string> includeTemplete = (string templatePath) =>
            {

                FileInfo fi = new FileInfo(string.Format("c:\\{0}{1}", folderPath, templatePath));
                string path = fi.FullName.Replace("c:", "");
                model[CommonConst.CommonValue.PAGE_TEMPLATE_PATH] = path;
                return string.Empty;
            };
            Func<string, JObject, JObject> ActionExecute =
               (string actionPath, JObject data) =>
               {
                   var param = ActionExecuterHelper.CreateParamContainer(null, httpProxy, logger, actionExecuter);
                   return actionExecuter.Exec<JObject>(actionPath, dbProxy, param);

               };
            model[CommonConst.CommonValue.METHODS]["Execute"] = ActionExecute;

            model[CommonConst.CommonValue.METHODS]["InclueTemplate"] = includeTemplete;

            model[CommonConst.CommonValue.METHODS]["GetData"] = getData;

            Func<JObject> requestBody = () => httpProxy.GetRequestBody<JObject>();
            model[CommonConst.CommonValue.METHODS]["RequestBody"] = requestBody;

            Func<string, string> queryString = (string key) => httpProxy.GetQueryString(key);
            model[CommonConst.CommonValue.METHODS]["QueryString"] = queryString;

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

    }
}
