using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Web.Helper;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Model;
using System;

namespace ZNxtApp.Core.Web.Services
{
    public class EmailService : IEmailService, IFlushService
    {

        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly IActionExecuter _actionExecuter;
        private readonly IViewEngine _viewEngine;
        private readonly ParamContainer _paramContainer;

        public EmailService(ILogger logger,
                         IDBService dbService,
                         IActionExecuter actionExecuter,
                         IViewEngine viewEngine,
            ParamContainer paramContainer)
        {
            _logger = logger;
            _actionExecuter = actionExecuter;
            _dbService = dbService;
            _viewEngine = viewEngine;
            _paramContainer = paramContainer;
        }


        public bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, string subject, Dictionary<string, dynamic> modelData)
        {
            var emailTemplateData = _dbService.FirstOrDefault(CommonConst.Collection.EMAIL_TEMPLATE, CommonConst.CommonField.DATA_KEY, emailTemplate);
            if (emailTemplateData != null && emailTemplateData[CommonConst.CommonField.BODY] != null)
            {
                ServerPageModelHelper.AddBaseData(modelData);
                var emailBody = _viewEngine.Compile(emailTemplateData[CommonConst.CommonField.BODY].ToString(), emailTemplate, modelData);
                return Send(toEmail, fromEmail, CC, emailBody, subject);
            }
            else
            {
                _logger.Error(string.Format("Error Unable to find Email template {0}", emailTemplate));
                return false;
            }
        }
         public bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailTemplate, string subject, JObject data)
        {
            Dictionary<string, dynamic> dataObj = new Dictionary<string, dynamic>();
            foreach (var item in data)
            {
                dataObj[item.Key] = item.Value;
            }
            return Send(toEmail, fromEmail, CC, emailTemplate, subject, dataObj);
        }
        public bool Send(List<string> toEmail, string fromEmail, List<string> CC, string emailBody, string subject)
        {
            JObject emailData = new JObject();
            emailData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            emailData[CommonConst.CommonField.FROM] = fromEmail;
            emailData[CommonConst.CommonField.SUBJECT] = subject;
            emailData[CommonConst.CommonField.TO] = new JArray();
            foreach (var email in toEmail)
            {
                (emailData[CommonConst.CommonField.TO] as JArray).Add(email);
            }
            emailData[CommonConst.CommonField.CC] = new JArray();
            if (CC != null)
            {
                foreach (var email in CC)
                {
                    (emailData[CommonConst.CommonField.CC] as JArray).Add(email);
                }
            }
            emailData[CommonConst.CommonField.BODY] = emailBody;
            emailData[CommonConst.CommonField.STATUS] = EmailStatus.Queue.ToString();

            if (_dbService.Write(CommonConst.Collection.EMAIL_QUEUE, emailData))
            {
                Dictionary<string, string> filter = new Dictionary<string, string>();
                filter[CommonConst.CommonField.DISPLAY_ID] = emailData[CommonConst.CommonField.DISPLAY_ID].ToString();

                var route = Routings.Routings.GetRoutings().GetRoute(CommonConst.ActionMethods.ACTION, "/api/email/send");
                if (route != null)
                {
                    _paramContainer.AddKey(CommonConst.CommonValue.EMAIL_QUEUE_ID, () => { return emailData[CommonConst.CommonField.DISPLAY_ID].ToString(); });
                    var emailResult = (bool)_actionExecuter.Exec(route, _paramContainer);
                    if (emailResult)
                    {
                        emailData[CommonConst.CommonField.STATUS] = EmailStatus.Sent.ToString();
                    }
                    else
                    {
                        emailData[CommonConst.CommonField.STATUS] = EmailStatus.SendError.ToString();
                    }
                    _dbService.Write(CommonConst.Collection.EMAIL_QUEUE, emailData, filter);
                    return emailResult;
                }
                else
                {
                    _logger.Error("Email sender route not found, Please install  email module ");
                    return false;

                }
            }
            else
            {
                _logger.Error("Error in add email data in queue");
                return false;

            }
        }
        public bool Flush()
        {
            _logger.Error("TODO EmailService.Flush");
            throw new NotImplementedException();
        }

    }
}
