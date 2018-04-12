using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class SignUpCommonController : ViewBaseService
    {
        public SignUpCommonController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public Dictionary<string,dynamic> SignupModel()
        {
            Dictionary<string, dynamic> model = new Dictionary<string, dynamic>();
            SetBaseViewModelData(model);
            return model;

        }
        public JObject SendMobileOTP()
        {
            Logger.Debug("Calling SendMobileOTP");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            Logger.Debug("Request body SendMobileOTP", request);
            var phonenumber = request[ModuleAppConsts.Field.PHONE_FIELD].ToString();

            var recaptchaResponse = request[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_RESPONSE_KEY].ToString();

            if (GoogleCaptchaHelper.ValidateResponse(Logger, recaptchaResponse, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY), AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY)))
            {
                Logger.Debug("Captcha validate success");
                if (!IsUserExists(phonenumber))
                {
                    if (OTPService.Send(phonenumber, ModuleAppConsts.Field.SIGN_UP_OTP_TEMPLATE, OTPType.Signup))
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                    }
                    else
                    {
                        return ResponseBuilder.CreateReponse(AppResponseCode._OTP_SEND_ERROR);
                    }
                }
                else
                {
                    Logger.Info(string.Format("User Exits with this phone number {0}", phonenumber));
                    return ResponseBuilder.CreateReponse(AppResponseCode._USER_EXISTS);
                }
            }
            else
            {
                Logger.Info("Captcha validate fail");
                return ResponseBuilder.CreateReponse(AppResponseCode._CAPTCHA_VALIDATION_FAIL);
            }
        }

        protected bool IsUserExists(string userId, UserIDType userIdType = UserIDType.PhoneNumber)
        {
            return GetUser(userId, userIdType) != null;
        }

        private JObject GetUser(string userId, UserIDType userIdType)
        {
            DBProxy.Collection = CommonConst.Collection.USERS;
            string filter = "{}";
            var response = DBProxy.Get(filter);
            if (response.Count != 0)
            {
                return response[0] as JObject;
            }
            else
            {
                return null;
            }
        }

        protected void SetBaseViewModelData(Dictionary<string, dynamic> viewModel)
        {
            viewModel[ModuleAppConsts.Field.GUID] = Guid.NewGuid().ToString();
            viewModel[ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SITE_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SITE_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SECRECT_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SECRECT_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SITE_KEY_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SITE_KEY_SETTING_KEY);


            viewModel[ModuleAppConsts.Field.FACEBOOK_API_SECRET_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_API_SECRET_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_GRAPH_API_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_GRAPH_API_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_OAUTH_CALLBACK_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_OAUTH_CALLBACK_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_OAUTH_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_OAUTH_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_REQUEST_OBJECT_ACCESS_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_REQUEST_OBJECT_ACCESS_SETTING_KEY);

            //var invisibleCapchaSiteKey = WebDBProxyHelper.GetAppSettingStringValue(_dbProxy, CommonConsts.G_INVISIBLE_RECAPTCHA_SITE_SETTING_KEY);
            //var visibleCapchaSiteKey = WebDBProxyHelper.GetAppSettingStringValue(_dbProxy, CommonConsts.G_RECAPTCHA_SITE_SETTING_KEY);

            //viewModel[CommonConsts.APP_THEME_COLOR_SETTING_KEY] = GetAppSettingStringValue(CommonConsts.APP_THEME_COLOR_SETTING_KEY);
            //viewModel[CommonConsts.APP_NAME_SETTING_KEY] = GetAppName();
            //viewModel[CommonConsts.CLIENT_SESSION_DATA_KEY] = _httpRequestProxy.GetSessionUser();
            //viewModel["GUID"] = Guid.NewGuid().ToString();
            //viewModel[CommonConsts.G_INVISIBLE_RECAPTCHA_SITE_SETTING_KEY] = invisibleCapchaSiteKey;
            //viewModel[CommonConsts.G_RECAPTCHA_SITE_SETTING_KEY] = visibleCapchaSiteKey;


        }
    }
}
