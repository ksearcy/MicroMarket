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
using System.Data.SqlClient;



namespace deOROFtp
{
    public class FtpUsgSend
    {

        //static string FtpSyncLogLocal ;
        //public string FtpSyncLogPublic;


        private static string ftpusgSyncLogPublic;

        public string FtpUsgSyncLogPublic
        {
            get { return ftpusgSyncLogPublic; }
            set { ftpusgSyncLogPublic = value; }
        }

        private void CreateCSVFile() {


            string LastFileWriteDate = "01/01/1999 12:00:00 AM";
            string TodayDate = DateTime.Now.ToString();

            string PathCSVFiles = "C:\\deORO\\CSV Files";

            try
            {
                if (!Directory.Exists(PathCSVFiles))
                {
                    Directory.CreateDirectory(PathCSVFiles);
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var CSVFilesDirectory = new DirectoryInfo(PathCSVFiles);
                var LastFile = (from f in CSVFilesDirectory.GetFiles()
                                orderby f.LastWriteTime descending
                                select f).First();
                LastFileWriteDate = LastFile.LastWriteTime.ToString();
            }
            catch (Exception ex)
            {


            }

            //string CSVEmptyFilePath = Path.Combine(Environment.CurrentDirectory, @"CSV Template\CSV Empty File.csv");
            string CSVEmptyFilePath = @"C:\\deORO\\CSV Template\\CSV Empty File.csv";

            System.IO.FileInfo FileExists = new System.IO.FileInfo(CSVEmptyFilePath);

            if (FileExists.Exists != false)
            {

                string connectionString = "Data Source=.;Initial Catalog=deORO_Local;Integrated Security=True;";

                SqlConnection LocalDBConnection = new SqlConnection(connectionString);

                string SQLCommand = "SELECT A.[userpkid],A.[pkid],A.[created_date_time], B.[barcode], C.[description], C.[quantity], B.[price], B.[Tax],B.[Tax]  FROM [shoppingcart] AS A JOIN [shoppingcartdetail] AS B ON A.[pkid] = B.[shoppingcartpkid] JOIN [item] AS C ON B.[itemid]= C.[id] where A.[created_date_time] > @fromDate AND A.[created_date_time] < @toDate";

                SqlCommand SelectData = new SqlCommand(SQLCommand, LocalDBConnection);

                SelectData.Parameters.AddWithValue("@fromDate", LastFileWriteDate);
                SelectData.Parameters.AddWithValue("@toDate", TodayDate);

                LocalDBConnection.Open();

                var dataReader = SelectData.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                string[][] CSVFile = new string[dataTable.Rows.Count][];

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    CSVFile[i] = new string[] { "1010", "2020", "3030", dataTable.Rows[i]["userpkid"].ToString(), dataTable.Rows[i]["pkid"].ToString(), dataTable.Rows[i]["created_date_time"].ToString(), dataTable.Rows[i]["barcode"].ToString(), dataTable.Rows[i]["description"].ToString(), "1", dataTable.Rows[i]["price"].ToString(), dataTable.Rows[i]["Tax"].ToString(), "NULL" };
                  
                }

                LocalDBConnection.Close();

                string NewCSVFilePath = PathCSVFiles + "\\CSV Report " + DateTime.Now.ToString() + ".csv";

                NewCSVFilePath = NewCSVFilePath.Replace("/", "-");

                NewCSVFilePath = NewCSVFilePath.Replace(":", "_");

                NewCSVFilePath = NewCSVFilePath.Replace("C_", "C:");

                File.Copy(CSVEmptyFilePath, NewCSVFilePath);

                string delimiter = ",";


                int length = CSVFile.GetLength(0);
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < length; index++)
                    sb.AppendLine(string.Join(delimiter, CSVFile[index]));

                File.WriteAllText(NewCSVFilePath, sb.ToString());

            }
        
        }

        public void SyncFtpUsgMainMethod()
        {

            try
            {
                CreateCSVFile();


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
                    session.FileTransferred += UsgFileTransferred;

                    // Connect
                    session.Open(sessionOptions);

                    string ftpCustomer = ApplicationConfig.FtpCustomer;
                    string ftpLocation = ApplicationConfig.FtpLocation;
                    string ftpLocalPath = "C:\\deORO\\CSV Files";


                    // Synchronize files
                    SynchronizationResult synchronizationResult;
                    synchronizationResult =
                        session.SynchronizeDirectories(
                        SynchronizationMode.Remote, ftpLocalPath, @"video\csvfiles", true);

                    // Throw on any error
                    synchronizationResult.Check();
                }
               //return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n\r\n" + @"Error:" + e + "\r\n\r\n";
                //return 1;
            }

        }



        private static void UsgFileTransferred(object sender, TransferEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Upload of {0} succeeded", e.FileName);

                ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Upload of " + e.FileName + " succeeded";
            }
            else
            {
                Console.WriteLine("Upload of {0} failed: {1}", e.FileName, e.Error);

                ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Upload of " + e.FileName + " failed: " + e.Error;
            }

            if (e.Chmod != null)
            {
                if (e.Chmod.Error == null)
                {
                    Console.WriteLine("Permisions of {0} set to {1}", e.Chmod.FileName, e.Chmod.FilePermissions);

                    ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Permisions of " + e.Chmod.FileName + " set to " + e.Chmod.FilePermissions;
                   
                }
                else
                {
                    Console.WriteLine("Setting permissions of {0} failed: {1}", e.Chmod.FileName, e.Chmod.Error);

                    ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Setting permissions of " + e.Chmod.FileName + " failed: " + e.Chmod.Error;
                }
            }
            else
            {
                Console.WriteLine("Permissions of {0} kept with their defaults", e.Destination);

                ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Permissions of " + e.Destination + " kept with their defaults";
            }

            if (e.Touch != null)
            {
                if (e.Touch.Error == null)
                {
                    Console.WriteLine("Timestamp of {0} set to {1}", e.Touch.FileName, e.Touch.LastWriteTime);

                    ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Timestamp of " + e.Touch.FileName + " set to " + e.Touch.LastWriteTime;
                }
                else
                {
                    Console.WriteLine("Setting timestamp of {0} failed: {1}", e.Touch.FileName, e.Touch.Error);

                    ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Setting timestamp of " + e.Touch.FileName + " failed: " + e.Touch.Error;
                }
            }
            else
            {
                // This should never happen during "local to remote" synchronization
                Console.WriteLine("Timestamp of {0} kept with its default (current time)", e.Destination);

                ftpusgSyncLogPublic = ftpusgSyncLogPublic + "\r\n" + @"Timestamp of  " + e.Destination + " kept with its default (current time)";
            }
        }
    }
  
}



