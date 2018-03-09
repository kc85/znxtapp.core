using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.Helper
{
    public static class ServerPageModelHelper
    {
        public static Dictionary<string, dynamic> SetDefaultModel(IDBService dbProxy, IHttpContextProxy httpProxy, ILogger logger, IViewEngine viewEngine, Dictionary<string, dynamic> model)
        {
            if (model == null)
            {
                model = new Dictionary<string, dynamic>();
            }
            model["Methods"] = new Dictionary<string, dynamic>();

            Func<string, string,JArray> getData =
                (string collection, string filter) =>
            {
                dbProxy.Collection = collection;
                return dbProxy.Get(filter);

            };

            Func<string, string> viewTemplete = (string templatePath) => {
                model[CommonConst.CommonValue.PAGE_TEMPLATE_PATH] = templatePath;
                return string.Empty;
            };

            model[CommonConst.CommonValue.METHODS]["InclueTemplate"] = viewTemplete;

            model[CommonConst.CommonValue.METHODS]["GetData"] = getData;

            Func<JObject> requestBody = () => httpProxy.GetRequestBody<JObject>();
            model[CommonConst.CommonValue.METHODS]["RequestBody"] = requestBody;

            Func<string, string> queryString = (string key) => httpProxy.GetQueryString(key);
            model[CommonConst.CommonValue.METHODS]["QueryString"] = queryString;

            Func<string, JObject, string> includeBlock =
                (string blockPath, JObject blockModel) =>
                {
                    string blockId = blockPath;
                    var inputBlockModel = new Dictionary<string, dynamic>();
                    if (blockModel != null)
                    {
                        foreach (var item in blockModel)
                        {
                            inputBlockModel[item.Key] = item.Value;
                        }
                    }
                    var data = StaticContentHandler.GetStringContent(dbProxy, logger, blockPath);
                    data = viewEngine.Compile(data, blockId, ServerPageModelHelper.SetDefaultModel(dbProxy, httpProxy, logger, viewEngine, inputBlockModel));
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
