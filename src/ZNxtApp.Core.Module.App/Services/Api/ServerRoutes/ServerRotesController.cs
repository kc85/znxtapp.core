using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Services.Api.Signup;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.ServerRoutes
{
    public class ServerRotesController : ViewBaseService
    {
        public ServerRotesController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public JObject Get()
        {
            try
            {
                return GetPaggedData(CommonConst.Collection.SERVER_ROUTES);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("SettingController.Get {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
        public JObject Update()
        {
            try
            {
                JObject request = HttpProxy.GetRequestBody<JObject>();
                if (request == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                if (request[CommonConst.CommonField.DISPLAY_ID] == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                JObject filter = new JObject();
                filter[CommonConst.CommonField.DISPLAY_ID] = request[CommonConst.CommonField.DISPLAY_ID].ToString();

                var dbresponse = DBProxy.Update(CommonConst.Collection.SERVER_ROUTES, filter.ToString(), request, true, MergeArrayHandling.Replace);
                if (dbresponse == 0)
                {
                    Logger.Error(string.Format("Error in  ServerRotesController.update: {0}, collection {1}", "Error in updating data in db", CommonConst.Collection.SERVER_ROUTES));
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("ServerRotesController.update {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
    }
}
