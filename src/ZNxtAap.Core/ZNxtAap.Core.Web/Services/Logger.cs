using System;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class Logger : ILogger
    {
        private static object lockObjet = new object();
        private static Logger _logger;
        private string _loggerName;
        private string _transactionId;

        public string TransactionId
        {
            get { return _transactionId; }
        }

        public static ILogger GetLogger(string loggerName)
        {
            if (_logger == null)
            {
                lock (lockObjet)
                {
                    _logger = new Logger();
                    _logger._loggerName = loggerName;
                }
            }
            return _logger;
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