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


namespace deOROSyncData
{
    public class SyncData : ISyncData
    {
        deOROService.SyncDataServiceClient client;


        public void Init()
        {

            client = new deOROService.SyncDataServiceClient("WSHttpBinding_ISyncDataService", ApplicationConfig.DeOROServiceUrl);
            client.ClientCredentials.UserName.UserName = ApplicationConfig.DeOROServiceAccessUserName;
            client.ClientCredentials.UserName.Password = ApplicationConfig.DeOROServiceAccessPassword;
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            System.Net.ServicePointManager.Expect100Continue = false;

        }

        public async Task<int> SyncScheduledItems()
        {
            if (client.State == System.ServiceModel.CommunicationState.Faulted)
                Init();

            return await Task.Run(() =>
            {
                DownloadData();

                DataSet ds = client.GetScheduleAndItemsQuantityInfo(ApplicationConfig.CustomerId, ApplicationConfig.LocationId);

                if (ds.Tables.Count > 0)
                {
                    ItemSnapshotRepository repo1 = new ItemSnapshotRepository();
                    repo1.Delete(DateTime.Now);
                    repo1.SnapshotItems(ds.Tables[0]);

                    ItemRepository repo2 = new ItemRepository();
                    return repo2.UpdateItems(ds.Tables[0]);
                }
                else
                {
                    return -1;
                }
            });
        }

        public async Task<int> UpdateScheduledStatusAndItemsQuantity()
        {
            return await Task.Run(() =>
            {

                DataSet ds = new DataSet();
                ItemRepository item = new ItemRepository();
                ds.Tables.Add(item.GetList().ToDataTable("LocationItem"));
                client.UpdateScheduledStatus(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds);
                client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);

                return 0;
            });

        }

        public void DownloadUser(string pkid)
        {
            try
            {
                SyncDataRepository syncDataRepo = new SyncDataRepository();
                SynclogRepository syncRepo = new SynclogRepository();

                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    Init();

                UserRepository userRepo = new UserRepository();

                DataSet ds = client.DownloadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId,
                    ApplicationConfig.UserSharedAcrosssLocations);
                //DataSet ds = client.DownloadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId,
                //    ApplicationConfig.UserSharedAcrosssLocations,
                //    ApplicationConfig.CanDownloadUsersFromServer, "User", "pkid", pkid);

                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.TableName.Equals("User"))
                    {
                        UserRepository repo = new UserRepository();
                        repo.Save(dt);
                    }
                }

                syncRepo.Save();


            }
            catch (Exception ex)
            {
                SynclogRepository syncRepo = new SynclogRepository();
                synclog log = new synclog();
                log.description = "UserUpdate";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
            }

        }

        public void UploadShoppingCartDetails(string pkid)
        {
            try
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    Init();

                UserRepository userRepo = new UserRepository();
                ShoppingCartDetailRepository shoppingDetailCart = new ShoppingCartDetailRepository();
                ShoppingCartRepository shoppingCart = new ShoppingCartRepository();
                ItemRepository item = new ItemRepository();
                PaymentRepository payment = new PaymentRepository();

                DataSet ds = new DataSet();

                ds.Tables.Add(shoppingCart.GetList(pkid).ToDataTable("ShoppingCart"));
                ds.Tables.Add(shoppingDetailCart.GetList(pkid).ToDataTable("ShoppingCartDetail"));
                ds.Tables.Add(payment.GetList(pkid).ToDataTable("Payment"));

                try
                {
                    if (ds.Tables["ShoppingCart"].Rows[0][2] != null)
                    {
                        ds.Tables.Add(userRepo.GetList(ds.Tables["ShoppingCart"].Rows[0][2].ToString()).ToDataTable("User"));
                    }
                }
                catch { }

                List<int> ids = new List<int>();
                foreach (DataRow row in ds.Tables["ShoppingCartDetail"].Rows)
                {
                    ids.Add(Convert.ToInt32(row["itemid"]));
                }

                ds.Tables.Add(item.GetItems(ids).ToDataTable("LocationItem"));

                client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);
            }
            catch (Exception ex)
            {
                SynclogRepository syncRepo = new SynclogRepository();
                synclog log = new synclog();
                log.description = "UploadShoppingCartDetails";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
            }

        }

        public bool UploadData()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();

            DateTime? lastSync;
            sync_data sync = syncDataRepo.GetLastUpload();

            if (sync == null)
            {
                lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
            }
            else
            {
                if (sync.status == "Success")
                {
                    lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                }
                else
                {
                    if (sync.date_time != null)
                        lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                    else
                        lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
                }
            }

            try
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    Init();

                UserRepository userRepo = new UserRepository();
                DeleteMeRepository deleteMeRepo = new DeleteMeRepository();
                AccountBalanceHistoryRepository accountBalanceRepo = new AccountBalanceHistoryRepository();
                ShoppingCartDetailRepository shoppingDetailCart = new ShoppingCartDetailRepository();
                ShoppingCartRepository shoppingCart = new ShoppingCartRepository();
                ItemRepository item = new ItemRepository();
                PaymentRepository payment = new PaymentRepository();
                CashStatusRepository cashStatus = new CashStatusRepository();
                TransactionErrorRepository transError = new TransactionErrorRepository();
                CashCollectionRepository cashCollection = new CashCollectionRepository();
                DeviceErrorRepository deviceError = new DeviceErrorRepository();
                CashDispenseRepository dispense = new CashDispenseRepository();
                CashCounterRepository counter = new CashCounterRepository();
                LocationServiceRepository service = new LocationServiceRepository();
                CreditActivityRepository creditActivity = new CreditActivityRepository();
                EventLogRepository eventLog = new EventLogRepository();

                DataSet ds = new DataSet();

                ds.Tables.Add(userRepo.GetList().ToDataTable("User"));
                ds.Tables.Add(item.GetList().ToDataTable("LocationItem"));
                ds.Tables.Add(creditActivity.GetList().ToDataTable("CreditActivity"));
                ds.Tables.Add(eventLog.GetList(lastSync).ToDataTable("EventLog"));
                ds.Tables.Add(deleteMeRepo.GetList(lastSync).ToDataTable("DeleteMe"));
                ds.Tables.Add(accountBalanceRepo.GetList(lastSync).ToDataTable("AccountBalance"));
                ds.Tables.Add(shoppingCart.GetList(lastSync).ToDataTable("ShoppingCart"));
                ds.Tables.Add(shoppingDetailCart.GetList(lastSync).ToDataTable("ShoppingCartDetail"));
                ds.Tables.Add(payment.GetList(lastSync).ToDataTable("Payment"));
                ds.Tables.Add(cashStatus.GetList(lastSync).ToDataTable("CashStatus"));
                ds.Tables.Add(transError.GetList(lastSync).ToDataTable("TransactionError"));
                ds.Tables.Add(cashCollection.GetList(lastSync).ToDataTable("CashCollection"));
                ds.Tables.Add(deviceError.GetList(lastSync).ToDataTable("DeviceError"));
                ds.Tables.Add(dispense.GetList(lastSync).ToDataTable("CashDispense"));


                string[] pkids = ds.Tables["CashCollection"].AsEnumerable().Select(row => row.Field<string>("pkid")).ToArray();
                ds.Tables.Add(counter.GetList(pkids).ToDataTable("CashCounter"));

                ds.Tables.Add(service.GetList(lastSync).ToDataTable("LocationService"));


                client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);


                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        synclog log = new synclog();

                        log.description = string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count);
                        log.createddatetime = DateTime.Now;
                        log.type = "Success";
                        log.message = "Success";
                        syncRepo.AddSynclog(log);
                    }
                }

                syncRepo.Save();
                syncDataRepo.UpdateUpload(DateTime.Now, "Success");
                return true;

            }
            catch (Exception ex)
            {
                syncDataRepo.UpdateUpload(DateTime.Now, "Failed");
                synclog log = new synclog();
                log.description = "Upload Failed";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                return false;
            }
        }

        public bool CashCollection()
        {
            return false;
        }


        public bool UploadCashCollection()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();

            DateTime? lastSync;
            sync_data sync = syncDataRepo.GetLastUpload();

            if (sync == null)
            {
                lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
            }
            else
            {
                if (sync.status == "Success")
                {
                    lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                }
                else
                {
                    if (sync.date_time != null)
                        lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                    else
                        lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
                }
            }

            try
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    Init();

                PaymentRepository payment = new PaymentRepository();
                CashStatusRepository cashStatus = new CashStatusRepository();
                CashCollectionRepository cashCollection = new CashCollectionRepository();
                CashDispenseRepository dispense = new CashDispenseRepository();
                CashCounterRepository counter = new CashCounterRepository();
                LocationServiceRepository service = new LocationServiceRepository();

                DataSet ds = new DataSet();

                ds.Tables.Add(payment.GetList(lastSync).ToDataTable("Payment"));
                ds.Tables.Add(cashStatus.GetList(lastSync).ToDataTable("CashStatus"));
                ds.Tables.Add(cashCollection.GetList(lastSync).ToDataTable("CashCollection"));

                string[] pkids = ds.Tables["CashCollection"].AsEnumerable().Select(row => row.Field<string>("pkid")).ToArray();
                ds.Tables.Add(counter.GetList(pkids).ToDataTable("CashCounter"));

                ds.Tables.Add(service.GetList(lastSync).ToDataTable("LocationService"));


                client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);


                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        synclog log = new synclog();

                        log.description = string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count);
                        log.createddatetime = DateTime.Now;
                        log.type = "Success";
                        log.message = "Success";
                        syncRepo.AddSynclog(log);
                    }
                }

                syncRepo.Save();
                syncDataRepo.UpdateUpload(DateTime.Now, "Success");
                return true;

            }
            catch (Exception ex)
            {
                syncDataRepo.UpdateUpload(DateTime.Now, "Failed");
                synclog log = new synclog();
                log.description = "Upload Failed";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                return false;
            }
        }

        public bool UploadShoppingCart()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();

            DateTime? lastSync;
            sync_data sync = syncDataRepo.GetLastUpload();

            if (sync == null)
            {
                lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
            }
            else
            {
                if (sync.status == "Success")
                {
                    lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                }
                else
                {
                    if (sync.date_time != null)
                        lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                    else
                        lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
                }
            }

            try
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    Init();

                ShoppingCartDetailRepository shoppingDetailCart = new ShoppingCartDetailRepository();
                ShoppingCartRepository shoppingCart = new ShoppingCartRepository();
                LocationServiceRepository service = new LocationServiceRepository();
                DataSet ds = new DataSet();

                ds.Tables.Add(shoppingCart.GetList(lastSync).ToDataTable("ShoppingCart"));
                ds.Tables.Add(shoppingDetailCart.GetList(lastSync).ToDataTable("ShoppingCartDetail"));
               
                ds.Tables.Add(service.GetList(lastSync).ToDataTable("LocationService"));


                client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);


                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        synclog log = new synclog();

                        log.description = string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count);
                        log.createddatetime = DateTime.Now;
                        log.type = "Success";
                        log.message = "Success";
                        syncRepo.AddSynclog(log);
                    }
                }

                syncRepo.Save();
                syncDataRepo.UpdateUpload(DateTime.Now, "Success");
                return true;

            }
            catch (Exception ex)
            {
                syncDataRepo.UpdateUpload(DateTime.Now, "Failed");
                synclog log = new synclog();
                log.description = "Upload Failed";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                return false;
            }
        }


        public bool UploadAccountBalances()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();

            DateTime? lastSync;
            sync_data sync = syncDataRepo.GetLastUpload();

            if (sync == null)
            {
                lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
            }
            else
            {
                if (sync.status == "Success")
                {
                    lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                }
                else
                {
                    if (sync.date_time != null)
                        lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
                    else
                        lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
                }
            }

            try
            {
                if (client.State == System.ServiceModel.CommunicationState.Faulted)
                    Init();

                UserRepository userRepo = new UserRepository();
                AccountBalanceHistoryRepository accountBalanceRepo = new AccountBalanceHistoryRepository();
                LocationServiceRepository service = new LocationServiceRepository();
                DataSet ds = new DataSet();

                ds.Tables.Add(userRepo.GetList().ToDataTable("User"));
                ds.Tables.Add(accountBalanceRepo.GetList(lastSync).ToDataTable("AccountBalance"));
               

                ds.Tables.Add(service.GetList(lastSync).ToDataTable("LocationService"));


                client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);


                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        synclog log = new synclog();

                        log.description = string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count);
                        log.createddatetime = DateTime.Now;
                        log.type = "Success";
                        log.message = "Success";
                        syncRepo.AddSynclog(log);
                    }
                }

                syncRepo.Save();
                syncDataRepo.UpdateUpload(DateTime.Now, "Success");
                return true;

            }
            catch (Exception ex)
            {
                syncDataRepo.UpdateUpload(DateTime.Now, "Failed");
                synclog log = new synclog();
                log.description = "Upload Failed";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                return false;
            }
        }


        public void ProcessCredits()
        {

        }


        public bool DownloadData()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();
            bool success = true;

            try
            {
                DataSet ds = client.DownloadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId,
                                                ApplicationConfig.UserSharedAcrosssLocations);
                //DataSet ds = client.DownloadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId,
                //    ApplicationConfig.UserSharedAcrosssLocations, ApplicationConfig.CanDownloadUsersFromServer,
                //    "NoSpecified", "NoSpecified", "NoSpecified");
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        synclog log = new synclog();

                        log.description = string.Format("Download Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count);
                        log.createddatetime = DateTime.Now;
                        log.type = "Success";
                        log.message = "Success";

                        if (dt.TableName.Equals("Item"))
                        {
                            try
                            {
                                ItemRepository repo = new ItemRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("Discount"))
                        {
                            try
                            {
                                DiscountRepository repo = new DiscountRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("Category"))
                        {
                            try
                            {
                                CategoryRepository repo = new CategoryRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("PlanogramItem"))
                        {
                            try
                            {
                                PlanogramItemRepository repo = new PlanogramItemRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("ComboDiscount"))
                        {
                            try
                            {
                                ComboDiscountRepository repo = new ComboDiscountRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("ComboDiscountDetail"))
                        {
                            try
                            {
                                ComboDiscountDetailRepository repo = new ComboDiscountDetailRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("Subsidy"))
                        {
                            try
                            {
                                SubsidyRepository repo = new SubsidyRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("SubsidyDetail"))
                        {
                            try
                            {
                                SubsidyDetailRepository repo = new SubsidyDetailRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("Credit"))
                        {
                            try
                            {
                                CreditRepository repo = new CreditRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("CreditUser"))
                        {
                            try
                            {
                                CreditUserRepository repo = new CreditUserRepository();
                                repo.Save(dt);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                log.type = "Failed";
                                log.message = ex.ToString();
                            }
                        }
                        else if (dt.TableName.Equals("User"))
                        {
                            UserRepository repo = new UserRepository();
                            repo.Save(dt);
                        }

                        syncRepo.AddSynclog(log);
                    }
                }

                syncRepo.Save();

                if (success)
                {
                    syncDataRepo.UpdateDownload(DateTime.Now, "Success");
                }
                else
                {
                    syncDataRepo.UpdateDownload(DateTime.Now, "Failed");
                }

                return success;
            }
            catch (Exception ex)
            {
                success = false;
                syncDataRepo.UpdateDownload(DateTime.Now, "Failed");
                synclog log = new synclog();

                log.description = "Download Failed";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                return success;
            }
        }

        public bool DownloadCredit()
        {
            return false;
        }

        public bool AdjustUnevenBalances()
        {
            int ret = 0;
            SynclogRepository syncRepo = new SynclogRepository();
            synclog log = new synclog();
            log.description = "AdjustUnevenBalances";
            try
            {
                ret = client.AdjustUnevenBalances(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ApplicationConfig.UserSharedAcrosssLocations);
                log.type = "Success";
                log.message = "Adjusted "+ret.ToString()+" user balances";
            }
            catch (Exception ex)
            {
                log.type = "Failed";
                log.message = ex.ToString();
            }

            log.createddatetime = DateTime.Now;
            
            syncRepo.AddSynclog(log);
            syncRepo.Save();

            return ret >0;
        }

        public void SyncTime() {
            deORODataAccessApp.SyncTimeRet ret = deORODataAccessApp.SyncTime.SetWindowsSystemTime();

            // logging
            SynclogRepository syncRepo = new SynclogRepository();
            synclog log = new synclog();
            log.description = "SyncTime";
            log.createddatetime = DateTime.Now;
            log.message = ret.description;

            if (ret.status) {
                log.type = "Success";
            }
            else {
                log.type = "Failed";
            }
            syncRepo.AddSynclog(log);
            syncRepo.Save();
        }

        public void UploadCashCollectionEvents(string userPkid = "")
        {
        //    bool success = true;
        //    SyncDataRepository syncDataRepo = new SyncDataRepository();
        //    SynclogRepository syncRepo = new SynclogRepository();

        //    DateTime? lastSync;
        //    sync_data sync = syncDataRepo.GetLastUpload();

        //    if (sync == null)
        //    {
        //        lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
        //    }
        //    else
        //    {
        //        if (sync.status == "Success")
        //        {
        //            lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
        //        }
        //        else
        //        {
        //            if (sync.date_time != null)
        //                lastSync = sync.date_time.Value.AddMinutes(-15); //Go Back 15 minutes
        //            else
        //                lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
        //        }
        //    }

        //    try
        //    {
        //        if (client.State == System.ServiceModel.CommunicationState.Faulted)
        //            Init();

        //        PaymentRepository payment = new PaymentRepository();
        //        CashStatusRepository cashStatus = new CashStatusRepository();
        //        CashCollectionRepository cashCollection = new CashCollectionRepository();
        //        CashDispenseRepository dispense = new CashDispenseRepository();
        //        CashCounterRepository counter = new CashCounterRepository();
        //        LocationServiceRepository service = new LocationServiceRepository();

        //        DataSet ds = new DataSet();

        //        ds.Tables.Add(payment.GetList(lastSync).ToDataTable("Payment"));
        //        ds.Tables.Add(cashStatus.GetList(lastSync).ToDataTable("CashStatus"));
        //        ds.Tables.Add(cashCollection.GetList(lastSync).ToDataTable("CashCollection"));

        //        string[] pkids = ds.Tables["CashCollection"].AsEnumerable().Select(row => row.Field<string>("pkid")).ToArray();
        //        ds.Tables.Add(counter.GetList(pkids).ToDataTable("CashCounter"));

        //        ds.Tables.Add(service.GetList(lastSync).ToDataTable("LocationService"));


        //        client.UploadData(ApplicationConfig.CustomerId, ApplicationConfig.LocationId, ds, ApplicationConfig.UserSharedAcrosssLocations);


        //        foreach (DataTable dt in ds.Tables)
        //        {
        //            if (dt.Rows.Count > 0)
        //            {
        //                synclog log = new synclog();

        //                log.description = string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count);
        //                log.createddatetime = DateTime.Now;
        //                log.type = "Success";
        //                log.message = "Success";
        //                syncRepo.AddSynclog(log);
        //            }
        //        }

        //        syncRepo.Save();
        //        syncDataRepo.UpdateUpload(DateTime.Now, "Success");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        syncDataRepo.UpdateDownload(DateTime.Now, "Failed");
        //        synclog log = new synclog();

        //        log.description = "Upload CashCollection Failed";
        //        log.createddatetime = DateTime.Now;
        //        log.type = "Failed";
        //        log.message = ex.ToString();

        //        syncRepo.AddSynclog(log);
        //        syncRepo.Save();
        //        return success;
        //    }

        }


        bool ISyncData.ProcessCredits()
        {
            throw new NotImplementedException();
        }
    }

    public static class Extensions
    {
        private readonly static object _lock = new object();

        public static DataTable ToDataTable<T>(this IList<T> data, string tableName = "")
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(tableName);
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static T ConvertToEntity<T>(this DataRow tableRow) where T : new()
        {
            // Create a new type of the entity I want
            Type t = typeof(T);
            T returnObject = new T();


            foreach (DataColumn col in tableRow.Table.Columns)
            {
                string colName = col.ColumnName;

                // Look for the object's property with the columns name, ignore case
                PropertyInfo pInfo = t.GetProperty(colName.ToLower(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // did we find the property ?
                if (pInfo != null)
                {
                    object val = tableRow[colName];

                    // is this a Nullable<> type
                    bool IsNullable = (Nullable.GetUnderlyingType(pInfo.PropertyType) != null);
                    if (IsNullable)
                    {
                        if (val is System.DBNull)
                        {
                            val = null;
                        }
                        else
                        {
                            // Convert the db type into the T we have in our Nullable<T> type
                            val = Convert.ChangeType(val, Nullable.GetUnderlyingType(pInfo.PropertyType));
                        }
                    }
                    else
                    {
                        // Convert the db type into the type of the property in our entity
                        val = Convert.ChangeType(val, pInfo.PropertyType);
                    }
                    // Set the value of the property with the value from the db
                    pInfo.SetValue(returnObject, val, null);
                }
            }

            // return the entity object with values
            return returnObject;
        }

        public static void CopyPropertyValues(object source, object destination, string[] ignoreKeys)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                if (!ignoreKeys.Contains(sourceProperty.Name))
                    foreach (var destProperty in destProperties)
                    {
                        if (destProperty.Name == sourceProperty.Name && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                        {
                            destProperty.SetValue(destination, sourceProperty.GetValue(
                                source, new object[] { }), new object[] { });

                            break;
                        }
                    }
            }
        }

    }
}
