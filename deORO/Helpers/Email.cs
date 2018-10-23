using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp;
using deORODataAccessApp.Models;

namespace deORO.Helpers
{
    public class Email
    {
        private static Email instance;
        private string emailTemplate;

        private Email()
        {
            try
            {
                emailTemplate = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"\\Templates\EmailTemplate.html").ReadToEnd();
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


        public void SendEmailChanged()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your email address has been successfully changed on deORO microMARKET.");

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Email address changed notification", body);
        }

        public void SendPassword(string userName, string email, string password)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your password for deORO microMarket Login is <b>{0}</b>", password);

            string body = string.Format(emailTemplate, userName, sb.ToString());
            SendEmail(email, "deORO microMARKET - Password reminder notification", body);
        }

        public void SendBarcodeChanged()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your Barcode has been successfully changed on deORO microMARKET.");

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Barcode changed notification", body);
        }


        public void SendPasswordChanged()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your password has been successfully changed on deORO microMARKET.");

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Password changed notification", body);
        }

        public void SendFingerPrintChanged()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your fingerprints have been successfully changed on deORO microMARKET.");

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Fingerprints changed notification", body);
        }

        public void SendAccountRefilled(decimal refillAmount, decimal reward, string paymentMethod)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your account on deORO microMarket has been refilled with an amount of <b>{0}</b>. ", refillAmount.ToString("C2"));
            sb.AppendFormat("An additional <b>{0}</b> is also added as part of a reward. <br /> <br />", reward.ToString("C2"));
            sb.AppendFormat("Payment method used was <b>{0}</b>. <br /> <br />", paymentMethod);
            sb.AppendFormat("Remaining balance on your account is <b>{0}</b>. ", Global.User.AccountBalance.ToString("C2"));

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Account refill notification", body);
        }

        public void SendTransactionError(transaction_error error)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Id: " + error.pkid + "<br>");
            sb.AppendLine("Date Time: " + error.created_date_time + "<br>");
            sb.AppendLine("Source: " + error.source + "<br>");
            sb.AppendLine("Event: " + error.description + "<br>");

            string subject = string.Format("Transaction Error at CustomerId:{0} CustomerName:{1}, LocationId:{2} LocationName:{3}",
                                                Global.CustomerId, Global.CustomerName, Global.LocationId, Global.LocationName);
            SendEmail(Global.ToAddress, subject, sb.ToString(), Global.CcAddress);

        }

        public void SendPaymentComplete(string paymentMethod, List<ShoppingCartItem> items)
        {
            if (Global.User == null)
                return;

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Your shopping receipt from deORO microMARKET with an amount of <b>{0}</b>. ", items.Sum(x => x.PriceTaxIncluded).ToString("C2"));
            sb.AppendFormat("Payment method used was <b>{0}</b>. <br /> <br />", paymentMethod);

            sb.Append("<table class=\"tblShopping\" style=\"border-spacing: 0;border-color: whitesmoke;\" border=\"1\"><tr><th style=\"text-align:left;padding:0 5 0 5;\">BarCode</th><th style=\"text-align:left;\">Product</th><th style=\"text-align:right;padding:0 5 0 5;\">Price $</th><th style=\"text-align:right;padding:0 5 0 5;\">CRV $</th><th style=\"text-align:right;padding:0 5 0 5;\">Tax $</th><th style=\"text-align:right;padding:0 5 0 5;\">Total $</th><th style=\"text-align:right;padding:0 5 0 5;\">Original Price $</th><th style=\"text-align:right;padding:0 5 0 5;\">Original Tax $</th><th style=\"text-align:right;padding:0 5 0 5;\">Discount Price$</th><th style=\"text-align:right;padding:0 5 0 5;\">Discount Tax$</th></tr>");

            foreach (ShoppingCartItem item in items)
            {
                sb.AppendFormat("<tr><td style=\"text-align:right;padding:0 5 0 5;\">{0}</td><td style=\"text-align:left;\">{1}</td><td style=\"text-align:right;padding:0 5 0 5;\">{2}</td><td style=\"text-align:right;padding:0 5 0 5;\">{3}</td><td style=\"text-align:right;padding:0 5 0 5;\">{4}</td><td style=\"text-align:right;padding:0 5 0 5;\">{5}</td><td style=\"text-align:right;padding:0 5 0 5;\">{6}</td><td style=\"text-align:right;padding:0 5 0 5;\">{7}</td><td style=\"text-align:right;padding:0 5 0 5;\">{8}</td><td style=\"text-align:right;padding:0 5 0 5;\">{9}</td></tr>", item.BarCode, item.Name, item.Price.ToString("N2"), item.Crv.ToString("N2"), item.Tax.ToString("N2"), item.PriceTaxIncluded.ToString("N2"), item.OriginalPrice.ToString("N2"), item.OriginalTax.ToString("N2"),
                  item.DiscountPrice.ToString("N2"), item.DiscountTax.ToString("N2"));
            }

            sb.Append("<tr><th style=\"text-align:right;padding:0 5 0 5;\" colspan=\"2\"><b>Total</b></th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.Price).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.Crv).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.Tax).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.PriceTaxIncluded).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.OriginalPrice).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.OriginalTax).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.DiscountPrice).ToString("N2"));
            sb.Append("</th><th style=\"text-align:right;padding:0 5 0 5;\">");
            sb.Append(items.Sum(x => x.DiscountTax).ToString("N2"));
            sb.Append("</th></tr></table>");

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Purchase complete notification", body);
        }

        public void SendUserDeleted(decimal accountBalance, string firstName, string lastName, string address, string city, string state, string zip, string phone)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Your account <b>{0}</b> on deORO microMarket has been successfully deleted. ", Global.User.UserName);
            sb.AppendFormat("We will issue a refund of <b>{0}</b> using data you have supplied. <br /> <br />", accountBalance.ToString("C2"));

            sb.AppendFormat("{0} {1} <br />", firstName, lastName);
            sb.AppendFormat("{0} <br />", address);
            sb.AppendFormat("{0}, {1} {2} <br />", city, state, zip);
            sb.AppendFormat("{0}", phone);

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - User account deleted notification", body, Global.CcAddress);
        }

        public void SendUserCreated()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Creation of your account <b>{0}</b> on deORO microMarket has been successfully completed. <br /> <br />", Global.User.UserName);
            //sb.AppendFormat("Your password is <b>{0}</b>. ", Global.User.GetPassword());
            sb.Append("You can change it upon logging into deORO microMarket.");

            string body = string.Format(emailTemplate, Global.User.UserName, sb.ToString());
            SendEmail(Global.User.Email, "deORO microMARKET - Account created notification", body);
        }

        public void SendCoinShortFall(decimal shortFall)
        {

        }

        private void SendEmail(string toAddress, string subject, string body, string cc = "")
        {
            if (toAddress == null || toAddress.Trim() == "")
                return;


            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var message = new System.Net.Mail.MailMessage();
                        message.From = new System.Net.Mail.MailAddress(Global.FromMailAddress);
                        message.To.Add(toAddress);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;

                        if (!cc.Equals(""))
                            message.CC.Add(cc);

                        var smtpClient = new System.Net.Mail.SmtpClient();
                        smtpClient.Host = Global.SmptServer;
                        smtpClient.Port = Global.SmtpPort;
                        smtpClient.EnableSsl = Global.EnableSSL;
                        smtpClient.Credentials = new System.Net.NetworkCredential(Global.SmtpUserName, Global.SmtpPassword);

                        smtpClient.SendAsync(message, null);
                    }
                    catch { }
                });
        }

    }
}
