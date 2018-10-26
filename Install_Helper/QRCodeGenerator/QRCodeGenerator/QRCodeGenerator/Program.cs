using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ZXing;
using System.Drawing;
using System.Net.Mail;

namespace QRCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

            deORO_LocalEntities entities = new QRCodeGenerator.deORO_LocalEntities();
            var users = entities.users;

            var email = Email.Instance;

            foreach (var user in users)
            {
                try
                {
                    log.Log(NLog.LogLevel.Info, "Processing " + user.username);
                    string content = "deORO_" + Guid.NewGuid();
                    string imagePath = Helper.GetQRCode(content);

                    email.SendPassword(user.username, user.email, user.password, imagePath);

                    user.barcode = content;
                    entities.Entry<user>(user).State = System.Data.EntityState.Modified;

                    log.Log(NLog.LogLevel.Info, "Done Processing " + user.username + "\r\n");
                }
                catch(Exception ex)
                {
                    log.Log(NLog.LogLevel.Error, "Error while Processing " + user.username);
                    log.Log(NLog.LogLevel.Error, ex.ToString() + "\r\n");
                }
            }

            entities.SaveChanges();
        }
    }

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

        public void SendPassword(string userName, string email, string password, string imagePath)
        {
            string body = string.Format(emailTemplate, userName, email);
            SendEmail(email, "deORO microMARKET - Password reminder notification", body, imagePath);
        }

        private void SendEmail(string toAddress, string subject, string body, string imagePath, string cc = "")
        {
            if (toAddress == null || toAddress.Trim() == "")
                return;

            try
            {
                var message = new System.Net.Mail.MailMessage();
                message.From = new System.Net.Mail.MailAddress(ConfigurationSettings.AppSettings["FromMailAddress"]);
                message.To.Add(toAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                Attachment attachment = new Attachment(imagePath);
                attachment.ContentId = toAddress; 
                message.Attachments.Add(attachment);

                if (!cc.Equals(""))
                    message.CC.Add(cc);

                var smtpClient = new System.Net.Mail.SmtpClient();
                smtpClient.Host = ConfigurationSettings.AppSettings["SmtpServer"];
                smtpClient.Port = Convert.ToInt32(ConfigurationSettings.AppSettings["SmtpPort"]);
                smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationSettings.AppSettings["EnableSSL"]);
                smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings["SmtpUserName"], ConfigurationSettings.AppSettings["SmtpPassword"]);

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }

    public class Helper
    {
        public static string GetQRCode(string content)
        {
            try
            {
                IBarcodeWriter writer = new BarcodeWriter
                { Format = BarcodeFormat.QR_CODE };

                var result = writer.Write(content);
                var barcodeBitmap = new Bitmap(result);

                string fileName = Guid.NewGuid() + ".bmp";
                barcodeBitmap.Save(fileName);

                return fileName;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }

}
