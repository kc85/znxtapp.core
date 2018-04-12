using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface IEmailService
    {
        bool Send(string toEmail, string fromEmail, string emailBody);
        bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, Dictionary<string,dynamic> data);
    }
}
