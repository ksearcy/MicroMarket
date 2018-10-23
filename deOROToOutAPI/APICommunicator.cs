using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace deOROToOutAPI
{
    public static class APICommunicator
    {
        private static HttpClient httpClient;
        private static HttpResponseMessage httpResponse;
        private static Databases databases;
        private static List<APIResponse_DatabaseConfig> dbconfigs;

        public static void Initialize() {
            httpClient = new HttpClient();
            httpClient.BaseAddress = Common.API_URL_DOMAIN;
            httpClient.Timeout = new System.TimeSpan(0, 0, 120);
            httpClient.DefaultRequestHeaders.Add("DEORO", string.Format("api_key {0}", Common.API_KEY));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            dbconfigs = new List<APIResponse_DatabaseConfig>();
        }

        public static void RunAll() {
            Common.sqlConnection.Open();
            bool safe2continue = SendDatabases();
            if (safe2continue)
            {
                GetItemsCatalog();
                GetItemsPerLocations();
                SendSales();
            }
            Common.sqlConnection.Close();
        }

        public static bool SendDatabases()
        {
            bool ret = false, customer_location = false;
            try
            {
                if (databases != null)
                {
                    databases = null;
                }
                databases = DatabasesCommunicator.SendDatabases();
                httpResponse = httpClient.PostAsync(Common.API_URL_POST_DATABASE, new StringContent(JsonConvert.SerializeObject(databases), Encoding.UTF8, "application/json")).Result;
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(httpResponse.Content.ReadAsStringAsync().Result);

                    if (response.status == "ACK")
                    {
                        dbconfigs.Clear();
                        foreach (var o in (Newtonsoft.Json.Linq.JArray)response.data) {
                            APIResponse_DatabaseConfig current_config = new APIResponse_DatabaseConfig()
                            {
                                code = (string)o["code"],
                                name = (string)o["name"],
                                is_active = (bool)o["is_active"],
                                custloc = new List<APIResponse_CustomerLocationConfig>()
                            };
                            foreach (var cl in o["custloc"]) {
                                current_config.custloc.Add(new APIResponse_CustomerLocationConfig(){
                                    customerid = (int)cl["customerid"],
                                    locationid = (int)cl["locationid"],
                                    is_active = (bool)cl["is_active"],
                                    last_sales_down = ((Newtonsoft.Json.Linq.JValue)cl["last_sales_down"]) != null ? (DateTime?)cl["last_sales_down"]:null,
                                    count_sales_down = ((Newtonsoft.Json.Linq.JValue)cl["count_sales_down"]) != null ? (int?)cl["count_sales_down"] : null,
                                    id_sales_down = ((Newtonsoft.Json.Linq.JValue)cl["id_sales_down"]) != null ? (int?)cl["id_sales_down"] : null,
                                    serial_number = ((Newtonsoft.Json.Linq.JValue)cl["serial_number"]) != null ? (string)cl["serial_number"] : null
                                });
                            }
                            dbconfigs.Add(current_config);
                        }
                        DatabasesCommunicator.ReceiveDatabases(dbconfigs);
                        foreach (APIResponse_DatabaseConfig db in dbconfigs)
                        {
                            Database currdb = databases.dbs.First(d => d.name == db.name);
                            currdb.is_active = db.is_active;
                            currdb.customer.Clear();

                            if (db.is_active)
                            {
                                customer_location = true;                                
                                DatabasesCommunicator.SendCustomerLocations(currdb);
                            }
                        }
                        if (customer_location)
                            SendCustomerLocations();
                        ret = true;
                    }
                    else {
                        foreach (Database db in databases.dbs)
                        {
                            if (db.is_active)
                                DatabasesCommunicator.LogAction(db, -1, -1, String.Empty, Common.API_URL_POST_DATABASE, Common.API_DATABASES_DOWNLOAD, Common.TYPE_FAILURE, response.message);
                        }
                        Common.WriteToEventLog(String.Format("API Response NAK\n{0}", response.message), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
                else
                {
                    Common.WriteToEventLog(String.Format("API Response Error\n{0}", httpResponse.Content.ReadAsStringAsync().Result), System.Diagnostics.EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Common.WriteToEventLog(String.Format("{0}\n{1}", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
            return ret;
        }

        public static void SendCustomerLocations()
        {
            try
            {
                httpResponse = httpClient.PostAsync(Common.API_URL_POST_CUSTOMER_LOCATIONS, new StringContent(JsonConvert.SerializeObject(databases), Encoding.UTF8, "application/json")).Result;
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(httpResponse.Content.ReadAsStringAsync().Result);

                    if (response.status == "ACK")
                    {
                        foreach (Database db in databases.dbs)
                        {
                            if (db.is_active)
                                DatabasesCommunicator.LogAction(db, -1, -1, String.Empty, Common.API_URL_POST_CUSTOMER_LOCATIONS, Common.API_CUSTOMERLOCATION_DOWNLOAD, Common.TYPE_SUCCESS, System.DBNull.Value);
                        }
                    }
                    else
                    {
                        foreach (Database db in databases.dbs)
                        {
                            if (db.is_active)
                                DatabasesCommunicator.LogAction(db, -1, -1, String.Empty, Common.API_URL_POST_CUSTOMER_LOCATIONS, Common.API_CUSTOMERLOCATION_DOWNLOAD, Common.TYPE_FAILURE, response.message);
                        }
                        Common.WriteToEventLog(String.Format("API Response NAK\n{0}", response.message), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
                else
                {
                    Common.WriteToEventLog(String.Format("API Response Error\n{0}", Common.GetPlainTextFromHtml(httpResponse.Content.ReadAsStringAsync().Result)), System.Diagnostics.EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Common.WriteToEventLog(String.Format("{0}\n{1}", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public static void GetItemsCatalog() {
            try
            {
                string url, msg;
                url = Common.API_URL_GET_ITEMS_CATALOG;
                msg = Common.API_ITEMS_CATALOG;
                
                httpResponse = httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new APIRequest_Items_Catalog() {server=Common.SERVER_NAME}), Encoding.UTF8, "application/json")).Result;
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(httpResponse.Content.ReadAsStringAsync().Result);

                    if (response.status == "ACK")
                    {
                        ItemFull itemFull = new ItemFull()
                        {
                            manufacturers = new List<ItemManufacturer>(),
                            categories = new List<ItemCategory>(),
                            items = new List<Item>(),
                            items4databasecustomerlocation = new List<Item4DatabaseCustomerLocation>()
                        };
                        
                        foreach (var o in (Newtonsoft.Json.Linq.JArray)(((Newtonsoft.Json.Linq.JObject)response.data)["manufacturers"]))
                        {
                            itemFull.manufacturers.Add(new ItemManufacturer() {
                                code = (string)o["code"],
                                name = (string)o["name"]
                            });
                        }
                        foreach (var o in (Newtonsoft.Json.Linq.JArray)(((Newtonsoft.Json.Linq.JObject)response.data)["categories"]))
                        {
                            itemFull.categories.Add(new ItemCategory() {
                                code = (string)o["code"],
                                name = (string)o["name"],
                                description = (string)o["description"],
                                pickorder = float.Parse((string)o["pickorder"])
                            });
                        }
                        foreach (var o in (Newtonsoft.Json.Linq.JArray)(((Newtonsoft.Json.Linq.JObject)response.data)["items"]))
                        {
                            itemFull.items.Add(new Item() {
                                uniqident = (string)o["uniqident"],
                                upc = ((Newtonsoft.Json.Linq.JValue)o["upc"]).Value != null ? (string)o["upc"] : null,
                                barcode = ((Newtonsoft.Json.Linq.JValue)o["barcode"]).Value != null ? (string)o["barcode"] : null,
                                name = ((Newtonsoft.Json.Linq.JValue)o["name"]).Value != null ? ((string)o["name"]).Replace("''","'") : null,
                                description = ((Newtonsoft.Json.Linq.JValue)o["description"]).Value != null ? (string)o["description"] : null,
                                //mname = (string)o["mname"],
                                mcode = (string)o["mcode"],
                                //cname = (string)o["cname"],
                                ccode = (string)o["ccode"],
                                //sname = (string)o["sname"],
                                size = (string)o["size"],
                                count = ((Newtonsoft.Json.Linq.JValue)o["count"]).Value != null ? (int?)int.Parse((string)o["count"]) : null,
                                unitcost = ((Newtonsoft.Json.Linq.JValue)o["unitcost"]).Value != null?(float?)float.Parse((string)o["unitcost"]):null,
                                price = ((Newtonsoft.Json.Linq.JValue)o["price"]).Value != null ? (float?)float.Parse((string)o["price"]) : null,
                                is_taxable = (bool)o["is_taxable"],
                                price_tax_excluded = ((Newtonsoft.Json.Linq.JValue)o["price_tax_excluded"]).Value != null ? (float?)float.Parse((string)o["price_tax_excluded"]) : null,
                                tax_percent = ((Newtonsoft.Json.Linq.JValue)o["tax_percent"]).Value != null ? (float?)float.Parse((string)o["tax_percent"]) : null,
                                tax = ((Newtonsoft.Json.Linq.JValue)o["tax"]).Value != null ? (float?)float.Parse((string)o["tax"]) : null,
                                crv = ((Newtonsoft.Json.Linq.JValue)o["crv"]).Value != null ? (float?)float.Parse((string)o["crv"]) : null,
                                avgshelflife = ((Newtonsoft.Json.Linq.JValue)o["avgshelflife"]).Value != null ? (string)o["avgshelflife"] : null,
                                pickorder = ((Newtonsoft.Json.Linq.JValue)o["pickorder"]).Value != null ? (float?)float.Parse((string)o["pickorder"]) : null
                            });
                        }
                        
                        DatabasesCommunicator.GetItemsCatalog(databases, itemFull, url, msg);

                        itemFull.categories.Clear();
                        itemFull.categories = null;
                        itemFull.manufacturers.Clear();
                        itemFull.manufacturers = null;
                        itemFull.items.Clear();
                        itemFull.items = null;
                        itemFull.items4databasecustomerlocation.Clear();
                        itemFull.items4databasecustomerlocation = null;
                    }
                    else
                    {
                        foreach (Database db in databases.dbs)
                        {
                            if (db.is_active)
                            {
                                DatabasesCommunicator.LogAction(db, -1, -1, String.Empty, url, msg, Common.TYPE_FAILURE, response.message);
                            }
                        }
                        Common.WriteToEventLog(String.Format("API Response NAK\n{0}", response.message), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
                else
                {
                    Common.WriteToEventLog(String.Format("API Response Error\n{0}", Common.GetPlainTextFromHtml(httpResponse.Content.ReadAsStringAsync().Result)), System.Diagnostics.EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Common.WriteToEventLog(String.Format("{0}\n{1}", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public static void GetItemsPerLocations(string database = null, int? customer = null, int? location = null, string machineid = null)
        {
            try
            {
                string url, msg;
                url = Common.API_URL_GET_ITEMS_LOCATIONS;
                msg = Common.API_ITEMS_LOCATIONS;

                httpResponse = httpClient.PostAsync(url, 
                                        new StringContent(
                                            JsonConvert.SerializeObject(new APIRequest_Items_Location(){
                                                server = Common.SERVER_NAME,
                                                database = database,
                                                customer = customer,
                                                location = location}), 
                                                Encoding.UTF8, "application/json")).Result;
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(httpResponse.Content.ReadAsStringAsync().Result);

                    if (response.status == "ACK")
                    {
                        ItemFull itemFull = new ItemFull()
                        {
                            manufacturers = new List<ItemManufacturer>(),
                            categories = new List<ItemCategory>(),
                            items = new List<Item>(),
                            items4databasecustomerlocation = new List<Item4DatabaseCustomerLocation>()
                        };

                        Item4DatabaseCustomerLocation currItem = null;
                        foreach (var o in (Newtonsoft.Json.Linq.JArray)(((Newtonsoft.Json.Linq.JObject)response.data)["planograms"]))
                        {
                            if (currItem == null)
                            {
                                currItem = new Item4DatabaseCustomerLocation() { db = (string)o["database"] };
                                itemFull.items4databasecustomerlocation.Add(currItem);
                            }
                            else if (currItem.db != (string)o["database"]) {
                                currItem = itemFull.items4databasecustomerlocation.Find(d => d.db == (string)o["database"]);
                                if (currItem == null) {
                                    currItem = new Item4DatabaseCustomerLocation() { db = (string)o["database"] };
                                    itemFull.items4databasecustomerlocation.Add(currItem);
                                }
                            }
                            currItem.items.Add(new Item4CustomerLocation() { 
                                customerid = (int)o["customerid"],
                                locationid = (int)o["locationid"],
                                machineid = (string)o["machineid"],

                                item_unique_identifier = (string)o["item_unique_identifier"],
                                coil = ((Newtonsoft.Json.Linq.JValue)o["coil"]).Value != null ? (string)o["coil"] : null,
                                group = ((Newtonsoft.Json.Linq.JValue)o["group"]).Value != null ? (string)o["group"] : null,

                                source_counter = ((Newtonsoft.Json.Linq.JValue)o["source_counter"]).Value != null ? (int?)int.Parse((string)o["source_counter"]) : null,
                                counter = ((Newtonsoft.Json.Linq.JValue)o["counter"]).Value != null ? (int?)int.Parse((string)o["counter"]) : null,
                                level = ((Newtonsoft.Json.Linq.JValue)o["level"]).Value != null ? (int?)int.Parse((string)o["level"]) : null,
                                depletion_level = ((Newtonsoft.Json.Linq.JValue)o["depletion_level"]).Value != null ? (int?)int.Parse((string)o["depletion_level"]) : null,
                                par = ((Newtonsoft.Json.Linq.JValue)o["par"]).Value != null ? (int?)int.Parse((string)o["par"]) : null,
                                quantity = ((Newtonsoft.Json.Linq.JValue)o["quantity"]).Value != null ? (int?)int.Parse((string)o["quantity"]) : null,
                                item_count = ((Newtonsoft.Json.Linq.JValue)o["item_count"]).Value != null ? (int?)int.Parse((string)o["item_count"]) : null,

                                unitcost = ((Newtonsoft.Json.Linq.JValue)o["unitcost"]).Value != null ? (float?)float.Parse((string)o["unitcost"]) : null,
                                price = ((Newtonsoft.Json.Linq.JValue)o["price"]).Value != null ? (float?)float.Parse((string)o["price"]) : null,
                                desired_price = ((Newtonsoft.Json.Linq.JValue)o["desired_price"]).Value != null ? (float?)float.Parse((string)o["desired_price"]) : null,
                                is_taxable = (bool)o["is_taxable"],
                                price_tax_excluded = ((Newtonsoft.Json.Linq.JValue)o["price_tax_excluded"]).Value != null ? (float?)float.Parse((string)o["price_tax_excluded"]) : null,
                                tax_percent = ((Newtonsoft.Json.Linq.JValue)o["tax_percent"]).Value != null ? (float?)float.Parse((string)o["tax_percent"]) : null,
                                tax = ((Newtonsoft.Json.Linq.JValue)o["tax"]).Value != null ? (float?)float.Parse((string)o["tax"]) : null,
                                crv = ((Newtonsoft.Json.Linq.JValue)o["crv"]).Value != null ? (float?)float.Parse((string)o["crv"]) : null
                            });
                        }

                        DatabasesCommunicator.GetItemsPerLocations(databases, itemFull, url, msg);

                        itemFull.categories.Clear();
                        itemFull.categories = null;
                        itemFull.manufacturers.Clear();
                        itemFull.manufacturers = null;
                        itemFull.items.Clear();
                        itemFull.items = null;
                        itemFull.items4databasecustomerlocation.Clear();
                        itemFull.items4databasecustomerlocation = null;
                    }
                    else
                    {
                        foreach (Database db in databases.dbs)
                        {
                            if (db.is_active)
                                DatabasesCommunicator.LogAction(db, -1, -1, String.Empty, url, msg, Common.TYPE_FAILURE, response.message);
                        }
                        Common.WriteToEventLog(String.Format("API Response NAK\n{0}", response.message), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
                else
                {
                    Common.WriteToEventLog(String.Format("API Response Error\n{0}", Common.GetPlainTextFromHtml(httpResponse.Content.ReadAsStringAsync().Result)), System.Diagnostics.EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Common.WriteToEventLog(String.Format("{0}\n{1}", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        public static void SendSales(string database = null, int? customer = null, int? location = null) {
            try
            {

                Sales sales = DatabasesCommunicator.SendSales(dbconfigs: dbconfigs, database:database, customerid:customer, locationid:location);
                if (sales.sales.Count == 0) {
                    // if not sales to send, do not contact linode
                    return;
                }

                httpResponse = httpClient.PostAsync(Common.API_URL_POST_SALES, new StringContent(JsonConvert.SerializeObject(sales), Encoding.UTF8, "application/json")).Result;

                if (httpResponse.IsSuccessStatusCode)
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(httpResponse.Content.ReadAsStringAsync().Result);

                    if (response.status == "ACK")
                    {
                        if (database != null) {
                            if (customer != null) {
                                if (location != null) {
                                    DatabasesCommunicator.LogAction((string)database, (int)customer, (int)location, databases.dbs.Find(x=> x.name==(string)database).customer.Find(x=>x.id==(int)customer).location.Find(x=>x.id==(int)location).serial_number, Common.API_URL_POST_SALES, Common.API_SALES, Common.TYPE_SUCCESS, response.message);
                                }
                                DatabasesCommunicator.LogAction((string)database, (int)customer, -1, String.Empty, Common.API_URL_POST_SALES, Common.API_SALES, Common.TYPE_SUCCESS, response.message);
                            }
                            DatabasesCommunicator.LogAction(database, -1, -1, String.Empty, Common.API_URL_POST_SALES, Common.API_SALES, Common.TYPE_SUCCESS, response.message);
                        }
                        else
                        {
                            foreach (APIResponse_DatabaseConfig db in dbconfigs) {
                                if (db.is_active) {
                                    DatabasesCommunicator.LogAction(db.name, -1, -1, String.Empty, Common.API_URL_POST_SALES, Common.API_SALES, Common.TYPE_SUCCESS, response.message);
                                    foreach (APIResponse_CustomerLocationConfig cl in db.custloc) {
                                        if (cl.is_active) {
                                            DatabasesCommunicator.LogAction(db.name, cl.customerid, cl.locationid, databases.dbs.Find(x => x.name == db.name).customer.Find(x => x.id == cl.customerid).location.Find(x => x.id == cl.locationid).serial_number, Common.API_URL_POST_SALES, Common.API_SALES, Common.TYPE_SUCCESS, response.message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Database db in databases.dbs)
                        {
                            if (db.is_active)
                                DatabasesCommunicator.LogAction(db, -1, -1, String.Empty, Common.API_URL_POST_SALES, Common.API_SALES, Common.TYPE_FAILURE, response.message);
                        }
                        Common.WriteToEventLog(String.Format("API Response NAK\n{0}", response.message), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
                else
                {
                    Common.WriteToEventLog(String.Format("API Response Error\n{0}", httpResponse.Content.ReadAsStringAsync().Result), System.Diagnostics.EventLogEntryType.Error);
                }
            }
            catch (Exception ex)
            {
                Common.WriteToEventLog(String.Format("{0}\n{1}", ex.Message, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
