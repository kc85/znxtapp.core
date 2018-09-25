using System;
using System.Linq;
using System.Net.Mail;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.Services.Api.Email
{
    public class EmailController : ApiBaseService
    {
        private ParamContainer _paramContainer;

        public EmailController(ParamContainer paramContainer) : base(paramContainer)
        {
            _paramContainer = paramContainer;
        }

        public bool Send()
        {
            try
            {
                string emailQueueId = _paramContainer.GetKey(CommonConst.CommonValue.EMAIL_QUEUE_ID);

                if (string.IsNullOrEmpty(emailQueueId))
                {
                    Logger.Error("Error : emailQueueId not found");
                    return false;
                }

                var emailData = DBProxy.FirstOrDefault(CommonConst.Collection.EMAIL_QUEUE, CommonConst.CommonField.DISPLAY_ID, emailQueueId);
                if (emailData == null)
                {
                    Logger.Error(string.Format("Error : emailData not found for id : {0}", emailQueueId));
                    return false;
                }

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(AppSettingService.GetAppSettingData(CommonConst.CommonField.SMTP_SERVER));

                mail.From = new MailAddress(emailData[CommonConst.CommonField.FROM].ToString());
                foreach (var item in emailData[CommonConst.CommonField.TO])
                {
                    mail.To.Add(item.ToString());
                }

                mail.Subject = emailData[CommonConst.CommonField.SUBJECT].ToString();
                mail.Body = emailData[CommonConst.CommonField.BODY].ToString();
                mail.IsBodyHtml = true;
                int port = 587;
                int.TryParse(AppSettingService.GetAppSettingData(CommonConst.CommonField.SMTP_SERVER_PORT), out port);
                SmtpServer.Port = port;
                var user = AppSettingService.GetAppSettingData(CommonConst.CommonField.SMTP_SERVER_USER);
                var password = AppSettingService.GetAppSettingData(CommonConst.CommonField.SMTP_SERVER_PASSWORD);
                SmtpServer.Credentials = new System.Net.NetworkCredential(user, password);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error sending email : {0}", ex.Message), ex);
                return false;
            }
        }
    }
}