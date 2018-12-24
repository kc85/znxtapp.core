using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Services
{
    public abstract class BaseService
    {
        protected IDBService DBProxy { get; private set; }
        protected ILogger Logger { get; private set; }
        protected IActionExecuter ActionExecuter { get; private set; }
        protected IPingService PingService { get; private set; }
        protected ResponseBuilder ResponseBuilder { get; private set; }
        protected IAppSettingService AppSettingService { get; private set; }
        protected IViewEngine ViewEngine { get; private set; }
        protected IOTPService OTPService { get; private set; }
        protected ISMSService SMSService { get; private set; }
        protected IEmailService EmailService { get; private set; }
        protected IEncryption EncryptionService { get; private set; }
        protected IKeyValueStorage KeyValueStorage { get; private set; }
        protected IHttpRestClient HttpClient{ get; private set; }
        public BaseService(ParamContainer paramContainer)
        {
            DBProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_DBPROXY);
            Logger = paramContainer.GetKey(CommonConst.CommonValue.PARAM_LOGGER);
            ActionExecuter = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ACTIONEXECUTER);
            PingService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_PING_SERVICE);
            ResponseBuilder = paramContainer.GetKey(CommonConst.CommonValue.PARAM_RESPONBUILDER);
            ViewEngine = paramContainer.GetKey(CommonConst.CommonValue.PARAM_VIEW_ENGINE);
            AppSettingService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_APP_SETTING);
            OTPService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_OTP_SERVICE);
            SMSService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_SMS_SERVICE);
            EmailService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_EMAIL_SERVICE);
            EncryptionService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ENCRYPTION_SERVICE);
            KeyValueStorage = paramContainer.GetKey(CommonConst.CommonValue.PARAM_KEY_VALUE_STORAGE);
            HttpClient = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTP_CLIENT);
        }
    }
}