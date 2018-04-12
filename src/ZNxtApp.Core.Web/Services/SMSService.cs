using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class SMSService : ISMSService, IFlushService
    {
        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly IActionExecuter _actionExecuter;
        private readonly IViewEngine _viewEngine;

        public SMSService(ILogger logger,
                          IDBService dbService,
                          IActionExecuter actionExecuter,
                          IViewEngine viewEngine)
        {
            _logger = logger;
            _actionExecuter = actionExecuter;
            _dbService = dbService;
            _viewEngine = viewEngine;
        }

        public bool Send(string toNumber, string text, bool putInQueue = true)
        {
            _logger.Error("TODO SMSService.Send");
            return true;
        }

        public bool Send(string toNumber, string textTemplate, Dictionary<string, dynamic> data, bool putInQueue = true)
        {
            _logger.Error("TODO SMSService.Send");
            return true;
        }

        public bool Flush()
        {
            _logger.Error("TODO SMSService.Flush");
            return true;
        }
    }
}
