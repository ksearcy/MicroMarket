using deOROFtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deOROSyncData
{
    class Program
    {
        static void Main(string[] args)
        {
           
            ISyncData sync = SyncDataFactory.GetSyncData();
            Credit credit = new Credit();
            FtpSend FtpSync = new FtpSend();
            FtpUsgSend FtpUsgSend = new FtpUsgSend();

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "/help":
                        Console.WriteLine("/syncall /syncup /syncdown /credit /ftp /ftpusg /synctime /deORO");
                        break;

                    case "/syncup":
                        sync.Init();
                        sync.UploadData();
                        break;

                    case "/syncdown":
                        sync.Init();
                        sync.DownloadData();
                        sync.DownloadCredit();
                        sync.SyncTime();
                        break;

                    case "/synctime":
                        sync.Init();
                        sync.SyncTime();
                        break;

                    case "/credit":
                        sync.Init();
                        credit.ProcessCredits();
                        sync.DownloadCredit();
                        sync.SyncTime();
                        break;

                    case "/syncall":
                        sync.Init();
                        sync.DownloadData();
                        sync.UploadData();
                        credit.ProcessCredits();
                        if (sync.AdjustUnevenBalances())
                            sync.DownloadData(); 
                        sync.SyncTime();
                        break;

                    case "/accountbalances":
                        sync.Init();
                        sync.UploadAccountBalances();
                        break;

                    case "/shoppingcart":
                        sync.Init();
                           sync.UploadShoppingCart();
                        break;

                    //case "/cashcollection":
                    //    sync.Init();
                    //    sync.CashCollection();
                    //    break;

                    case "/ftp":
                        FtpSync.SyncFTPMainMethod();
                        break;

                    case "/ftpusg":
                        FtpUsgSend.SyncFtpUsgMainMethod();
                        break;

                    case "/user":
                        break;
                }

            }

        }
    }
}
