using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Web.Models
{
    public class EventSubscriptionModel
    {
        public string EventName { get; set; }
        public ExecutionEventType EventType { get; set; }
        public string ExecultAssembly { get; set; }
        public string ExecuteType { get; set; }
        public string ExecuteMethod { get; set; }
    }
}
