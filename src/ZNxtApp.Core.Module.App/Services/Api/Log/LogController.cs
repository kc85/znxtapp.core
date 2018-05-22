using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Services.Api.Signup;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Log
{
    public class LogController : ViewBaseService
    {
        public LogController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public JObject Get()
        {
            try
            {
                return GetPaggedData(CommonConst.Collection.SERVER_LOGS);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("LogController.Get {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

     
    }
}
