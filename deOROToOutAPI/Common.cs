using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace deOROToOutAPI
{
    public static class Common
    {
        public static SqlConnection sqlConnection = null;
        public static SqlCommand sqlCommand_SendDatabases = new SqlCommand();
        public static SqlCommand sqlCommand_Initialize1Database = new SqlCommand();
        public static SqlCommand sqlCommand_SendCustomerLocations = new SqlCommand();
        public static SqlCommand sqlCommand_LogAction = new SqlCommand();
        public static SqlCommand sqlCommand_Update1Database = new SqlCommand();
        public static SqlCommand sqlCommand_GetProducts = new SqlCommand();
        public static SqlCommand sqlCommand_GetItemsPerLocations = new SqlCommand();
        public static SqlCommand sqlCommand_RefreshConfigGetItems = new SqlCommand();
        public static SqlCommand sqlCommand_RefreshConfigGetItemsPerLocations = new SqlCommand();
        public static SqlCommand sqlCommand_RefreshConfigGetSales = new SqlCommand();
        public static SqlCommand sqlCommand_SendSales = new SqlCommand();
        public static StringBuilder sbUtils = new StringBuilder();
        public static StringBuilder sbSubCmd = new StringBuilder();
        public static System.Diagnostics.EventLog eventLog;
        public static void WriteToEventLog(string message, System.Diagnostics.EventLogEntryType type) {
            if (message.Length > 16350)
                message = message.Substring(0, 16350);
            eventLog.WriteEntry(message, type);
        }
        public static int RUN_SERVICE_EVERY_X_MINUTES = Properties.Settings.Default.RUN_SERVICE_EVERY_X_MINUTES;

        public const string API_DATABASES_DOWNLOAD = "Download Databases";
        public const string API_CUSTOMERLOCATION_DOWNLOAD = "Download Customer and Locations";
        public const string API_ITEMS_CATALOG = "Upload Items Catalog";
        public const string API_ITEMS_LOCATIONS = "Upload Items per Customer / Location";
        public const string API_SALES = "Download Sales";
        public const string TYPE_SUCCESS = "Success";
        public const string TYPE_FAILURE = "Failure";
        public static string SERVER_NAME = Properties.Settings.Default.SERVER_NAME;
        //public static string API_CUSTOMERLOCATION_DOWNLOAD_SUCCESS = string.Format("Customer and Locations are sent to {0}",Properties.Settings.Default.API_URL_DOMAIN);

        public static string API_KEY = Properties.Settings.Default.API_KEY;
        public static System.Uri API_URL_DOMAIN = new System.Uri(Properties.Settings.Default.API_URL_DOMAIN);
        public static string API_URL_POST_DATABASE = Properties.Settings.Default.API_URL_POST_DATABASE;
        public static string API_URL_POST_CUSTOMER_LOCATIONS = Properties.Settings.Default.API_URL_POST_CUSTOMER_LOCATIONS;
        public static string API_URL_GET_ITEMS_FULL = Properties.Settings.Default.API_URL_GET_ITEMS_FULL;
        public static string API_URL_GET_ITEMS_CATALOG = Properties.Settings.Default.API_URL_GET_ITEMS_CATALOG;
        public static string API_URL_GET_ITEMS_LOCATIONS = Properties.Settings.Default.API_URL_GET_ITEMS_LOCATIONS;
        public static string API_URL_POST_SALES = Properties.Settings.Default.API_URL_POST_SALES;

        private static Regex regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex regexHtmlTagPattern = new Regex("<.*?>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex regexEmptyline = new Regex(@"^\s+$[\r\n]*", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex regexSpecials = new Regex("(&(.*?);)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static string GetPlainTextFromHtml(string htmlString)
        {
            return regexSpecials.Replace(regexEmptyline.Replace(regexHtmlTagPattern.Replace(regexCss.Replace(htmlString, string.Empty), string.Empty), string.Empty)," ");
        }
    }
    public enum RefreshConfig_Flag
    {
        GET_ITEMS = 1,
        GET_ITEMS_LOCATIONS = 2,
        GET_SALES = 3
    }
    
    [DataContract]
    public class APIResponse {
        [DataMember(IsRequired =true)]
        public string status;

        [DataMember]
        public string message;

        [DataMember]
        public object data;
    }
    [DataContract]
    public class APIResponse_DatabaseConfig:IDisposable {
        [DataMember]
        public string name;

        [DataMember]
        public string code;

        [DataMember]
        public bool is_active;

        [DataMember]
        public List<APIResponse_CustomerLocationConfig> custloc;

        void IDisposable.Dispose()
        {
            this.custloc.Clear();
            this.custloc = null;
        }
    }
    [DataContract]
    public class APIResponse_CustomerLocationConfig
    {
        [DataMember]
        public int customerid;
        [DataMember]
        public int locationid;
        [DataMember]
        public bool is_active;
        [DataMember]
        public DateTime? last_sales_down = null;
        [DataMember]
        public int? count_sales_down = null;
        [DataMember]
        public int? id_sales_down = null;
        [DataMember]
        public string serial_number = null;
    }
    [DataContract]
    [Serializable]
    public class Databases:IDisposable
    {
        [DataMember(IsRequired = true)]
        public string server { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public List<Database> dbs { get; set; } = new List<Database>();

        void IDisposable.Dispose()
        {
            this.dbs.Clear();
            this.dbs = null;
        }
    }
    [DataContract]
    [Serializable]
    public class Database:IDisposable
    {
        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember(IsRequired = true)]
        public string code { get; set; }

        [DataMember(IsRequired = true)]
        public int id { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public bool is_active { get; set; } = false;

        [DataMember(EmitDefaultValue = true)]
        public List<Customer> customer = new List<Customer>();

        void IDisposable.Dispose()
        {
            this.customer.Clear();
            this.customer = null;
        }
    }
    [DataContract]
    [Serializable]
    public class Customer:IDisposable
    {
        [DataMember(IsRequired = true)]
        public int id { get; set; }
        
        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string state { get; set; }

        [DataMember]
        public string zip { get; set; }

        [DataMember]
        public string phone { get; set; }

        [DataMember]
        public string fax { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public List<Location> location = new List<Location>();

        void IDisposable.Dispose()
        {
            this.location.Clear();
            this.location = null;
        }
    }
    [DataContract]
    [Serializable]
    public class Location
    {
        [DataMember(IsRequired = true)]
        public int id { get; set; }

        [DataMember(IsRequired = true)]
        public int customerid { get; set; }

        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember]
        public string serial_number { get; set; }

        [DataMember]
        public string address { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string state { get; set; }

        [DataMember]
        public string zip { get; set; }

        [DataMember]
        public string phone { get; set; }

        [DataMember]
        public string fax { get; set; }

        [DataMember]
        public string email { get; set; }

    }
    [DataContract]
    public class APIRequest_Empty {
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string data = "REQ";
    }
    [DataContract]
    public class APIRequest_Items_Catalog {
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string server = Common.SERVER_NAME;
    }
    [DataContract]
    public class APIRequest_Items_Location
    {
        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public string server = Common.SERVER_NAME;

        [DataMember(EmitDefaultValue = true)]
        public string database = null;

        [DataMember(EmitDefaultValue = true)]
        public int? customer = null;

        [DataMember(EmitDefaultValue = true)]
        public int? location = null;
    }

    [DataContract]
    public class ItemCategory {
        [DataMember(IsRequired = true)]
        public string code;

        [DataMember(IsRequired = true)]
        public string name;

        [DataMember]
        public string description;

        [DataMember]
        public float pickorder;

    }
    [DataContract]
    public class ItemManufacturer
    {
        [DataMember(IsRequired = true)]
        public string name;

        [DataMember(IsRequired = true)]
        public string code;

        [DataMember(EmitDefaultValue = true)]
        public bool is_active = true;
    }
    [DataContract]
    public class Item {
        [DataMember(IsRequired = true)]
        public string uniqident;

        [DataMember]
        public string upc;

        [DataMember]
        public string barcode;

        [DataMember(IsRequired = true)]
        public string name;

        [DataMember]
        public string description;

        //[DataMember]
        //public string mname;

        [DataMember]
        public string mcode;

        //[DataMember]
        //public string cname;

        [DataMember]
        public string ccode;

        //[DataMember]
        //public string sname;

        [DataMember]
        public string size;

        [DataMember]
        public int? count;

        [DataMember]
        public float? unitcost;

        [DataMember]
        public float? price;

        [DataMember(EmitDefaultValue = true)]
        public bool is_taxable = false;

        [DataMember]
        public float? price_tax_excluded;

        [DataMember]
        public float? tax_percent;

        [DataMember]
        public float? tax;

        [DataMember]
        public float? crv;

        [DataMember]
        public string avgshelflife;

        [DataMember]
        public float? pickorder;

    }
    [DataContract]
    public class Item4DatabaseCustomerLocation : IDisposable
    {
        [DataMember(IsRequired = true)]
        public string db;

        [DataMember]
        public List<Item4CustomerLocation> items = new List<Item4CustomerLocation>();

        void IDisposable.Dispose()
        {
            this.items.Clear();
            this.items = null;
        }
    }
    [DataContract]
    public class Item4CustomerLocation
    {
        [DataMember(IsRequired = true)]
        public int customerid;

        [DataMember(IsRequired = true)]
        public int locationid;

        [DataMember(IsRequired = true)]
        public string machineid;

        [DataMember(IsRequired = true)]
        public string item_unique_identifier;

        [DataMember]
        public string group;

        [DataMember]
        public string coil;

        [DataMember]
        public int? source_counter;

        [DataMember]
        public int? counter;

        [DataMember]
        public int? level;
    
        [DataMember]
        public int? par; // par_level

        [DataMember]
        public int? depletion_level; // depletion;

        [DataMember]
        public int? quantity; // capacity

        [DataMember]
        public float? unitcost;

        [DataMember]
        public bool is_taxable;

        [DataMember]
        public float? desired_price;

        [DataMember]
        public float? price;

        [DataMember]
        public float? tax;

        [DataMember]
        public float? price_tax_excluded;

        [DataMember]
        public float? tax_percent;

        [DataMember]
        public float? crv;

        [DataMember]
        public int? item_count;
    }
    [DataContract]
    public class ItemFull:IDisposable {
        [DataMember]
        public List<ItemManufacturer> manufacturers;
        [DataMember]
        public List<ItemCategory> categories;
        [DataMember]
        public List<Item> items;
        [DataMember]
        public List<Item4DatabaseCustomerLocation> items4databasecustomerlocation;

        void IDisposable.Dispose()
        {
            this.manufacturers.Clear();
            this.manufacturers = null;

            this.categories.Clear();
            this.categories = null;

            this.items.Clear();
            this.items = null;

            this.items4databasecustomerlocation.Clear();
            this.items4databasecustomerlocation = null;
        }
    }

    [DataContract]
    public class Sales : IDisposable
    {
        [DataMember(EmitDefaultValue = true)]
        public string server = null;

        [DataMember(EmitDefaultValue = true)]
        public string database = null;

        [DataMember(EmitDefaultValue = true)]
        public int? customerid = null;

        [DataMember(EmitDefaultValue = true)]
        public int? locationid = null;

        [DataMember]
        public List<Sale> sales = new List<Sale>();

        void IDisposable.Dispose()
        {
            this.sales.Clear();
            this.sales = null;
        }
    }
    [DataContract]
    [Serializable]
    public class Sale {
        [DataMember(IsRequired = true)]
        public int d; // databaseid

        [DataMember(IsRequired = true)]
        public int c; // customerid

        [DataMember(IsRequired = true)]
        public int l; // locationid

        [DataMember(IsRequired = true)]
        public string u; // item_unique_identifier

        [DataMember(IsRequired = true)]
        public int n; // count

        [DataMember(IsRequired = true)]
        public int i; // max id

        [DataMember(IsRequired = true)]
        public DateTime t; // max created_date_time

        [DataMember(IsRequired = true)]
        public float p; // price_tax_included
    }
}
