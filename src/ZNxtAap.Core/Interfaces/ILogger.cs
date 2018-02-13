using Newtonsoft.Json.Linq;
using System;

namespace ZNxtAap.Core.Interfaces
{
    public interface ILogger
    {
        void Debug(string message, JObject logData = null);

        void Error(string message, Exception ex = null);

        void Error(string message, Exception ex = null, JObject logData = null);

        void Info(string message, JObject logData = null);

        void Transaction(JObject transactionData, TransactionState state);

        string TransactionId { get; }
    }

    public enum TransactionState
    {
        Start,
        Finish
    }
}