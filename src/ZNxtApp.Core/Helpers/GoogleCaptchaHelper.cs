using Newtonsoft.Json.Linq;
using System.Net;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Helpers
{
    public static class GoogleCaptchaHelper
    {
        public static bool ValidateResponse(ILogger logger, string captchaResponse, string secret, string validateUrl)
        {
            var client = new WebClient();
            var reply = client.DownloadString(string.Format("{0}?secret={1}&response={2}", validateUrl, secret, captchaResponse));

            JObject reponse = JObject.Parse(reply);
            bool isValid = false;
            bool.TryParse(reponse["success"].ToString(), out isValid);

            logger.Debug(reply);

            return isValid;
        }
    }
}