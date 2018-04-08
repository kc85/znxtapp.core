using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
