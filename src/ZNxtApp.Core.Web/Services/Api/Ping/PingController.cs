using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.Services.Api
{
    public class PingController : ApiBaseService
    {
        public PingController(ParamContainer requestParam) :base(requestParam)
        {

        }

        public JObject Ping()
        {
            string template = "Hello @Name , welcome to RazorEngine! @dt()";

            var inputData = new Dictionary<string, dynamic>();
            inputData["Name"] = "Khanin";
            Func<string> dt = () => { return DateTime.Now.ToString() ; };
            inputData["dt"] = dt;

            JObject data = new JObject();
            data["Text"] = ViewEngine.Compile(template, "templateKey", inputData);
            
            if (PingService.PingDb())
            {
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data);
            }
            else
            {
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
