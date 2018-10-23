using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel.Security;
using System.ServiceModel;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp;
using System.IO;
using WinSCP;
using deOROSyncData;



namespace deOROFtp
{
    public class FtpSend
    {

        //static string FtpSyncLogLocal ;
        //public string FtpSyncLogPublic;


        private static string ftpSyncLogPublic;

        public string FtpSyncLogPublic
        {
            get { return ftpSyncLogPublic; }
            set { ftpSyncLogPublic = value; }
        }

        public void SyncFTPMainMethod()
        {
            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = ApplicationConfig.FtpHostName,
                    PortNumber = ApplicationConfig.FtpPort,
                    UserName = ApplicationConfig.FtpUser,
                    Password = ApplicationConfig.FtpPassword,
                  
                };

                using (Session session = new Session())
                {
                    // Will continuously report progress of synchronization
                    session.FileTransferred += FileTransferred;

                    // Connect
                    session.Open(sessionOptions);

                    string ftpCustomer = ApplicationConfig.FtpCustomer;
                    string ftpLocation = ApplicationConfig.FtpLocation;
                    string ftpLocalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"iSpy\WebServerRoot\Media\video");


                    // Synchronize files
                    SynchronizationResult synchronizationResult;
                    synchronizationResult =
                        session.SynchronizeDirectories(
                        SynchronizationMode.Remote, ftpLocalPath, @"video\"+ftpCustomer+@"\"+ftpLocation, true);

                    // Throw on any error
                    synchronizationResult.Check();
                }
               //return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                ftpSyncLogPublic = ftpSyncLogPublic + "\r\n\r\n" + @"Error:" + e + "\r\n\r\n";
                //return 1;
            }

        }



        private static void FileTransferred(object sender, TransferEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Upload of {0} succeeded", e.FileName);

                ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Upload of " + e.FileName + " succeeded";
            }
            else
            {
                Console.WriteLine("Upload of {0} failed: {1}", e.FileName, e.Error);

                ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Upload of " + e.FileName + " failed: " + e.Error;
            }

            if (e.Chmod != null)
            {
                if (e.Chmod.Error == null)
                {
                    Console.WriteLine("Permisions of {0} set to {1}", e.Chmod.FileName, e.Chmod.FilePermissions);

                    ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Permisions of " + e.Chmod.FileName + " set to " + e.Chmod.FilePermissions;
                   
                }
                else
                {
                    Console.WriteLine("Setting permissions of {0} failed: {1}", e.Chmod.FileName, e.Chmod.Error);

                    ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Setting permissions of " + e.Chmod.FileName + " failed: " + e.Chmod.Error;
                }
            }
            else
            {
                Console.WriteLine("Permissions of {0} kept with their defaults", e.Destination);

                ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Permissions of " + e.Destination + " kept with their defaults";
            }

            if (e.Touch != null)
            {
                if (e.Touch.Error == null)
                {
                    Console.WriteLine("Timestamp of {0} set to {1}", e.Touch.FileName, e.Touch.LastWriteTime);

                    ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Timestamp of " + e.Touch.FileName + " set to " + e.Touch.LastWriteTime;
                }
                else
                {
                    Console.WriteLine("Setting timestamp of {0} failed: {1}", e.Touch.FileName, e.Touch.Error);

                    ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Setting timestamp of " + e.Touch.FileName + " failed: " + e.Touch.Error;
                }
            }
            else
            {
                // This should never happen during "local to remote" synchronization
                Console.WriteLine("Timestamp of {0} kept with its default (current time)", e.Destination);

                ftpSyncLogPublic = ftpSyncLogPublic + "\r\n" + @"Timestamp of  " + e.Destination + " kept with its default (current time)";
            }
        }
    }
  
}



