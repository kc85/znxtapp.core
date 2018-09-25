using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Services;

namespace ZNxtApp.Core.Web.Helper
{
    public static class ActionExecuterHelper
    {
        public static ParamContainer CreateParamContainer(ILogger loggerController, IActionExecuter actionExecuter)
        {
            ILogReader logReader = Logger.GetLogReader();
            IDBService dbService = new MongoDBService(ApplicationConfig.DataBaseName);
            IPingService pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName));
            ParamContainer paramContainer = new ParamContainer();
            IAppSettingService appSettingService = AppSettingService.Instance;
            IViewEngine viewEngine = ViewEngine.GetEngine();
            IEncryption encryption = new EncryptionService();
            ISMSService smsService = new SMSService(loggerController, dbService, actionExecuter, viewEngine, paramContainer);
            IEmailService emailService = new EmailService(loggerController, dbService, actionExecuter, viewEngine, paramContainer);
            IOTPService otpService = new OTPService(loggerController, dbService, smsService, emailService, appSettingService);
            IKeyValueStorage keyValueStorage = new KeyValueFileStorage(encryption, appSettingService);

            paramContainer.AddKey(CommonConst.CommonValue.PARAM_DBPROXY, () => { return dbService; });

            paramContainer.AddKey(CommonConst.CommonValue.PARAM_LOGGER, () => { return loggerController; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_ACTIONEXECUTER, () => { return actionExecuter; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_PING_SERVICE, () => { return pingService; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_PING_SERVICE, () => { return pingService; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_APP_SETTING, () => { return appSettingService; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_VIEW_ENGINE, () => { return viewEngine; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_OTP_SERVICE, () => { return otpService; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_SMS_SERVICE, () => { return smsService; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_EMAIL_SERVICE, () => { return emailService; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_ENCRYPTION_SERVICE, () => { return encryption; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_KEY_VALUE_STORAGE, () => { return keyValueStorage; });

            return paramContainer;
        }

        public static ParamContainer CreateParamContainer(RoutingModel route, IHttpContextProxy httpProxy, ILogger loggerController, IActionExecuter actionExecuter)
        {
            var paramContainer = CreateParamContainer(loggerController, actionExecuter);
            ILogReader logReader = Logger.GetLogReader();
            IDBService dbService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_DBPROXY);
            IViewEngine viewEngine = paramContainer.GetKey(CommonConst.CommonValue.PARAM_VIEW_ENGINE);
            ResponseBuilder responseBuilder = new ResponseBuilder(loggerController, logReader, httpProxy);
            ISessionProvider sessionProvider = new SessionProvider(httpProxy, dbService, loggerController);
            IwwwrootContentHandler ContentHandler = new WwwrootContentHandler(httpProxy, dbService, viewEngine, actionExecuter, loggerController);
            (dbService as MongoDBService).User = () =>
            {
                string result = string.Empty;
                var user = sessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (user != null)
                {
                    result = string.Format("{0}::{1}", user.id, user.name);
                }
                return result;
            };
            (loggerController as Logger).DBProxy = dbService;
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_ROUTE, () => { return route; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY, () => { return httpProxy; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_RESPONBUILDER, () => { return responseBuilder; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_SESSION_PROVIDER, () => { return sessionProvider; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_CONTENT_HANDLER, () => { return ContentHandler; });

            return paramContainer;
        }
    }
}