using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WinSCP;

namespace deOROSyncData
{
    class UsgFtpSend
    {
        //static string FtpSyncLogLocal ;
        //public string FtpSyncLogPublic;


        private static string usgftpSyncLogPublic;

        public string UsgFtpSyncLogPublic
        {
            get { return usgftpSyncLogPublic; }
            set { usgftpSyncLogPublic = value; }
        }

        public void SyncUsgFTPMainMethod()
        {
            try
            {
                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = ApplicationConfig.UsgFtpHostName,
                    PortNumber = ApplicationConfig.UsgFtpPort,
                    UserName = ApplicationConfig.UsgFtpUser,
                    Password = ApplicationConfig.UsgFtpPassword,

                };

                using (Session session = new Session())
                {
                    // Will continuously report progress of synchronization
                    session.FileTransferred += FileTransferred;

                    // Connect
                    session.Open(sessionOptions);

                    string usgftpCustomer = ApplicationConfig.UsgFtpCustomer;
                    string usgftpLocation = ApplicationConfig.UsgFtpLocation;
                    string usgftpLocalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"iSpy\WebServerRoot\Media\video");

                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = WinSCP.TransferMode.Binary;

                    TransferOperationResult transferResult;
                    transferResult = session.PutFiles(@"c:\deORO\SyncData\Usg", usgftpLocation, false, transferOptions);

                    //// Synchronize files
                    //SynchronizationResult synchronizationResult;
                    //synchronizationResult =
                    //    session.SynchronizeDirectories(
                    //    SynchronizationMode.Remote, usgftpLocalPath, @"video\"+usgftpCustomer+@"\"+usgftpLocation, true);

                    //// Throw on any error
                    //synchronizationResult.Check();
                }
                //return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n\r\n" + @"Error:" + e + "\r\n\r\n";
                //return 1;
            }

        }



        private static void FileTransferred(object sender, TransferEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Upload of {0} succeeded", e.FileName);

                usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Upload of " + e.FileName + " succeeded";
            }
            else
            {
                Console.WriteLine("Upload of {0} failed: {1}", e.FileName, e.Error);

                usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Upload of " + e.FileName + " failed: " + e.Error;
            }

            if (e.Chmod != null)
            {
                if (e.Chmod.Error == null)
                {
                    Console.WriteLine("Permisions of {0} set to {1}", e.Chmod.FileName, e.Chmod.FilePermissions);

                    usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Permisions of " + e.Chmod.FileName + " set to " + e.Chmod.FilePermissions;

                }
                else
                {
                    Console.WriteLine("Setting permissions of {0} failed: {1}", e.Chmod.FileName, e.Chmod.Error);

                    usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Setting permissions of " + e.Chmod.FileName + " failed: " + e.Chmod.Error;
                }
            }
            else
            {
                Console.WriteLine("Permissions of {0} kept with their defaults", e.Destination);

                usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Permissions of " + e.Destination + " kept with their defaults";
            }

            if (e.Touch != null)
            {
                if (e.Touch.Error == null)
                {
                    Console.WriteLine("Timestamp of {0} set to {1}", e.Touch.FileName, e.Touch.LastWriteTime);

                    usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Timestamp of " + e.Touch.FileName + " set to " + e.Touch.LastWriteTime;
                }
                else
                {
                    Console.WriteLine("Setting timestamp of {0} failed: {1}", e.Touch.FileName, e.Touch.Error);

                    usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Setting timestamp of " + e.Touch.FileName + " failed: " + e.Touch.Error;
                }
            }
            else
            {
                // This should never happen during "local to remote" synchronization
                Console.WriteLine("Timestamp of {0} kept with its default (current time)", e.Destination);

                usgftpSyncLogPublic = usgftpSyncLogPublic + "\r\n" + @"Timestamp of  " + e.Destination + " kept with its default (current time)";
            }
        }
    }
}

