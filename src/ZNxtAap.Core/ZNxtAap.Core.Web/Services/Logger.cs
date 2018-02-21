using System;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class Logger : ILogger
    {
        private static object lockObjet = new object();
        private string _loggerName;
        private string _transactionId;

        private IDBService _dbProxy;
        public string TransactionId
        {
            get { return _transactionId; }
        }

        public Logger()
        {
            _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName); ;
        }
        public static ILogger GetLogger(string loggerName)
        {
            lock (lockObjet)
            {
                Logger _logger = new Logger();
                _logger._loggerName = loggerName;
                return _logger;
            }
        }

        public void Debug(string message, Newtonsoft.Json.Linq.JObject logData = null)
        {

        }

        public void Error(string message, Exception ex = null)
        {
        }

        public void Error(string message, Exception ex = null, Newtonsoft.Json.Linq.JObject logData = null)
        {
        }

        public void Info(string message, Newtonsoft.Json.Linq.JObject logData = null)
        {

        }

        public void Transaction(Newtonsoft.Json.Linq.JObject transactionData, TransactionState state)
        {

        }
    }
}