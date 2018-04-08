using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Module.App.Consts;
using Facebook;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class FacebookAuthUserController: ViewBaseService
    {
        public FacebookAuthUserController(ParamContainer paramContainer) : base(paramContainer)
        {

        }

        public JObject GetOAuthRequestData()
        {
            var fb = new Facebook.FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_API_SECRET_SETTING_KEY),
                redirect_uri = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_OAUTH_CALLBACK_URL_SETTING_KEY),
                response_type = "code",
                scope = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_REQUEST_OBJECT_ACCESS_SETTING_KEY)
            });

            JObject response = new JObject();
            response[ModuleAppConsts.Field.FACEBOOK_GRAPH_API_URL_SETTING_KEY] = loginUrl.AbsoluteUri.Replace("https://www.facebook.com/dialog/oauth", "https://graph.facebook.com/v2.8/oauth/authorize");
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, response);
        }

    }
}
