using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using NLog.Config;
using deORO.Helpers;


namespace deORO.Helpers
{
    public class NLogConfig
    {
        public static void CreateMailTarget(LoggingConfiguration config)
        {

            var mailTarget = new MailTarget();
            
            config.AddTarget("mail", mailTarget);

            mailTarget.SmtpServer = Global.SmptServer;
            mailTarget.SmtpPort = Global.SmtpPort;
            mailTarget.SmtpAuthentication = (SmtpAuthenticationMode)Global.SmtpAuthenticationMode;
            mailTarget.SmtpUserName = Global.SmtpUserName;
            mailTarget.SmtpPassword = Global.SmtpPassword;
            mailTarget.EnableSsl = Global.EnableSSL;
            mailTarget.To = Global.ToAddress;
            mailTarget.CC = Global.CcAddress;
            mailTarget.From = Global.FromMailAddress;
            mailTarget.Subject = string.Format("Application Error at CustomerId:{0} CustomerName:{1}, LocationId:{2} LocationName:{3}",
                                                Global.CustomerId, Global.CustomerName, Global.LocationId, Global.LocationName);

            var rule = new LoggingRule("*", LogLevel.Error, mailTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;

        }

        public static void CreateFileTarget(LoggingConfiguration config)
        {

            var fileTarget = new FileTarget();

            config.AddTarget("file", fileTarget);
            fileTarget.Layout = "${longdate} ${logger} ${message}";
            fileTarget.FileName = @"C:\deORO\Logs\${shortdate}.log";

            var rule = new LoggingRule("*", LogLevel.Error, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;

        }
    }
}
