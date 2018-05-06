using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Module
{
    public class GetModuleController : ViewBaseService
    {
        public GetModuleController(ParamContainer paramContainer) : base(paramContainer)
        {

        }

        public JObject GetModules()
        {
            DBProxy.Collection = CommonConst.Collection.MODULES;
            var data = DBProxy.Get("{}");
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data);
        }
    }
}
