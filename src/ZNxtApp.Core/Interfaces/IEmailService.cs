using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ZNxtApp.Core.Interfaces
{
    public interface IEmailService
    {
        bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, string subject, Dictionary<string, dynamic> data);

        bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, string subject, JObject data);
    }
}