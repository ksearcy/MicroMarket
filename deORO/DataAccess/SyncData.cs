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

namespace deORO.DataAccess
{
    public class SyncData
    {
        deOROService.SyncDataServiceClient client;


        public void Init()
        {

            client = new deOROService.SyncDataServiceClient("WSHttpBinding_ISyncDataService", Helpers.Global.DeOROServiceUrl);
            client.ClientCredentials.UserName.UserName = Helpers.Global.DeOROServiceAccessUserName;
            client.ClientCredentials.UserName.Password = Helpers.Global.DeOROServiceAccessPassword;
            client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            System.Net.ServicePointManager.Expect100Continue = false;

        }

        public void Test()
        {
            client.TestService(Helpers.Global.CustomerId, Helpers.Global.LocationId);
        }

        public async Task<int> SyncScheduledItems()
        {
            if (client.State == System.ServiceModel.CommunicationState.Faulted)
                Init();

            return await Task.Run(() =>
            {
                DownloadData();

                DataSet ds = client.GetScheduleAndItemsQuantityInfo(Helpers.Global.CustomerId, Helpers.Global.LocationId);

                if (ds.Tables.Count > 0)
                {
                    ItemRepository repo = new ItemRepository();
                    return repo.UpdateItems(ds.Tables[0]);
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
                client.UpdateScheduledStatus(Helpers.Global.CustomerId, Helpers.Global.LocationId);

                DataSet ds = new DataSet();
                ItemRepository item = new ItemRepository();
                ds.Tables.Add(item.GetList().ToDataTable("LocationItem"));
                client.UploadData(Helpers.Global.CustomerId, Helpers.Global.LocationId, ds);

                return 0;
            });
        }

        public bool UploadData()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();

            DateTime? lastSync = syncDataRepo.GetLastSuccessfulUpload();

            if (lastSync == null)
            {
                lastSync = new DateTime(2015, 1, 1, 0, 0, 0);
            }
            else
            {
                lastSync = lastSync.Value.AddSeconds(-Helpers.Global.SyncInterval);
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

                DataSet ds = new DataSet();
                ds.Tables.Add(userRepo.GetList(lastSync).ToDataTable("User"));
                ds.Tables.Add(deleteMeRepo.GetList(lastSync).ToDataTable("DeleteMe"));
                ds.Tables.Add(accountBalanceRepo.GetList(lastSync).ToDataTable("AccountBalance"));
                ds.Tables.Add(shoppingCart.GetList(lastSync).ToDataTable("ShoppingCart"));
                ds.Tables.Add(shoppingDetailCart.GetList(lastSync).ToDataTable("ShoppingCartDetail"));
                ds.Tables.Add(item.GetList(lastSync).ToDataTable("LocationItem"));
                ds.Tables.Add(payment.GetList(lastSync).ToDataTable("Payment"));
                ds.Tables.Add(cashStatus.GetList(lastSync).ToDataTable("CashStatus"));
                ds.Tables.Add(transError.GetList(lastSync).ToDataTable("TransactionError"));
                ds.Tables.Add(cashCollection.GetList(lastSync).ToDataTable("CashCollection"));
                ds.Tables.Add(deviceError.GetList(lastSync).ToDataTable("DeviceError"));
                ds.Tables.Add(dispense.GetList(lastSync).ToDataTable("CashDispense"));
                ds.Tables.Add(counter.GetList(lastSync).ToDataTable("CashCounter"));
                ds.Tables.Add(service.GetList(lastSync).ToDataTable("LocationService"));

                client.UploadData(Helpers.Global.CustomerId, Helpers.Global.LocationId, ds);
                
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

        public void ProcessUploadResults()
        {

        }

        public bool DownloadData()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();
            bool success = true;

            try
            {
                DataSet ds = client.DownloadData(Helpers.Global.CustomerId, Helpers.Global.LocationId);

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
