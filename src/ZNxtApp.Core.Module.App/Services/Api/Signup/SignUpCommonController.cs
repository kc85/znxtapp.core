using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class SignUpCommonController : UserRegistrationBase
    {
        protected const string SIGNUP_OTP_CHECK_SETTING_KEY = "signup_otp_check";

        public SignUpCommonController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public Dictionary<string, dynamic> SignupModel()
        {
            Dictionary<string, dynamic> model = new Dictionary<string, dynamic>();
            SetBaseViewModelData(model);
            return model;
        }

        public JObject SendSignUpOTP()
        {
            Logger.Debug("Calling SendMobileOTP");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            Logger.Debug("Request body SendMobileOTP", request);
            UserModel requestUser = GetUserDataFromRequest(request);

            var recaptchaResponse = request[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_RESPONSE_KEY].ToString();
            var capchaChecked = SessionProvider.GetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY);

            if (capchaChecked && !GoogleCaptchaHelper.ValidateResponse(Logger, recaptchaResponse, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY), AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY)))
            {
                Logger.Info("Captcha validate fail");
                return ResponseBuilder.CreateReponse(AppResponseCode._CAPTCHA_VALIDATION_FAIL);
            }
            else
            {
                SessionProvider.SetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY, true);
            }
            if (!IsUserExists(requestUser.user_id))
            {
                string securityToken = CommonUtility.RandomString(10);
                if (requestUser.user_type == UserIDType.PhoneNumber.ToString() && OTPService.Send(requestUser.user_id, ModuleAppConsts.Field.SIGN_UP_OTP_SMS_TEMPLATE, OTPType.Signup, securityToken))
                {
                    JObject dataResponse = new JObject();
                    dataResponse[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, null, dataResponse);
                }
                else if (requestUser.user_type == UserIDType.Email.ToString() && OTPService.SendEmail(requestUser.user_id, ModuleAppConsts.Field.SIGN_UP_OTP_EMAIL_TEMPLATE, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.SIGN_UP_OTP_EMAIL_SUBJECT), OTPType.Signup, securityToken))
                {
                    JObject dataResponse = new JObject();
                    dataResponse[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, null, dataResponse);
                }
                else
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._OTP_SEND_ERROR);
                }
            }
            else
            {
                Logger.Info(string.Format("User Exits with this phone number {0}", requestUser.user_id));
                return ResponseBuilder.CreateReponse(AppResponseCode._USER_EXISTS);
            }
        }

        public JObject ValidateOTP()
        {
            Logger.Debug("Calling ValidateOTP");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            var otp = request[CommonConst.CommonField.OTP].ToString();
            UserModel requestUser = GetUserDataFromRequest(request);
            var securityToken = request[CommonConst.CommonField.SECURITY_TOKEN].ToString();
            if (!IsUserExists(requestUser.user_id))
            {
                if (requestUser.user_type == UserIDType.PhoneNumber.ToString() && OTPService.Validate(requestUser.user_id, otp, OTPType.Signup, securityToken))
                {
                    UserModel tempUser = new UserModel() { user_id = requestUser.user_id, user_type = UserIDType.PhoneNumber.ToString() };
                    SessionProvider.SetValue(CommonConst.CommonValue.SIGN_UP_SESSION_USER_KEY, tempUser);
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                }
                else if (requestUser.user_type == UserIDType.Email.ToString() && OTPService.ValidateEmail(requestUser.user_id, otp, OTPType.Signup, securityToken))
                {
                    UserModel tempUser = new UserModel() { user_id = requestUser.user_id, user_type = UserIDType.Email.ToString() };
                    SessionProvider.SetValue(CommonConst.CommonValue.SIGN_UP_SESSION_USER_KEY, tempUser);
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                }
                else
                {
                    Logger.Error("Error OTP validation fail");
                    return ResponseBuilder.CreateReponse(AppResponseCode._OTP_VALIDATION_FAIL);
                }
            }
            else
            {
                Logger.Info(string.Format("User Exits with this phone number {0}", requestUser.user_id));
                return ResponseBuilder.CreateReponse(AppResponseCode._USER_EXISTS);
            }
        }

        public JObject CreateUser()
        {
            Logger.Debug("Calling CreateUser");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            var redirect_url = HttpProxy.GetQueryString(CommonConst.CommonField.REDIRECT_URL_KEY);
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }
            var requestUser = GetUserDataFromRequest(request);

            if (IsOTPCheckEnable())
            {
                var signUpUser = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SIGN_UP_SESSION_USER_KEY);
                if (signUpUser == null)
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._SESSION_USER_NOT_FOUND);
                }
                if (signUpUser.user_id != requestUser.user_id)
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._SESSION_USER_DATA_MISMATCH);
                }
            }

            if (request[CommonConst.CommonField.PASSWORD].ToString() != request[CommonConst.CommonField.CONFIRM_PASSWORD].ToString())
            {
                return ResponseBuilder.CreateReponse(AppResponseCode._SESSION_USER_DATA_MISMATCH);
            }

            var recaptchaResponse = request[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_RESPONSE_KEY].ToString();
            var capchaChecked = SessionProvider.GetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY);

            if (capchaChecked && !GoogleCaptchaHelper.ValidateResponse(Logger, recaptchaResponse, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY), AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY)))
            {
                Logger.Info("Captcha validate fail");
                return ResponseBuilder.CreateReponse(AppResponseCode._CAPTCHA_VALIDATION_FAIL);
            }
            else
            {
                SessionProvider.SetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY, true);
            }
            if (!IsUserExists(requestUser.user_id))
            {
                if (CreateUser(requestUser, request[CommonConst.CommonField.PASSWORD].ToString()))
                {
                    var user = DBProxy.FirstOrDefault<UserModel>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, requestUser.user_id);
                    if (user == null)
                    {
                        Logger.Error(string.Format("User not found user_id : {0} ", requestUser.user_id));
                        return ResponseBuilder.CreateReponse(AppResponseCode._USER_NOT_FOUND);
                    }
                    else
                    {
                        SessionProvider.SetValue(CommonConst.CommonValue.SESSION_USER_KEY, user);
                        var rurl = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.SIGNUP_LENDING_PAGE_SETTING_KEY);
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
                Logger.Info(string.Format("User Exits with this phone number {0}", requestUser.user_id));
                return ResponseBuilder.CreateReponse(AppResponseCode._USER_EXISTS);
            }
        }

        private bool IsOTPCheckEnable()
        {
            bool otpSignupAuthCheck = true;
            bool.TryParse(AppSettingService.GetAppSettingData(SIGNUP_OTP_CHECK_SETTING_KEY), out otpSignupAuthCheck);
            return otpSignupAuthCheck;
        }

        protected void SetBaseViewModelData(Dictionary<string, dynamic> viewModel)
        {
            viewModel[CommonConst.CommonField.GUID] = Guid.NewGuid().ToString();
            viewModel[ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SITE_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SITE_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SECRECT_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_INVISIBLE_RECAPTCHA_SECRECT_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SITE_KEY_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SITE_KEY_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_API_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_API_SETTING_KEY);

            viewModel[ModuleAppConsts.Field.FACEBOOK_API_SECRET_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_API_SECRET_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_GRAPH_API_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_GRAPH_API_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_OAUTH_CALLBACK_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_OAUTH_CALLBACK_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_OAUTH_URL_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_OAUTH_URL_SETTING_KEY);
            viewModel[ModuleAppConsts.Field.FACEBOOK_REQUEST_OBJECT_ACCESS_SETTING_KEY] = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FACEBOOK_REQUEST_OBJECT_ACCESS_SETTING_KEY);
        }

        private bool CreateUser(UserModel user, string password)
        {
            JObject userData = new JObject();
            userData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            userData[CommonConst.CommonField.USER_ID] = userData[CommonConst.CommonField.DATA_KEY] = user.user_id;
            userData[CommonConst.CommonField.USER_TYPE] = user.user_type;
            userData[CommonConst.CommonField.PASSWORD] = EncryptionService.GetHash(password);
            userData[CommonConst.CommonField.IS_ENABLED] = true;

            if (user.user_type == UserIDType.PhoneNumber.ToString())
            {
                userData[CommonConst.CommonField.PHONE] = user.user_id;
                userData[CommonConst.CommonField.NAME] = user.user_id;
                userData[CommonConst.CommonField.IS_PHONE_VALIDATE] = (true && IsOTPCheckEnable());
            }
            else if (user.user_type == UserIDType.Email.ToString())
            {
                userData[CommonConst.CommonField.EMAIL] = user.user_id;
                userData[CommonConst.CommonField.NAME] = user.user_id;
                userData[CommonConst.CommonField.IS_EMAIL_VALIDATE] = (true && IsOTPCheckEnable());
            }

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