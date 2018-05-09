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
using ZNxtApp.Core.Module.App.Services.Api.Signup;

namespace ZNxtApp.Core.Module.App.Services.Api.Forgetpassword
{
    public class Forgetpassword : UserRegistrationBase
    {
        public Forgetpassword(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject SendForgetpassOTP()
        {
            Logger.Debug("Calling SendForgetpassOTP");
            JObject request = HttpProxy.GetRequestBody<JObject>();
            if (request == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            Logger.Debug("Request body SendForgetpassOTP", request);
            UserModel requestUser = GetUserDataFromRequest(request);
            var recaptchaResponse = request[ModuleAppConsts.Field.GOOGLE_RECAPTCHA_RESPONSE_KEY].ToString();
            var capchaChecked = SessionProvider.GetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY);

            if (capchaChecked && !GoogleCaptchaHelper.ValidateResponse(Logger, recaptchaResponse, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_SECRECT_SETTING_KEY), AppSettingService.GetAppSettingData(ModuleAppConsts.Field.GOOGLE_RECAPTCHA_VALIDATE_URL_SETTING_KEY)))
            {
                Logger.Info("Captcha validate fail SendForgetpassOTP");
                return ResponseBuilder.CreateReponse(AppResponseCode._CAPTCHA_VALIDATION_FAIL);
            }
            else
            {
                SessionProvider.SetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY, true);
            }
            if (IsUserExists(requestUser.user_id))
            {
                string securityToken = CommonUtility.RandomString(10);
                if (requestUser.user_type == UserIDType.PhoneNumber.ToString() && OTPService.Send(requestUser.user_id, ModuleAppConsts.Field.FORGET_PASS_OTP_SMS_TEMPLATE, OTPType.Forgetpassword, securityToken))
                {
                    JObject dataResponse = new JObject();
                    dataResponse[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;
                    SessionProvider.SetValue<UserModel>(CommonConst.CommonValue.SIGN_UP_SESSION_USER_KEY, requestUser);

                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, null, dataResponse);
                }
                else if (requestUser.user_type == UserIDType.Email.ToString() && OTPService.SendEmail(requestUser.user_id, ModuleAppConsts.Field.FORGET_PASS_OTP_EMAIL_TEMPLATE, AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FORGET_PASS_OTP_EMAIL_SUBJECT), OTPType.Forgetpassword, securityToken))
                {
                    JObject dataResponse = new JObject();
                    dataResponse[CommonConst.CommonField.SECURITY_TOKEN] = securityToken;

                    SessionProvider.SetValue<UserModel>(CommonConst.CommonValue.SIGN_UP_SESSION_USER_KEY, requestUser);
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, null, dataResponse);
                }
                else
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._OTP_SEND_ERROR);
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(AppResponseCode._USER_NOT_FOUND);
            }
        }


        public JObject ResetPassword()
        {

            try
            {
                Logger.Debug("Calling ResetPassword");
                JObject request = HttpProxy.GetRequestBody<JObject>();
                Logger.Debug("ResetPassword Request data", request);

                var redirect_url = HttpProxy.GetQueryString(CommonConst.CommonField.REDIRECT_URL_KEY);
                if (request == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                var otp = request[CommonConst.CommonField.OTP].ToString();
                var requestUser = GetUserDataFromRequest(request);
                var forgetpasswordUser = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SIGN_UP_SESSION_USER_KEY);
                if (forgetpasswordUser == null)
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._SESSION_USER_NOT_FOUND);
                }
                if (forgetpasswordUser.user_id != requestUser.user_id)
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._SESSION_USER_DATA_MISMATCH);
                }


                if (request[CommonConst.CommonField.PASSWORD].ToString() != request[CommonConst.CommonField.CONFIRM_PASSWORD].ToString())
                {
                    return ResponseBuilder.CreateReponse(AppResponseCode._SESSION_USER_DATA_MISMATCH);
                }

                var capchaChecked = SessionProvider.GetValue<bool>(USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY);

                if (!capchaChecked)
                {
                    Logger.Info("Captcha validate fail");
                    return ResponseBuilder.CreateReponse(AppResponseCode._CAPTCHA_VALIDATION_FAIL);
                }
               

                bool OTPValidate = false;
                if (requestUser.user_type == UserIDType.PhoneNumber.ToString() && OTPService.Validate(requestUser.user_id, otp, OTPType.Forgetpassword,string.Empty))
                {
                    OTPValidate = true;
                }
                else if (requestUser.user_type == UserIDType.Email.ToString() && OTPService.ValidateEmail(requestUser.user_id, otp, OTPType.Forgetpassword, string.Empty))
                {
                    OTPValidate = true;
                }
                else
                {
                    Logger.Error("Error OTP validation fail");
                    return ResponseBuilder.CreateReponse(AppResponseCode._OTP_VALIDATION_FAIL);
                }

                if (ResetPass(requestUser, request[CommonConst.CommonField.PASSWORD].ToString()) && OTPValidate)
                {
                    var user = DBProxy.FirstOrDefault<UserModel>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, requestUser.user_id);


                    var rurl = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.FORGET_PASS_LENDING_PAGE_SETTING_KEY);
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
                else
                {
                    Logger.Error("Error while ResetPassword");
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
            }
            catch (Exception ex)
            {

                Logger.Error(string.Format("Forgetpassword.ResetPassword error : {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
           
        }

        private bool ResetPass(UserModel requestUser, string password)
        {
            JObject userData = new JObject();
            userData[CommonConst.CommonField.USER_ID] = userData[CommonConst.CommonField.DATA_KEY] = requestUser.user_id;
            userData[CommonConst.CommonField.USER_TYPE] = requestUser.user_type;

            var filter = userData.ToString();
            userData[CommonConst.CommonField.PASSWORD] = EncryptionService.GetHash(password);

            return DBProxy.Write(CommonConst.Collection.USERS, userData, filter);
        }
    }
}
