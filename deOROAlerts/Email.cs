using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace deOROAlerts
{
    public class Email
    {
        private static Email instance;
        private string emailTemplate;

        private Email()
        {
            try
            {
                emailTemplate = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"EmailTemplate.html").ReadToEnd();
            }
            catch { }
        }

        public static Email Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Email();
                }
                return instance;
            }
        }

        public void SendEmail(string toAddress, string subject, string body = "", string cc = "", string [] filesPathArray = null)
        {
            if (toAddress == null || toAddress.Trim() == "")
                return;

            body = string.Format(emailTemplate, body);


            try
            {
                var message = new System.Net.Mail.MailMessage();
                message.From = new System.Net.Mail.MailAddress(System.Configuration.ConfigurationSettings.AppSettings["FromMailAddress"]);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;


                if (filesPathArray != null)
                {
                    foreach (string filePath in filesPathArray)
                    {
                        message.Attachments.Add(new Attachment(filePath));
                    }
                }
               
                
                if (!cc.Equals(""))
                    message.CC.Add(cc);

                var smtpClient = new System.Net.Mail.SmtpClient();
                smtpClient.Host = System.Configuration.ConfigurationSettings.AppSettings["SmtpServer"];
                smtpClient.Port = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["SmtpPort"]);
                smtpClient.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["EnableSSL"]);
                smtpClient.Credentials = new System.Net.NetworkCredential(
                                                                          System.Configuration.ConfigurationSettings.AppSettings["SmtpUserName"],
                                                                          System.Configuration.ConfigurationSettings.AppSettings["SmtpPassword"]);

                smtpClient.Send(message);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
