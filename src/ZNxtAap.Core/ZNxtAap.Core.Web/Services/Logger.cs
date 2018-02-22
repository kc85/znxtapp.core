using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class Logger : ILogger,ILogReader
    {
        private static object lockObjet = new object();
        private string _loggerName;
        private string _transactionId;
        private IDBService _dbProxy; 
        List<string> _logLevels;
        
        public string TransactionId
        {
            get { return _transactionId; }
        }

        public Logger()
        {
            _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.SERVER_LOGS);
            _logLevels = GetLogLevels();
        }
        public static ILogger GetLogger(string loggerName, string transactionId)
        {
            lock (lockObjet)
            {
                Logger _logger = new Logger();
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
            _dbProxy.WriteData(logData);
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
            return _dbProxy.Get("{'" + CommonConst.CommonField.TRANSATTION_ID + "' : '" + transactionId + "'}");
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