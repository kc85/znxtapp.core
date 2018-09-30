using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services.Helper;
using ZNxtApp.Core.Web.Helper;

namespace ZNxtApp.Core.Web.Services
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
                pamamContainer.AddKey(CommonConst.CommonValue.PARAM_CRON_JOB_OBJ, () => { return cronJob; });
                IDBService dbProxy = pamamContainer.GetKey(CommonConst.CommonValue.PARAM_DBPROXY);

                var filter = new JObject();
                filter[CommonConst.CommonField.DISPLAY_ID] = cronJob[CommonConst.CommonField.DISPLAY_ID];
                cronJob[CommonConst.CommonField.STATUS] = CommonConst.CommonValue.INPROGRESS;
                cronJob[CommonConst.CommonField.TRANSACTION_ID] = _logger.TransactionId;
                var startDatetime = DateTime.Now;
                cronJob[CommonConst.CommonField.LAST_EXEC_ON] = startDatetime.ToString();
                cronJob[CommonConst.CommonField.ERR_MESSAGE] = string.Empty;
                if (cronJob[CommonConst.CommonField.HISTORY] == null)
                {
                    cronJob[CommonConst.CommonField.HISTORY] = new JArray();
                }
                else
                {
                    while ((cronJob[CommonConst.CommonField.HISTORY] as JArray).Count >= 10)
                    {
                        (cronJob[CommonConst.CommonField.HISTORY] as JArray).Remove((cronJob[CommonConst.CommonField.HISTORY] as JArray)[0]);
                    }
                }
                dbProxy.Update(CommonConst.Collection.CRON_JOB, filter.ToString(), cronJob, false, MergeArrayHandling.Replace);
                try
                {
                    object resonse = actionExecuter.Exec(cronJob[CommonConst.CommonField.EXECULT_ASSEMBLY].ToString(), cronJob[CommonConst.CommonField.EXECUTE_TYPE].ToString(), cronJob[CommonConst.CommonField.EXECUTE_METHOD].ToString(), pamamContainer);
                    cronJob[CommonConst.CommonField.STATUS] = CommonConst.CommonValue.FINISH;
                }
                catch (Exception ex)
                {
                    cronJob[CommonConst.CommonField.ERR_MESSAGE] = ex.Message;
                    cronJob[CommonConst.CommonField.STATUS] = CommonConst.CommonValue.FINISH_WITH_ERROR;
                }
                finally
                {
                    cronJob[CommonConst.CommonField.DURATION] = (DateTime.Now - startDatetime).TotalMilliseconds;
                    JObject history = new JObject();
                    history[CommonConst.CommonField.START_ON] = startDatetime.ToString();
                    history[CommonConst.CommonField.DURATION] = cronJob[CommonConst.CommonField.DURATION];
                    history[CommonConst.CommonField.STATUS] = cronJob[CommonConst.CommonField.STATUS];
                    if (cronJob[CommonConst.CommonField.ERR_MESSAGE] != null)
                    {
                        history[CommonConst.CommonField.ERR_MESSAGE] = cronJob[CommonConst.CommonField.ERR_MESSAGE];
                    }
                    history[CommonConst.CommonField.TRANSACTION_ID] = _logger.TransactionId;
                    (cronJob[CommonConst.CommonField.HISTORY] as JArray).Add(history);
                }

                dbProxy.Update(CommonConst.Collection.CRON_JOB, filter.ToString(), cronJob, false, MergeArrayHandling.Replace);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private ParamContainer CreateParamContainer(ILogger loggerController, IActionExecuter actionExecuter)
        {
            ParamContainer paramContainer = ActionExecuterHelper.CreateParamContainer(_logger, actionExecuter);
            IRoutings routings = Routings.Routings.GetRoutings();
            ILogReader logReader = Logger.GetLogReader();
            ResponseBuilder responseBuilder = new ResponseBuilder(loggerController, logReader, new CronJobInitData(loggerController.TransactionId));
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_RESPONBUILDER, () => { return responseBuilder; });
            paramContainer.AddKey(CommonConst.CommonValue.PARAM_ROUTING_OBJECT, () => { return routings; });
            return paramContainer;
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