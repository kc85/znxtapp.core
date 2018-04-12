using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class EmailService : IEmailService, IFlushService
    {

        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly IActionExecuter _actionExecuter;
        private readonly IViewEngine _viewEngine;

        public EmailService(ILogger logger,
                         IDBService dbService,
                         IActionExecuter actionExecuter,
                         IViewEngine viewEngine)
        {
            _logger = logger;
            _actionExecuter = actionExecuter;
            _dbService = dbService;
            _viewEngine = viewEngine;
        }
        public bool Send(string toEmail, string fromEmail, string emailBody)
        {
            _logger.Error("TODO EmailService.Send");
            throw new NotImplementedException();
        }

        public bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, Dictionary<string, dynamic> data)
        {
            _logger.Error("TODO EmailService.Send");
            throw new NotImplementedException();
        }
        public bool Flush()
        {
            _logger.Error("TODO EmailService.Flush");
            throw new NotImplementedException();
        }

    }
}
