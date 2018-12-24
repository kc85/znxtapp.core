using Newtonsoft.Json.Linq;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Login
{
    public class LoginController : ViewBaseService
    {
        public LoginController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public JObject Login()
        {
            Logger.Debug("Calling LoginController.Login");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            var user_id = request[CommonConst.CommonField.USER_ID].ToString();
            var password = request[CommonConst.CommonField.PASSWORD].ToString();
            var recaptchaResponse = request[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_RESPONSE_KEY].ToString();
            if (!GoogleCaptchaHelper.ValidateResponse(Logger, recaptchaResponse, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SECRECT_SETTING_KEY), AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY)))
            {
                Logger.Info("Captcha validate fail");
                return ResponseBuilder.CreateReponse(AppResponseCode._CAPTCHA_VALIDATION_FAIL);
            }
            if (Validate(user_id, password))
            {
                JObject user = DBProxy.FirstOrDefault(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, user_id);
                if (user == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED, user);
                }
                else
                {
                    UserModel userModel = JObjectHelper.Deserialize<UserModel>(user);
                    SessionProvider.SetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY, userModel);
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }
        }

        private bool Validate(string userId, string password)
        {
            var passwordHash = EncryptionService.GetHash(password);
            JObject filter = new JObject();
            filter[CommonConst.CommonField.USER_ID] = userId;
            filter[CommonConst.CommonField.PASSWORD] = passwordHash;
            filter[CommonConst.CommonField.IS_ENABLED] = true;
            var user = DBProxy.FirstOrDefault(CommonConst.Collection.USERS, filter.ToString());
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}