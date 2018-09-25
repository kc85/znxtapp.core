using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.Services.Api
{
    public class PingController : ApiBaseService
    {
        public PingController(ParamContainer requestParam) : base(requestParam)
        {
        }

        public JObject Ping()
        {
            try
            {
                if (PingService.PingDb())
                {
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                }
                else
                {
                    return ResponseBuilder.CreateReponse(PingResponseCode._PING_FAIL);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(PingResponseCode._PING_FAIL);
            }
        }

        public int Completed()
        {
            return 1;
        }

        public JObject ActionTest()
        {
            return ResponseBuilder.CreateReponse(CommonConst._200_OK);
        }
    }
}