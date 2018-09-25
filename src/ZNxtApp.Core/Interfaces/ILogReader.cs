using Newtonsoft.Json.Linq;

namespace ZNxtApp.Core.Interfaces
{
    public interface ILogReader
    {
        JArray GetLogs(string transactionId);
    }
}