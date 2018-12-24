using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Common
{
    public class CommonGetController : ViewBaseService
    {
        public CommonGetController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public JObject Get()
        {
            try
            {
                var collection = HttpProxy.GetQueryString("collection");
                if (string.IsNullOrEmpty(collection))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                return GetPaggedData(collection);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("CommonGetController.Get {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
    }
}