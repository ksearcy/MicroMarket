using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace deOROWeb.Helper
{
    public class EmailHelper
    {

        private static EmailHelper instance;
        private string emailTemplate;

        private EmailHelper()
        {
            try
            {
                emailTemplate = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\\Templates\EmailTemplate.html").ReadToEnd();
            }
            catch { }
        }

        public static EmailHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmailHelper();
                }
                return instance;
            }
        }


        public void SendPassword(string emailAddress, string userName, string password)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your password <b>{0}</b>", password);

            string body = string.Format(emailTemplate, userName, sb.ToString());
            SendEmail(emailAddress, "deORO microMARKET - Password reminder notification", body);
        }


        private void SendEmail(string toAddress, string subject, string body, string cc = "")
        {

            var message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings.Get("FromMailAddress"));
            message.To.Add(toAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            if (!cc.Equals(""))
                message.CC.Add(cc);

            var smtpClient = new System.Net.Mail.SmtpClient();
            smtpClient.Host = System.Configuration.ConfigurationManager.AppSettings.Get("SmtpServer");
            smtpClient.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("SmtpPort"));
            smtpClient.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("EnableSSL"));
            smtpClient.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings.Get("SmtpUserName"),
                                                                      System.Configuration.ConfigurationManager.AppSettings.Get("SmtpPassword"));

            smtpClient.Send(message);

        }
    }
}