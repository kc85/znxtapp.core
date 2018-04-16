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
            var phonenumber = request[CommonConst.CommonField.PHONE_FIELD].ToString();

            var recaptchaResponse = request[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_RESPONSE_KEY].ToString();

            if (GoogleCaptchaHelper.ValidateResponse(Logger, recaptchaResponse, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY), AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY)))
            {
                Logger.Debug("Captcha validate success");
                if (!IsUserExists(phonenumber))
                {
                    string securityToken = CommonUtility.RandomString(10);
                    if (OTPService.Send(phonenumber, ModuleAppConsts.Field.SIGN_UP_OTP_TEMPLATE, OTPType.Signup, securityToken))
                    {
                        JObject dataResponse = new JObject();
                        dataResponse[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS,null, dataResponse);
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

        public JObject ValidateOTPCreateUser()
        {
            
            Logger.Debug("Calling ValidateOTPCreateUser");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            var redirect_url = HttpProxy.GetQueryString(CommonConst.CommonField.REDIRECT_URL_KEY);
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            var otp = request[CommonConst.CommonField.OTP].ToString();
            var phonenumber = request[CommonConst.CommonField.PHONE_FIELD].ToString();
            var securityToken = request[CommonConst.CommonField.SECURITY_TOKEN].ToString();
            if (!IsUserExists(phonenumber))
            {
                if (OTPService.Validate(phonenumber, otp, OTPType.Signup, securityToken))
                {
                    if (CreateUser(phonenumber))
                    {
                        var user = DBProxy.FirstOrDefault<UserModel>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, phonenumber);
                        if (user == null)
                        {
                            Logger.Error(string.Format("User not found for phone : {0} ", phonenumber));
                            return ResponseBuilder.CreateReponse(AppResponseCode._USER_NOT_FOUND);
                        }
                        else
                        {
                            SessionProvider.SetValue(CommonConst.CommonValue.SESSION_USER_KEY, user);
                            var rurl = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.PHONE_SIGNUP_DEFAULT_REDIRECT_PAGE_SETTING_KEY);
                            JObject resonseData = new JObject();
                            if (string.IsNullOrEmpty(redirect_url))
                            {
                                resonseData[CommonConst.CommonField.REDIRECT_URL_KEY] = rurl;
                            }
                            else
                            {
                                resonseData[CommonConst.CommonField.REDIRECT_URL_KEY] = string.Format("{0}?{1}={2}", rurl, CommonConst.CommonField.REDIRECT_URL_KEY, redirect_url);
                            }
                            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, null, resonseData);
                        }
                    }
                    else
                    {
                        Logger.Error("Error while addd new user");
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                }
                else
                {
                    Logger.Error("Error OTP validation fail");
                    return ResponseBuilder.CreateReponse(AppResponseCode._OTP_VALIDATION_FAIL);
                }
            }
            else
            {
                Logger.Info(string.Format("User Exits with this phone number {0}", phonenumber));
                return ResponseBuilder.CreateReponse(AppResponseCode._USER_EXISTS);
            }
        }

        protected bool IsUserExists(string userId, UserIDType userIdType = UserIDType.PhoneNumber)
        {
            return GetUser(userId, userIdType) != null;
        }

        private JObject GetUser(string userId, UserIDType userIdType)
        {
            return DBProxy.FirstOrDefault<JObject>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, userId);
        }

        protected void SetBaseViewModelData(Dictionary<string, dynamic> viewModel)
        {
            viewModel[CommonConst.CommonField.GUID] = Guid.NewGuid().ToString();
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
            
        }

        private bool CreateUser(string phoneNumber)
        {
            JObject userData = new JObject();
            userData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            userData[CommonConst.CommonField.USER_ID] = userData[CommonConst.CommonField.DATA_KEY] = phoneNumber;
            userData[CommonConst.CommonField.USER_TYPE] = UserIDType.PhoneNumber.ToString();
            userData[CommonConst.CommonField.USER_TYPE] = UserIDType.PhoneNumber.ToString();
            var userGroup = AppSettingService.GetAppSetting(ModuleAppConsts.Field.DEFAULT_USER_GROUPS_APP_SETTING_KEY);
            Logger.Debug("userGroup setting data", userGroup);
            if (userGroup == null)
            {
                throw new Exception("User Group setting not found in AppSettings");
            }
            if (userGroup[CommonConst.CommonField.DATA] == null || userGroup[CommonConst.CommonField.DATA][CommonConst.CommonField.USER_GROUPS] == null)
            {
                throw new Exception("User Groups not found in AppSettings");
            }
            userData[CommonConst.CommonField.USER_GROUPS] = (userGroup[CommonConst.CommonField.DATA][CommonConst.CommonField.USER_GROUPS] as JArray);
            return DBProxy.Write(CommonConst.Collection.USERS, userData);

        }
    }
}
