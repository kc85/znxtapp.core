using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Web.Helper;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Web.Services
{
    public class SMSService : ISMSService, IFlushService
    {
        private readonly ILogger _logger;
        private readonly IDBService _dbService;
        private readonly IActionExecuter _actionExecuter;
        private readonly IViewEngine _viewEngine;
        const string SMS_QUEUE_ID = "sms_queue_id";
        private readonly ParamContainer _paramContainer;
        public SMSService(ILogger logger,
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

        public bool Send(string toNumber, string text, bool putInQueue = true)
        {
            _logger.Error("TODO SMSService.Send");
            return true;
        }

        public bool Send(string toNumber, string textTemplate, Dictionary<string, dynamic> modelData, bool putInQueue = true)
        {
            var smsTemplateData = _dbService.FirstOrDefault(CommonConst.Collection.SMS_TEMPLATE, CommonConst.CommonField.DATA_KEY, textTemplate);
            if (smsTemplateData != null && smsTemplateData[CommonConst.CommonField.BODY] != null)
            {
                ServerPageModelHelper.AddBaseData(modelData);
                modelData[CommonConst.CommonField.PHONE_FIELD] = toNumber;
                var textSMSData = _viewEngine.Compile(smsTemplateData[CommonConst.CommonField.BODY].ToString(), textTemplate, modelData);
                JObject smsData = new JObject();
                smsData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                smsData[CommonConst.CommonField.PHONE_FIELD] = toNumber;
                smsData[CommonConst.CommonField.BODY] = textSMSData;
                smsData[CommonConst.CommonField.STATUS] = SMSStatus.Queue.ToString();

                if (_dbService.Write(CommonConst.Collection.SMS_QUEUE, smsData))
                {

                    var route = Routings.Routings.GetRoutings().GetRoute(CommonConst.ActionMethods.ACTION, "/api/sms/send");
                    if (route != null)
                    {
                        _paramContainer.AddKey(SMS_QUEUE_ID, () => { return smsData[CommonConst.CommonField.DISPLAY_ID].ToString(); });
                        var obj = _actionExecuter.Exec(route, _paramContainer);
                        return (bool)obj;

                    }
                    else
                    {
                        _logger.Error("SMS sender route not found, Please install  sms module ");
                        return false;

                    }
                }
                else
                {
                    _logger.Error(string.Format("Error  data write fail on SMS_QUEUE"));
                    return false;
                }
            }
            else
            {
                _logger.Error(string.Format("Error Unable to find SMS template {0}", textTemplate));
                return false;
            }
        }

        public bool Send(string toNumber, string smsTemplate, JObject data, bool putInQueue = true)
        {

            Dictionary<string, dynamic> modelData = new Dictionary<string, dynamic>();

            foreach (var item in data)
            {
                modelData[item.Key] = item.Value;
            }
            return Send(toNumber, smsTemplate, modelData, putInQueue);
        }
        public bool Flush()
        {
            _logger.Error("TODO SMSService.Flush");
            return true;
        }
    }
}
