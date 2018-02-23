using Newtonsoft.Json.Linq;
using System;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Util;

namespace ZNxtAap.Core.Web.Services
{
    public class CronJobExecuter
    {
        private AssemblyLoader _assemblyLoader;
        private ILogger _logger;

        public CronJobExecuter()
        {
            _assemblyLoader = AssemblyLoader.GetAssemblyLoader();
            _logger = Logger.GetLogger(this.GetType().FullName, Guid.NewGuid().ToString());
        }

        internal void Exec(JObject cronJob)
        {
            try
            {
                IActionExecuter actionExecuter = new ActionExecuter(_logger);
                ParamContainer pamamContainer = CreateParamContainer(_logger, actionExecuter);
                object resonse = actionExecuter.Exec(cronJob[CommonConst.CommonField.EXECULT_ASSEMBLY].ToString(), cronJob[CommonConst.CommonField.EXECUTE_TYPE].ToString(), cronJob[CommonConst.CommonField.EXECUTE_METHOD].ToString(), pamamContainer);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private ParamContainer CreateParamContainer(ILogger loggerController, IActionExecuter actionExecuter)
        {
            IRoutings routings = Routings.Routings.GetRoutings();
            ILogReader logReader = Logger.GetLogReader();
            ResponseBuilder responseBuilder = new ResponseBuilder(loggerController, logReader, new CronJobInitData(loggerController.TransactionId));
            IDBService dbService = new MongoDBService(ApplicationConfig.DataBaseName);
            IPingService pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.PING));
            ParamContainer pamamContainer = new ParamContainer();
            IAppSettingService appSettingService = AppSettingService.Instance;
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_DBPROXY, () => { return dbService; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_LOGGER, () => { return loggerController; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_ACTIONEXECUTER, () => { return actionExecuter; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_PING_SERVICE, () => { return pingService; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_RESPONBUILDER, () => { return responseBuilder; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_APP_SETTING, () => { return appSettingService; });
            pamamContainer.AddKey(CommonConst.CommonValue.PARAM_ROUTING_OBJECT, () => { return routings; });

            return pamamContainer;
        }
    }

    public class CronJobInitData : IInitData
    {
        public DateTime InitDateTime { get; private set; }

        public string TransactionId { get; private set; }

        public CronJobInitData(string transactionId)
        {
            InitDateTime = DateTime.Now;
            TransactionId = transactionId;
        }
    }
}