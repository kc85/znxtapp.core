using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Logout
{
    public class LogoutController : ViewBaseService
    {
        public LogoutController(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject LogoutAction()
        {
            try
            {
                Logger.Debug("ResetSession");
                SessionProvider.ResetSession();
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("LogoutAction error {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
    }
}
