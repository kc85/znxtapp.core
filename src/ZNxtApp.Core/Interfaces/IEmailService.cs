using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface IEmailService
    {
        bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, string subject, Dictionary<string, dynamic> data);
        bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, string subject, JObject data);
    }
}
