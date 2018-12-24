using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Net;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Module.SMS.TextLocal.Services.Api
{
    public static class TextLocalSMSHelper
    {
        public static bool SendSMS(string smsbody, string to, string apikey, string endpoint, string from, ILogger logger)
        {
            try
            {
                logger.Debug(string.Format("TextLocalSMSHelper.SendSMS phone: {0}, EndPoint: {1}", to, endpoint));
                String messageData = smsbody;
                using (var wb = new WebClient())
                {
                    byte[] response = wb.UploadValues(endpoint, new NameValueCollection()
                        {
                        {"apikey" , apikey},
                        {"numbers" , to},
                        {"message" , messageData},
                        {"sender" , from}
                        });

                    string result = System.Text.Encoding.UTF8.GetString(response);

                    logger.Debug(string.Format("SMS sender resonse{0}", result));

                    JObject responseData = JObject.Parse(result);
                    var status = responseData["status"].ToString();
                    if (status == "success")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Error in SendSMS : {0}", ex.Message), ex);
                return false;
            }
        }
    }
}