using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;

namespace deOROSyncData
{
    public static class ApplicationConfig
    {

        private static string vmsProvider;

        public static string VmsProvider
        {
            get
            {
                ApplicationConfig.vmsProvider = (ConfigurationManager.GetSection("application") as NameValueCollection)["VmsProvider"];
                return vmsProvider; 
            }
            set { vmsProvider = value; }
        }

        private static string ftpHostName;
        public static string FtpHostName
        {
            get
            {
                ApplicationConfig.ftpHostName = (ConfigurationManager.GetSection("application") as NameValueCollection)["FtpHostName"];
                return ApplicationConfig.ftpHostName;
            }
            set { ApplicationConfig.ftpHostName = value; }
        }

        private static int ftpPort;
        public static int FtpPort
        {
            get
            {
                ApplicationConfig.ftpPort = Convert.ToInt32((ConfigurationManager.GetSection("application") as NameValueCollection)["FtpPort"]);
                return ApplicationConfig.ftpPort;
            }
            set { ApplicationConfig.ftpPort = value; }
        }

        private static string ftpUser;
        public static string FtpUser
        {
            get
            {
                ApplicationConfig.ftpUser = (ConfigurationManager.GetSection("application") as NameValueCollection)["FtpUser"];
                return ApplicationConfig.ftpUser;
            }
            set { ApplicationConfig.ftpUser = value; }
        }


        private static string ftpPassword;
        public static string FtpPassword
        {
            get
            {
                ApplicationConfig.ftpPassword = (ConfigurationManager.GetSection("application") as NameValueCollection)["FtpPassword"];
                return ApplicationConfig.ftpPassword;
            }
            set { ApplicationConfig.ftpPassword = value; }
        }

        private static string ftpCustomer;
        public static string FtpCustomer
        {
            get
            {
                ApplicationConfig.ftpCustomer = (ConfigurationManager.GetSection("application") as NameValueCollection)["FtpCustomer"];
                return ApplicationConfig.ftpCustomer;
            }
            set { ApplicationConfig.ftpCustomer = value; }
        }

        private static string ftpLocation;
        public static string FtpLocation
        {
            get
            {
                ApplicationConfig.ftpLocation = (ConfigurationManager.GetSection("application") as NameValueCollection)["FtpLocation"];
                return ApplicationConfig.ftpLocation;
            }
            set { ApplicationConfig.ftpLocation = value; }
        }


        private static int locationId;
        public static int LocationId
        {
            get
            {
                ApplicationConfig.locationId = Convert.ToInt32((ConfigurationManager.GetSection("application") as NameValueCollection)["LocationId"]);
                return ApplicationConfig.locationId;
            }
            set { ApplicationConfig.locationId = value; }
        }

        private static int locationIdBase;
        public static int LocationIdBase
        {
            get
            {
                ApplicationConfig.locationIdBase = Convert.ToInt32((ConfigurationManager.GetSection("application") as NameValueCollection)["LocationIdBase"]);
                return ApplicationConfig.locationIdBase;
            }
            set { ApplicationConfig.locationIdBase = value; }
        }

        private static int customerIdBase;
        public static int CustomerIdBase
        {
            get
            {
                ApplicationConfig.customerIdBase = Convert.ToInt32((ConfigurationManager.GetSection("application") as NameValueCollection)["CustomerIdBase"]);
                return ApplicationConfig.customerIdBase;
            }
            set { ApplicationConfig.customerIdBase = value; }
        }

        private static int customerId;
        public static int CustomerId
        {
            get
            {
                ApplicationConfig.customerId = Convert.ToInt32((ConfigurationManager.GetSection("application") as NameValueCollection)["CustomerId"]);
                return ApplicationConfig.customerId;
            }
            set { ApplicationConfig.customerId = value; }
        }
       
        private static string deOROServiceUrl;
        public static string DeOROServiceUrl
        {
            get
            {
                ApplicationConfig.deOROServiceUrl = (ConfigurationManager.GetSection("application") as NameValueCollection)["DeOROServiceUrl"];
                return ApplicationConfig.deOROServiceUrl;
            }
            set { ApplicationConfig.deOROServiceUrl = value; }
        }

        private static string deOROServiceUrlBase;
        public static string DeOROServiceUrlBase
        {
            get
            {
                ApplicationConfig.deOROServiceUrlBase = (ConfigurationManager.GetSection("application") as NameValueCollection)["DeOROServiceUrlBase"];
                return ApplicationConfig.deOROServiceUrlBase;
            }
            set { ApplicationConfig.deOROServiceUrlBase = value; }
        }

        private static string deOROServiceAccessUserName;
        public static string DeOROServiceAccessUserName
        {
            get
            {
                ApplicationConfig.deOROServiceAccessUserName = (ConfigurationManager.GetSection("application") as NameValueCollection)["DeOROServiceAccessUserName"];
                return ApplicationConfig.deOROServiceAccessUserName;
            }
            set { ApplicationConfig.deOROServiceAccessUserName = value; }
        }

        private static string deOROServiceAccessUserNameBase;
        public static string DeOROServiceAccessUserNameBase
        {
            get
            {
                ApplicationConfig.deOROServiceAccessUserNameBase = (ConfigurationManager.GetSection("application") as NameValueCollection)["DeOROServiceAccessUserNameBase"];
                return ApplicationConfig.deOROServiceAccessUserNameBase;
            }
            set { ApplicationConfig.deOROServiceAccessUserNameBase = value; }
        }

        private static string deOROServiceAccessPassword;
        public static string DeOROServiceAccessPassword
        {
            get
            {
                ApplicationConfig.deOROServiceAccessPassword = (ConfigurationManager.GetSection("application") as NameValueCollection)["DeOROServiceAccessPassword"];
                return ApplicationConfig.deOROServiceAccessPassword;
            }
            set { ApplicationConfig.deOROServiceAccessPassword = value; }
        }

        private static string deOROServiceAccessPasswordBase;
        public static string DeOROServiceAccessPasswordBase
        {
            get
            {
                ApplicationConfig.deOROServiceAccessPasswordBase = (ConfigurationManager.GetSection("application") as NameValueCollection)["DeOROServiceAccessPasswordBase"];
                return ApplicationConfig.deOROServiceAccessPasswordBase;
            }
            set { ApplicationConfig.deOROServiceAccessPasswordBase = value; }
        }

        private static bool userSharedAcrosssLocations;
        public static bool UserSharedAcrosssLocations
        {
            get
            {
                ApplicationConfig.userSharedAcrosssLocations = Convert.ToBoolean((ConfigurationManager.GetSection("application") as NameValueCollection)["UserSharedAcrosssLocations"]);
                return ApplicationConfig.userSharedAcrosssLocations;
            }
            set { ApplicationConfig.userSharedAcrosssLocations = value; }
        }

        private static bool canDownloadUsersFromServer;
        public static bool CanDownloadUsersFromServer
        {
            get
            {
                ApplicationConfig.canDownloadUsersFromServer = Convert.ToBoolean((ConfigurationManager.GetSection("application") as NameValueCollection)["CanDownloadUsersFromServer"]);
                return ApplicationConfig.canDownloadUsersFromServer;
            }
            set { ApplicationConfig.canDownloadUsersFromServer = value; }
        }

    }
}
