using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using Newtonsoft.Json;
using RestSharp;
using System.IO;

namespace deOROSyncData
{
    public class SyncVendor4Me : ISyncData
    {
        deOROService.SyncDataServiceClient client;
        private string ServiceURL;
        private string UserName;
        private string Password;
        private RestClient restClient = null;

        public void Init()
        {
            ServiceURL = ApplicationConfig.DeOROServiceUrl;
            UserName = ApplicationConfig.DeOROServiceAccessUserName;
            Password = ApplicationConfig.DeOROServiceAccessPassword;

            restClient = new RestClient(ServiceURL);
            restClient.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(UserName, Password);
        }


        public void DownloadUser(string pkid)
        {

        }

        public bool DownloadCredit()
        {
            return false;
        }

        public bool UploadAccountBalances()
        {
            return false;
        }

        public bool UploadShoppingCart()
        {
            return false;
        }

        public bool CashCollection()
        {
            return false;
        }

        public void UploadShoppingCartDetails(string pkid)
        {
            SynclogRepository syncRepo = new SynclogRepository();
            try
            {
                ShoppingCartDetailRepository shoppingDetailCart = new ShoppingCartDetailRepository();
                var records = shoppingDetailCart.GetList(pkid);

                foreach (var shoppingcart in records.GroupBy(x => x.shoppingcartpkid))
                {
                    ShoppingCartDetail detail = new ShoppingCartDetail();
                    detail.uuid = shoppingcart.Key;
                    detail.vm_id = ApplicationConfig.LocationId;

                    List<Product> products = new List<Product>();
                    foreach (var items in shoppingcart.GroupBy(x => x.itemid))
                    {
                        Product product = new Product();
                        product.product_id = items.Key.Value.ToString();
                        product.sel_price = items.ElementAt(0).price_tax_included.Value;
                        product.sel_vended_r = items.Count();

                        products.Add(product);
                        detail.timestamp = items.ElementAt(0).created_date_time.Value.ToUniversalTime().ToString("o");
                    }

                    detail.products = products;

                    var requestCreate = new RestRequest("Master/create", Method.POST);
                    requestCreate.AddHeader("x-rest-password", Password);
                    requestCreate.AddHeader("x-rest-username", UserName);
                    requestCreate.AddHeader("cache-control", "no-cache");
                    requestCreate.RequestFormat = DataFormat.Json;
                    requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(detail), ParameterType.RequestBody);

                    var response = restClient.Execute<ResponseObject>(requestCreate);

                    if (!response.Data.success)
                    {
                        synclog log = new synclog();
                        log.description = "UploadShoppingCartDetails";
                        log.createddatetime = DateTime.Now;
                        log.type = "Failed";
                        log.message = response.Data.message;

                        syncRepo.AddSynclog(log);
                        syncRepo.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                synclog log = new synclog();
                log.description = "UploadShoppingCartDetails";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
            }
        }

        public void UploadDoorEvents(DateTime? lastSync)
        {
            SynclogRepository syncRepo = new SynclogRepository();

            try
            {
                EventLogRepository eventLogRepository = new EventLogRepository();
                var eventLogrecords = eventLogRepository.GetList(lastSync);

                foreach (var record in eventLogrecords.Where(x => x.source == "Relay1"))
                {
                    Door door = new Door();
                    door.vm_id = ApplicationConfig.LocationId;
                    door.uuid = record.pkid;
                    door.timestamp = record.created_date_time.Value.ToUniversalTime().ToString("o");
                    door.door = record.code;

                    var requestCreate = new RestRequest("esnStatus/create", Method.POST);
                    requestCreate.AddHeader("x-rest-password", Password);
                    requestCreate.AddHeader("x-rest-username", UserName);
                    requestCreate.AddHeader("cache-control", "no-cache");
                    requestCreate.RequestFormat = DataFormat.Json;
                    requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(door), ParameterType.RequestBody);

                    var response = restClient.Execute<ResponseObject>(requestCreate);

                    if (!response.Data.success)
                    {
                        synclog log = new synclog();
                        log.description = "UploadDoorEvents";
                        log.createddatetime = DateTime.Now;
                        log.type = "Failed";
                        log.message = response.Data.message;

                        syncRepo.AddSynclog(log);
                        syncRepo.Save();

                    }
                }
            }
            catch (Exception ex)
            {
                synclog log = new synclog();
                log.description = "UploadDoorEvents";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
            }
        }

        public void UploadCashCollectionEvents(string userPkid = "")
        {
            SynclogRepository syncRepo = new SynclogRepository();

            try
            {
                CashCounterRepository counterRepo = new CashCounterRepository();
                CashDispenseRepository dispenseRepo = new CashDispenseRepository();

                CashCollection cash = new CashCollection();
                cash.vm_id = ApplicationConfig.LocationId;
                cash.uuid = Guid.NewGuid().ToString();
                cash.timestamp = DateTime.Now.ToUniversalTime().ToString("o");
                cash.cash_in = counterRepo.GetList().Sum(x => x.amount).Value;
                cash.cash_out = dispenseRepo.GetList().Sum(x => x.amount).Value;
                try
                {
                    if (userPkid != "")
                    {
                        UserRepository userRepo = new UserRepository();
                        var user = userRepo.GetUser(userPkid);
                        cash.hr_id = Convert.ToInt32(user.applicationname.Split(':')[1]);
                    }
                    
                }
                catch
                {
                    cash.hr_id = 0;
                }

                var requestCreate = new RestRequest("meterValues/create", Method.POST);
                requestCreate.AddHeader("x-rest-password", Password);
                requestCreate.AddHeader("x-rest-username", UserName);
                requestCreate.AddHeader("cache-control", "no-cache");
                requestCreate.RequestFormat = DataFormat.Json;
                requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(cash), ParameterType.RequestBody);

                var response = restClient.Execute<ResponseObject>(requestCreate);

                if (!response.Data.success)
                {
                    synclog log = new synclog();
                    log.description = "UploadCashCollectionEvents";
                    log.createddatetime = DateTime.Now;
                    log.type = "Failed";
                    log.message = response.Data.message;

                    syncRepo.AddSynclog(log);
                    syncRepo.Save();

                }

            }
            catch (Exception ex)
            {
                synclog log = new synclog();
                log.description = "UploadCashCollectionEvents";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
            }
        }

        public void UploadEventLogDetails(DateTime? lastSync)
        {
            SynclogRepository syncRepo = new SynclogRepository();

            try
            {
                TransactionErrorRepository transactionLogRepo = new TransactionErrorRepository();
                var records = transactionLogRepo.GetList(lastSync);

                foreach (var record in records)
                {
                    var e = new Event();
                    e.vm_id = ApplicationConfig.LocationId;
                    e.source = record.source;
                    e.description = record.description;
                    e.timestamp = record.created_date_time.Value.ToUniversalTime().ToString("o");
                    e.uuid = record.pkid;
                    e.error_code = record.code;

                    var requestCreate = new RestRequest("VWError/create", Method.POST);
                    requestCreate.AddHeader("x-rest-password", Password);
                    requestCreate.AddHeader("x-rest-username", UserName);
                    requestCreate.AddHeader("cache-control", "no-cache");
                    requestCreate.RequestFormat = DataFormat.Json;
                    requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(e), ParameterType.RequestBody);

                    var response = restClient.Execute<ResponseObject>(requestCreate);

                    if (!response.Data.success)
                    {
                        synclog log = new synclog();
                        log.description = "UploadEventDetails";
                        log.createddatetime = DateTime.Now;
                        log.type = "Failed";
                        log.message = response.Data.message;

                        syncRepo.AddSynclog(log);
                        syncRepo.Save();

                    }
                }
            }
            catch (Exception ex)
            {
                synclog log = new synclog();
                log.description = "UploadEventDetails";
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

            try
            {

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


                ShoppingCartRepository shoppingCartRepo = new ShoppingCartRepository();
                ShoppingCartDetailRepository shoppingDetailCartRepo = new ShoppingCartDetailRepository();
                var records = shoppingDetailCartRepo.GetList(lastSync);
                //var records = shoppingDetailCartRepo.GetList().Where(x => x.shoppingcartpkid == "9d483656-a95e-4b92-8487-6bdef0a6d0f0").ToList();
                //var records = shoppingDetailCartRepo.GetList().Where(x => x.itemid == 3988).ToList();

                foreach (var shoppingcart in records.GroupBy(x => x.shoppingcartpkid))
                {
                    ShoppingCartDetail detail = new ShoppingCartDetail();
                    detail.uuid = shoppingcart.Key;
                    detail.vm_id = ApplicationConfig.LocationId;
                    try
                    {
                        detail.status = shoppingCartRepo.GetList(shoppingcart.Key).FirstOrDefault().status;
                    }
                    catch { }
                    
                    List<Product> products = new List<Product>();
                    foreach (var items in shoppingcart.GroupBy(x => x.itemid))
                    {
                        Product product = new Product();
                        product.product_id = items.Key.Value.ToString();
                        product.sel_price = items.ElementAt(0).price_tax_included.Value;
                        product.sel_vended_r = items.Count();

                        products.Add(product);
                        detail.timestamp = items.ElementAt(0).created_date_time.Value.ToUniversalTime().ToString("o");
                    }

                    detail.products = products;

                    var requestCreate = new RestRequest("Master/create", Method.POST);
                    requestCreate.AddHeader("x-rest-password", Password);
                    requestCreate.AddHeader("x-rest-username", UserName);
                    requestCreate.AddHeader("cache-control", "no-cache");
                    requestCreate.RequestFormat = DataFormat.Json;
                    requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(detail), ParameterType.RequestBody);
                    

                    var response = restClient.Execute<ResponseObject>(requestCreate);

                    if (!response.Data.success)
                    {
                        synclog log = new synclog();
                        log.description = "Upload Table Name - ShoppingCartDetail";
                        log.createddatetime = DateTime.Now;
                        log.type = "Failed";
                        log.message = response.Data.message;

                        syncRepo.AddSynclog(log);
                        syncRepo.Save();
                    }
                    else
                    {
                        synclog log = new synclog();
                        log.description = string.Format("Upload Table Name - {0}, Rows = {1}", "ShoppingCartDetail", records.Count);
                        log.createddatetime = DateTime.Now;
                        log.type = "Success";
                        log.message = "Success";

                        syncRepo.AddSynclog(log);
                        syncRepo.Save();
                    }

                    System.Diagnostics.Debug.WriteLine(detail.uuid + " " + response.Data.success);
                }


                UploadEventLogDetails(lastSync);
                UploadDoorEvents(lastSync);
                UploadCashCollectionEvents();

                syncDataRepo.UpdateUpload(DateTime.Now, "Success");
                return true;
            }
            catch (Exception ex)
            {
                synclog log = new synclog();
                log.description = "UploadData";
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = ex.Message.ToString();

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                syncDataRepo.UpdateUpload(DateTime.Now, "Failed");

                return false;
            }
        }

        public bool ProcessCredits()
        {
            return true;
        }

        public bool DownloadData()
        {
            SyncDataRepository syncDataRepo = new SyncDataRepository();
            bool success = DownloadItems();
            success = DownloadUsers();

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

        private bool DownloadItems()
        {
            SynclogRepository syncRepo = new SynclogRepository();
            synclog log = null;

            try
            {
                var requestVM = new RestRequest("Vm/" + ApplicationConfig.LocationId, Method.GET);
                requestVM.AddHeader("x-rest-password", Password);
                requestVM.AddHeader("x-rest-username", UserName);
                requestVM.AddHeader("cache-control", "no-cache");

                IRestResponse responseVM = restClient.Execute(requestVM);
                var deserializedVM = JsonConvert.DeserializeObject<VMRoot>(responseVM.Content);

                if (!deserializedVM.success)
                {
                    log.description = string.Format("Items - " + deserializedVM.message);
                    log.createddatetime = DateTime.Now;
                    log.type = "Failed";
                    log.message = "Failed";

                    syncRepo.AddSynclog(log);
                    syncRepo.Save();
                    return false;
                }

                var opCenter = deserializedVM.data.opcenter_id;

                var requestMasterList = new RestRequest("Product?opcenter_id=" + opCenter, Method.GET);
                requestMasterList.AddHeader("x-rest-password", Password);
                requestMasterList.AddHeader("x-rest-username", UserName);
                requestMasterList.AddHeader("cache-control", "no-cache");

                IRestResponse responseMasterList = restClient.Execute(requestMasterList);
                var deserializedProductMasterList = JsonConvert.DeserializeObject<ProductMasterListRoot>(responseMasterList.Content);


                if (!deserializedProductMasterList.success)
                {
                    log = new synclog();
                    log.description = "Items - " + deserializedProductMasterList.message;
                    log.createddatetime = DateTime.Now;
                    log.type = "Failed";
                    log.message = "Failed";

                    syncRepo.AddSynclog(log);
                    syncRepo.Save();
                    return false;
                }

                var requestProductList = new RestRequest("vmProducts/{ID}", Method.GET);
                requestProductList.AddHeader("x-rest-password", Password);
                requestProductList.AddHeader("x-rest-username", UserName);
                requestProductList.AddUrlSegment("ID", ApplicationConfig.LocationId.ToString());

                IRestResponse responseProductList = restClient.Execute(requestProductList);
                var deserializedProductList = JsonConvert.DeserializeObject<ProductListRoot>(responseProductList.Content);

                if (deserializedProductList.success)
                {
                    //Process Categories
                    CategoryRepository catRepo = new CategoryRepository();
                    int maxCategoryId = 0;

                    try
                    {
                        maxCategoryId = catRepo.GetCategories().Max(x => x.id);
                    }
                    catch { }

                    foreach (var c in deserializedProductMasterList.data.Select(x => new { category = x.category_name }).Distinct())
                    {
                        if (catRepo.GetCategory(c.category) == null)
                        {
                            maxCategoryId = maxCategoryId + 1;
                            var category = new category();
                            category.code = c.category;
                            category.description = c.category;
                            category.name = c.category;
                            category.id = maxCategoryId;
                            catRepo.AddCategory(category);
                            catRepo.Save();
                        }

                        if (catRepo.GetSubCategory(c.category) == null)
                        {
                            try
                            {
                                maxCategoryId = maxCategoryId + 1;
                                var sub = new category();
                                sub.code = c.category;
                                sub.description = c.category;
                                sub.name = c.category;
                                sub.parentid = catRepo.GetCategory(c.category).id;
                                sub.id = maxCategoryId;
                                catRepo.AddCategory(sub);
                                catRepo.Save();
                            }
                            catch { }
                        }
                    }

                    //Process Items
                    ItemRepository itemRepo = new ItemRepository();

                    log = new synclog();
                    log.description = string.Format("Download Table Name - {0}, Rows = {1}", "Item", deserializedProductList.data.Count);
                    log.createddatetime = DateTime.Now;
                    log.type = "Success";
                    log.message = "Success";

                    foreach (var product in deserializedProductList.data.Distinct().OrderBy(x => x.product_id))
                    {
                        item item = itemRepo.GetItem(product.product_id);
                        if (item == null)
                        {
                            item = new item();
                            item.id = product.product_id;
                            item.price = Convert.ToDecimal(product.price);
                            item.price_tax_included = item.price;
                            item.quantity = product.capacity;

                            var masterProduct = deserializedProductMasterList.data.SingleOrDefault(x => x.product_id == product.product_id);
                            if (masterProduct != null)
                            {
                                item.upc = masterProduct.product_sku;
                                item.name = masterProduct.product_name;
                                item.barcode = masterProduct.product_sku;
                                item.description = masterProduct.brand_name + " " + masterProduct.package_name;
                                try
                                {
                                    item.categoryid = catRepo.GetSubCategory(masterProduct.category_name).id;
                                }
                                catch { }
                            }

                            itemRepo.AddItem(item);
                        }
                        else
                        {
                            item.price = Convert.ToDecimal(product.price);
                            item.price_tax_included = item.price;
                            item.quantity = product.capacity;

                            var masterProduct = deserializedProductMasterList.data.SingleOrDefault(x => x.product_id == product.product_id);
                            if (masterProduct != null)
                            {
                                item.upc = masterProduct.product_sku;
                                item.name = masterProduct.product_name;
                                item.barcode = masterProduct.product_sku;
                                item.description = masterProduct.brand_name + " " + masterProduct.package_name;
                                try
                                {
                                    item.categoryid = catRepo.GetSubCategory(masterProduct.category_name).id;
                                }
                                catch { }
                            }

                            itemRepo.UpdateItem(item);
                        }
                    }

                    itemRepo.Save();
                    syncRepo.AddSynclog(log);
                    syncRepo.Save();


                    //Get Users from localdb who are not part this data pull
                    var items = itemRepo.GetList().Select((x) =>
                    {
                        foreach (var record in deserializedProductList.data)
                        {
                            if (x.id == record.product_id)
                            {
                                return null;
                            }
                        }
                        return x;
                    }).Where(x => x != null);
                    
                    foreach (var item in items)
                    {
                        itemRepo.Delete(item.id);
                    }

                    return true;
                }
                else
                {
                    log = new synclog();
                    log.description = "Download Table Name - Items - " + deserializedProductList.message;
                    log.createddatetime = DateTime.Now;
                    log.type = "Failed";
                    log.message = "Failed";

                    syncRepo.AddSynclog(log);
                    syncRepo.Save();

                    return false;
                }
            }
            catch (Exception ex)
            {
                log = new synclog();
                log.description = "Download Table Name - Items - " + ex.Message;
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = "Failed";

                syncRepo.AddSynclog(log);
                syncRepo.Save();

                return false;
            }

        }

        private bool DownloadUsers()
        {
            SynclogRepository syncRepo = new SynclogRepository();
            synclog log = new synclog();

            try
            {
                var requestVM = new RestRequest("Vm/" + ApplicationConfig.LocationId, Method.GET);
                requestVM.AddHeader("x-rest-password", Password);
                requestVM.AddHeader("x-rest-username", UserName);
                requestVM.AddHeader("cache-control", "no-cache");

                IRestResponse responseVM = restClient.Execute(requestVM);
                var deserializedVM = JsonConvert.DeserializeObject<VMRoot>(responseVM.Content);

                if (!deserializedVM.success)
                {
                    log.description = string.Format("Users - " + deserializedVM.message);
                    log.createddatetime = DateTime.Now;
                    log.type = "Failed";
                    log.message = "Failed";

                    syncRepo.AddSynclog(log);
                    syncRepo.Save();
                    return false;
                }

                var opCenter = deserializedVM.data.opcenter_id;
                var requestHrList = new RestRequest("Hr/?opcenter_id=" + opCenter, Method.GET);
                requestHrList.AddHeader("x-rest-password", Password);
                requestHrList.AddHeader("x-rest-username", UserName);
                requestHrList.AddHeader("cache-control", "no-cache");

                IRestResponse responseUserList = restClient.Execute(requestHrList);
                var deserializedUserList = JsonConvert.DeserializeObject<HrListRoot>(responseUserList.Content);

                if (!deserializedUserList.success)
                {
                    log.description = string.Format("Download Table Name - Users - " + deserializedUserList.message);
                    log.createddatetime = DateTime.Now;
                    log.type = "Failed";
                    log.message = "Failed";

                    syncRepo.AddSynclog(log);
                    syncRepo.Save();
                    return false;
                }

                UserRepository userRepo = new UserRepository();

                foreach (var record in deserializedUserList.data)
                {
                    if (record.hr_number.HasValue)
                    {
                        if (record.hr_email == null || record.hr_email.Trim() == "")
                            continue;

                        var user = userRepo.GetUserByUsername(record.hr_email);

                        if (user == null)
                        {
                            user = new user();
                            try
                            {
                                user.first_name = record.hr_name.Split(' ')[0];
                            }
                            catch { }
                            try
                            {
                                user.last_name = record.hr_name.Split(' ')[1];
                            }
                            catch { }

                            if (!userRepo.EmailExists(record.hr_email))
                                user.email = record.hr_email;

                            user.username = record.hr_email;
                            user.password = record.hr_password;
                            try
                            {
                                user.created_date_time = DateTime.Parse(record.created);
                            }
                            catch { }
                            try
                            {
                                user.last_updated_on = DateTime.Parse(record.modified);
                            }
                            catch { }

                            user.is_active = 1;
                            user.is_approved = 1;
                            user.is_lockedout = 0;
                            user.is_staff = 1;
                            user.is_superuser = 0;
                            user.applicationname = "vendwatch:" + record.hr_id;
                            user.pkid = Guid.NewGuid().ToString();
                            user.account_balance = 0.00M;

                            userRepo.AddUser(user);

                        }
                        else
                        {
                            try
                            {
                                user.first_name = record.hr_name.Split(' ')[0];
                            }
                            catch { }
                            try
                            {
                                user.last_name = record.hr_name.Split(' ')[1];
                            }
                            catch { }

                            if (!userRepo.EmailExists(record.hr_email))
                                user.email = record.hr_email;

                            user.password = record.hr_password;
                            try
                            {
                                user.last_updated_on = DateTime.Parse(record.modified);
                            }
                            catch { }

                            user.is_active = 1;
                            user.is_approved = 1;
                            user.is_lockedout = 0;
                            user.is_staff = 1;
                            user.is_superuser = 0;

                            userRepo.UpdateUser(user, false);
                        }
                    }
                }

                //Get Users from localdb who are not part this data pull
                var users = userRepo.GetList().Select((x) =>
                                                        {
                                                            foreach (var record in deserializedUserList.data)
                                                            {
                                                                if (x.applicationname == null || x.applicationname.Trim() == "" || !x.applicationname.Contains("vendwatch"))
                                                                {
                                                                    return null;
                                                                }
                                                                else if (record.hr_email == x.username && x.applicationname != null && x.applicationname.Contains("vendwatch"))
                                                                    return null;
                                                            }
                                                            return x;
                                                        }
                                                    ).Where(x => x != null);


                foreach (var user in users)
                {
                    userRepo.DeleteUser(user.id);
                }
                
                log = new synclog();
                log.description = string.Format("Download Table Name - {0}, Rows = {1}", "User", deserializedUserList.data.Count);
                log.createddatetime = DateTime.Now;
                log.type = "Success";
                log.message = "Success";

                syncRepo.AddSynclog(log);
                syncRepo.Save();

                return true;
            }
            catch (Exception ex)
            {
                log.description = "Download Table Name - Users - " + ex.Message;
                log.createddatetime = DateTime.Now;
                log.type = "Failed";
                log.message = "Failed";

                syncRepo.AddSynclog(log);
                syncRepo.Save();
                return false;
            }
        }

        public bool AdjustUnevenBalances()
        {
            return false;
        }

        public void SyncTime()
        {

        }

        public Task<int> SyncScheduledItems()
        {
            return null;
        }

        public Task<int> UpdateScheduledStatusAndItemsQuantity()
        {
            return null;
        }

        public void MockErrorData()
        {
            Event events = new Event();
            events.vm_id = 5569;
            events.source = "Bill";
            events.description = "Bill Entry Jam";
            events.error_code = "ENA";
            events.uuid = Guid.NewGuid().ToString();
            events.timestamp = DateTime.Now.ToUniversalTime().ToString("o");

            var requestCreate = new RestRequest("VWError/create", Method.POST);
            requestCreate.AddHeader("x-rest-password", Password);
            requestCreate.AddHeader("x-rest-username", UserName);
            requestCreate.AddHeader("cache-control", "no-cache");
            requestCreate.RequestFormat = DataFormat.Json;
            requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(events), ParameterType.RequestBody);

            var response = restClient.Execute<ResponseObject>(requestCreate);
        }

        public void MockDoorData()
        {
            Door door = new Door();
            door.vm_id = 5569;
            door.uuid = Guid.NewGuid().ToString();
            door.timestamp = DateTime.Now.ToUniversalTime().ToString("o");
            door.door = "close";

            var requestCreate = new RestRequest("esnStatus/create", Method.POST);
            requestCreate.AddHeader("x-rest-password", Password);
            requestCreate.AddHeader("x-rest-username", UserName);
            requestCreate.AddHeader("cache-control", "no-cache");
            requestCreate.RequestFormat = DataFormat.Json;
            requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(door), ParameterType.RequestBody);

            var response = restClient.Execute<ResponseObject>(requestCreate);
        }

        public void MockCashCollection()
        {
            CashCollection cash = new CashCollection();
            cash.vm_id = 5569;
            cash.uuid = Guid.NewGuid().ToString();
            cash.timestamp = DateTime.Now.ToUniversalTime().ToString("o");
            cash.cash_in = 112.45M;
            cash.cash_out = 14.75M;
            cash.hr_id = 397;

            var requestCreate = new RestRequest("meterValues/create", Method.POST);
            requestCreate.AddHeader("x-rest-password", Password);
            requestCreate.AddHeader("x-rest-username", UserName);
            requestCreate.AddHeader("cache-control", "no-cache");
            requestCreate.RequestFormat = DataFormat.Json;
            requestCreate.AddParameter("application/json; charset=utf-8", JsonConvert.SerializeObject(cash), ParameterType.RequestBody);

            var response = restClient.Execute<ResponseObject>(requestCreate);
        }

        public void GetHRData()
        {
            var requestHrList = new RestRequest("hr", Method.GET);
            requestHrList.AddHeader("x-rest-password", Password);
            requestHrList.AddHeader("x-rest-username", UserName);
            requestHrList.AddHeader("cache-control", "no-cache");

            IRestResponse responseMasterList = restClient.Execute(requestHrList);
            var deserializedProductMasterList = JsonConvert.DeserializeObject<HrListRoot>(responseMasterList.Content);
        }

        public void DEX() {


            SyncDataRepository syncDataRepo = new SyncDataRepository();
            SynclogRepository syncRepo = new SynclogRepository();

            try
            {

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


                ShoppingCartRepository shoppingCartRepo = new ShoppingCartRepository();
                ShoppingCartDetailRepository shoppingDetailCartRepo = new ShoppingCartDetailRepository();
                DateTime? lastSync = null;
                var records = shoppingDetailCartRepo.GetList(lastSync).GroupBy(x => x.itemid).Select(grp => new
                {
                    itemid = grp.First().id,
                    soldQuantity = grp.Count(),
                    amountSales = grp.Sum(x => x.price_tax_included).ToString(),
                    amountDiscounts = grp.Sum(x => x.discount_price).ToString(),
                    itemBarcode = grp.First().barcode,
                    itemPrice = grp.First().price_tax_included,
                }).ToList(); 
                //var records = shoppingDetailCartRepo.GetList().Where(x => x.shoppingcartpkid == "9d483656-a95e-4b92-8487-6bdef0a6d0f0").ToList();
                //var records = shoppingDetailCartRepo.GetList().Where(x => x.itemid == 3988).ToList();

                string todayDate=  DateTime.Now.ToLongDateString();
                todayDate = todayDate.Replace("/","_");

                string path = @"C:\deORO\Vend4MeReports\" + todayDate + ".txt";

                foreach (var soldItem in records)
                {
                  
                  if (File.Exists(path))
                    {
                        using (var tw = new StreamWriter(path, true))
                        {
                            tw.WriteLine(soldItem.itemid + "," + soldItem.itemPrice+ "," + soldItem.itemBarcode);
                            tw.WriteLine(soldItem.soldQuantity + "," + soldItem.amountSales + "," + soldItem.itemBarcode);
                            tw.Close();
                        }
                    }

                  
                }


            }
            catch (Exception ex)
            {
                //synclog log = new synclog();
                //log.description = "UploadData";
                //log.createddatetime = DateTime.Now;
                //log.type = "Failed";
                //log.message = ex.Message.ToString();

                //syncRepo.AddSynclog(log);
                //syncRepo.Save();
                //syncDataRepo.UpdateUpload(DateTime.Now, "Failed");

               // return false;
            }      
        
        }
    }

    /*class ProductMasterListRoot
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<ProductMasterListData> data { get; set; }
  }

    public class ProductMasterListData
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string brand_name { get; set; }
        public string brand_code { get; set; }
        public string package_name { get; set; }
        public string package_code { get; set; }
        public string product_price { get; set; }
        public string product_sku { get; set; }
        public string category_name { get; set; }
    }

    public class ProductListRoot
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<ProductListData> data { get; set; }
    }

    public class ProductListData
    {
        public int id { get; set; }
        public int vm_id { get; set; }
        public int product_id { get; set; }
        public int par { get; set; }
        public int capacity { get; set; }
        public string price { get; set; }
        public int? sort_order { get; set; }
        public int column_size { get; set; }
        public int? inventory { get; set; }
    }

    public class ShoppingCartDetail
    {
        public string uuid { get; set; }
        public int vm_id { get; set; }
        public string timestamp { get; set; }
        public string status { get; set; }
        public List<Product> products { get; set; }
    }

    public class Product
    {
        public string product_id { get; set; }
        public decimal sel_price { get; set; }
        public int sel_vended_r { get; set; }
    }

    public class ResponseObject
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public int errorCode { get; set; }
    }

    public class HrListData
    {
        public int hr_id { get; set; }
        public string hr_name { get; set; }
        public string hr_busphone { get; set; }
        public string hr_busfax { get; set; }
        public string hr_homephone { get; set; }
        public string hr_mobphone { get; set; }
        public string hr_pager { get; set; }
        public string hr_email { get; set; }
        public string hr_addr { get; set; }
        public int hr_hire_date { get; set; }
        public int hr_fire_date { get; set; }
        public int? hr_hh_pass { get; set; }
        public bool hr_setprice { get; set; }
        public int opcenter_id { get; set; }
        public int? hr_number { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public object destroyed { get; set; }
        public object hr_lang { get; set; }
        public string hr_password { get; set; }
    }

    public class HrListRoot
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<HrListData> data { get; set; }
    }


    public class Event
    {
        public string error_code { get; set; }
        public int vm_id { get; set; }
        public string source { get; set; }
        public string description { get; set; }
        public string uuid { get; set; }
        public string timestamp { get; set; }
    }

    public class Door
    {
        public string uuid { get; set; }
        public string timestamp { get; set; }
        public int vm_id { get; set; }
        public string door { get; set; }
    }

    public class CashCollection
    {
        public string error_code { get; set; }
        public int vm_id { get; set; }
        public int hr_id { get; set; }
        public string uuid { get; set; }
        public string timestamp { get; set; }
        public decimal cash_in { get; set; }
        public decimal cash_out { get; set; }

    }

    public class VMRoot
    {
        public bool success { get; set; }
        public string message { get; set; }
        public VMData data { get; set; }
    }

    public class VMData
    {
        public string vm_control { get; set; }
        public string asset_id { get; set; }
        public int opcenter_id { get; set; }
        public int model_alias_id { get; set; }
        public object billval_id { get; set; }
        public object coinval_id { get; set; }
        public object user_button_count { get; set; }
        public object user_column_count { get; set; }
        public string vm_esn { get; set; }
        public object acquire_date { get; set; }
        public string coin_serial { get; set; }
        public string client_number { get; set; }
        public string vm_serial { get; set; }
        public string bill_serial { get; set; }
        public string owner { get; set; }
        public string cabinet_serial { get; set; }
        public string purchase_price { get; set; }
        public string monthly_deprec { get; set; }
        public string changer_value { get; set; }
        public bool vm_warehouse { get; set; }
        public int location_id { get; set; }
        public bool vm_disabled { get; set; }
        public bool vm_virtual { get; set; }
        public bool vm_coin { get; set; }
        public bool vm_standby { get; set; }
        public string vm_esnport { get; set; }
        public bool @static { get; set; }
        public string static_visit_days { get; set; }
        public object static_start_date { get; set; }
        public string latlong { get; set; }
        public int static_freq { get; set; }
        public bool pending_pog { get; set; }
        public object exact_match { get; set; }

    }*/
}
