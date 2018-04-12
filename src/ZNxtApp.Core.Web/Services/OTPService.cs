using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Web.Services
{
    public class OTPService : IOTPService
    {
        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailService;

        public OTPService(ILogger logger,
                          IDBService dbService,
                          ISMSService smsService, 
                          IEmailService emailService)
        {
            _logger = logger;
            _smsService = smsService;
            _dbService = dbService;
            _emailService = emailService;
        }

        public OTPData Read(string phoneNumber, OTPType otpType)
        {
            _logger.Error("TODO OTPService.Read");

            return new OTPData();
        }

        public bool Send(string phoneNumber, string smsTemplate, OTPType otpType)
        {
            _logger.Error("TODO OTPService.Send");
            return true;
        }
    }
}
