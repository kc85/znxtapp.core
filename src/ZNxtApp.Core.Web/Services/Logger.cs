using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class Logger : ILogger,ILogReader
    {
        private static object lockObjet = new object();
        private string _loggerName;
        private string _transactionId;
        private IDBService _dbProxy;
        public IDBService DBProxy { get { return _dbProxy; } set { _dbProxy = value; } } 
        List<string> _logLevels;
        
        public string TransactionId
        {
            get { return _transactionId; }
        }

        public Logger(IDBService dbService = null)
        {
            if (dbService == null)
            {
                _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            }
           else {
                _dbProxy = dbService;
            }
            _logLevels = GetLogLevels();
        }
        public static ILogger GetLogger(string loggerName, string transactionId, IDBService dbService = null)
        {
            lock (lockObjet)
            {
                Logger _logger = new Logger(dbService);
                _logger._loggerName = loggerName;
                _logger._transactionId = transactionId;
                return _logger;
            }
        }
        public static ILogReader GetLogReader()
        {
            lock (lockObjet)
            {
                Logger _logger = new Logger();
                _logger._loggerName = string.Empty;
                _logger._transactionId = string.Empty;
                return _logger;
            }
        }

        public void Debug(string message, Newtonsoft.Json.Linq.JObject logData = null)
        {
            if (_logLevels.IndexOf(Loglevels.Debug.ToString()) != -1)
            {
                if (logData == null)
                {
                    logData = new JObject();
                }
                JObject log = LoggerCommon(message, logData, Loglevels.Debug.ToString());
                WriteLog(log);
            }
        }

        public void Error(string message, Exception ex = null)
        {
            Error(message, ex, null);
        }

        public void Error(string message, Exception ex, Newtonsoft.Json.Linq.JObject logData = null)
        {
            if (_logLevels.IndexOf(Loglevels.Error.ToString()) != -1)
            {
                if (logData == null)
                {
                    logData = new JObject();
                }
                JObject log = LoggerCommon(message, logData, Loglevels.Error.ToString());
                if (ex != null)
                {
                    log[CommonConst.CommonField.ERR_MESSAGE] = ex.Message;
                    log[CommonConst.CommonField.STACKTRACE] = ex.StackTrace.ToString();
                    //   log[CommonConst.CommonField.FULL_STACKTRACE] = GetAllFootprints(ex);
                }
                WriteLog(log);
            }
        }

        public void Info(string message, Newtonsoft.Json.Linq.JObject logData = null)
        {
            if (_logLevels.IndexOf(Loglevels.Info.ToString()) != -1)
            {
                if (logData == null)
                {
                    logData = new JObject();
                }
                JObject log = LoggerCommon(message, logData, Loglevels.Info.ToString());
                WriteLog(log);
            }
        }

        public void Transaction(Newtonsoft.Json.Linq.JObject transactionData, TransactionState state)
        {
            var log = LoggerCommon(string.Format("Transaction State:{0}", state.ToString()), transactionData, Loglevels.Transaction.ToString());
            log[CommonConst.CommonField.TRANSACTION_STATE] = state.ToString();
            WriteLog(log);
        }

        private void WriteLog(JObject logData)
        {
            lock (lockObjet)
            {                
                _dbProxy.WriteData(CommonConst.Collection.SERVER_LOGS,logData);
            }
        }
        private JObject LoggerCommon(string message, JObject loginputData, string level)
        {
            var logData = new JObject();
            logData[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = DateTime.Now;
            logData[CommonConst.CommonField.DISPLAY_ID] = Guid.NewGuid().ToString();
            logData[CommonConst.CommonField.LOGGER_NAME] = _loggerName;
            logData[CommonConst.CommonField.LOG_TYPE] = level;
            logData[CommonConst.CommonField.TRANSATTION_ID] = TransactionId;
            logData[CommonConst.CommonField.LOG_MESSAGE] = message;
            logData[CommonConst.CommonField.DATA] = loginputData;
            return logData;
        }
       
        private List<string> GetLogLevels()
        {
            List<string> logLevals = new List<string>();
            var setting = AppSettingService.Instance.GetAppSetting(this.ToString());
            if (setting == null)
            {
                AddLogComponents();
            }
            setting = AppSettingService.Instance.GetAppSetting(this.ToString());

            if (setting[CommonConst.CommonField.DATA][CommonConst.CommonField.LOG_LOG_LEVELS] != null)
            {
                foreach (var item in setting[CommonConst.CommonField.DATA][CommonConst.CommonField.LOG_LOG_LEVELS] as JArray)
                {
                    logLevals.Add(item.ToString());
                }
            }
            return logLevals;
        }
        private void AddLogComponents()
        {
            JObject data = new JObject();
            var levels = new JArray();
            data[CommonConst.CommonField.LOG_LOG_LEVELS] = levels;
            levels.Add(Loglevels.Debug.ToString());
            levels.Add(Loglevels.Info.ToString());
            levels.Add(Loglevels.Error.ToString());
            levels.Add(Loglevels.Transaction.ToString());
            AppSettingService.Instance.SetAppSetting(this.ToString(), data);
        }

        public JArray GetLogs(string transactionId)
        {
            return _dbProxy.Get(CommonConst.Collection.SERVER_LOGS,"{'" + CommonConst.CommonField.TRANSATTION_ID + "' : '" + transactionId + "'}");
        }
    }
    public enum Loglevels
    {
        Debug,
        Info,
        Error,
        Transaction
    }
}