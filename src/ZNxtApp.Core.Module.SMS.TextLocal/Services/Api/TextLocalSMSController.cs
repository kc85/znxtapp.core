using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.SMS.TextLocal.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.SMS.TextLocal.Services.Api
{
    public class TextLocalSMSController : ApiBaseService
    {
        private ParamContainer _paramContainer;

        public TextLocalSMSController(ParamContainer paramContainer) : base(paramContainer)
        {
            _paramContainer = paramContainer;
        }

        public bool Send()
        {
            Logger.Info("Start TextLocalSMSController.Send");
            string sms_id = _paramContainer.GetKey(TextLocalConsts.SMS_QUEUE_ID);
            if (string.IsNullOrEmpty(sms_id))
            {
                Logger.Error("sms_id is null");
                ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
            var smsData = DBProxy.FirstOrDefault(CommonConst.Collection.SMS_QUEUE, CommonConst.CommonField.DISPLAY_ID, sms_id);

            if (smsData == null)
            {
                Logger.Error("sms_data is null");
                ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

            var phone = smsData[CommonConst.CommonField.PHONE].ToString();
            var textSMSData = smsData[CommonConst.CommonField.BODY].ToString();

            string apiKey = AppSettingService.GetAppSettingData(TextLocalConsts.TEXT_LOCAL_SMS_GATEWAY_KEY);
            string endPoint = AppSettingService.GetAppSettingData(TextLocalConsts.TEXT_LOCAL_SMS_GATEWAY_ENDPOINT);
            string fromPhone = AppSettingService.GetAppSettingData(TextLocalConsts.SMS_FROM);

            return TextLocalSMSHelper.SendSMS(textSMSData, phone, apiKey, endPoint, fromPhone, Logger);
        }
    }
}