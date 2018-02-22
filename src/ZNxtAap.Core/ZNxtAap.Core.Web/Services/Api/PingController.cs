using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Services;

namespace ZNxtAap.Core.Web.Services.Api
{
    public class PingController : BaseController
    {
        public PingController(ParamContainer requestParam) :base(requestParam)
        {

        }

        public JObject Ping()
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
    }
}
