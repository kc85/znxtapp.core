using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface ISMSService
    {
        bool Send(string toNumber, string text, bool putInQueue = true);
        bool Send(string toNumber, string smsTemplate, Dictionary<string, dynamic> data, bool putInQueue = true);
        
    }
}
