using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deOROToOutAPI
{
    public static class DatabasesCommunicator
    {
        #region SQL Commands
        private const string COMMAND_SendDatabases = "SELECT [dbid], [name] FROM master.dbo.sysdatabases WHERE [name] LIKE 'deORO_%' AND [name] != 'deORO_Local_Backup';";
        private const string COMMAND_Initialize1Database = @"USE [{0}];
IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE [object_id] = OBJECT_ID(N'[dbo].[outapi_config]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[outapi_config](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[customerid] INT NOT NULL DEFAULT -1,
	[locationid] INT NOT NULL DEFAULT -1,
	[is_active] [bit] NOT NULL,
	[last_database_operation] [datetime] NULL,
	[last_customer_down] [datetime] NULL,
	[count_customer_down] [int] NULL,
	[id_customer_down] [int] NULL,
	[last_location_down] [datetime] NULL,
	[count_location_down] [int] NULL,
	[id_location_down] [int] NULL,
	[last_item_up] [datetime] NULL,
	[count_item_up] [int] NULL,
	[id_item_up] [int] NULL,
	[last_planogram_up] [datetime] NULL,
	[count_planogram_up] [int] NULL,
	[id_planogram_up] [int] NULL,
	[last_sales_down] [datetime] NULL,
	[count_sales_down] [int] NULL,
	[id_sales_down] [int] NULL,
 CONSTRAINT [PK_outapi_config] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
CREATE UNIQUE NONCLUSTERED INDEX [IX_outapi_config_location_customer] ON [dbo].[outapi_config]
(
	[customerid] ASC,
	[locationid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
END
IF  NOT EXISTS (SELECT * FROM sys.objects 
WHERE [object_id] = OBJECT_ID(N'[dbo].[outapi_log]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[outapi_log](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[customerid] [int] NOT NULL DEFAULT -1,
	[locationid] [int] NOT NULL DEFAULT -1,
	[machineid] [varchar](128) NULL,
	[api] [varchar](256) NULL,
	[description] [varchar](max) NULL,
	[type] [varchar](50) NULL,
	[message] [varchar](max) NULL,
	[created_date_time] [datetime] NULL
) ON [PRIMARY];
END";
        private const string COMMAND_SendCustomerLocations = @"USE [{0}];
SELECT [id]
      ,[name]
      ,[address]
      ,[city]
      ,[state]
      ,[zip]
      ,[phone]
      ,[fax]
      ,[email_address]
FROM [dbo].[customer] c WHERE c.[is_active] = 1
ORDER BY [id] ASC;
SELECT [id]
      ,[customerid]
      ,[name]
      ,[address]
      ,[city]
      ,[state]
      ,[zip]
      ,[phone]
      ,[fax]
      ,[email_address]
      ,[serial_number]
FROM [dbo].[location] l WHERE l.[is_active] = 1 AND EXISTS(SELECT * FROM [dbo].[customer] c WHERE c.[is_active] = 1 AND c.id=l.customerid)
ORDER BY [customerid] ASC, [id] ASC;";
        private const string COMMAND_LogAction = @"USE [{0}];
INSERT INTO [dbo].[outapi_log]
           ([customerid]
           ,[locationid]
           ,[machineid]
           ,[api]
           ,[description]
           ,[type]
           ,[message]
           ,[created_date_time])
     VALUES
           (@customerid
           ,@locationid
           ,@machineid
           ,@api
           ,@description
           ,@type
           ,@message
           ,GETDATE());
";
        private const string COMMAND_UseDatabase = "USE [{0}]; ";
        private const string COMMAND_UpdateConfig_Part1 = @"
MERGE [dbo].[outapi_config] WITH (HOLDLOCK) AS target
USING (VALUES ";
        private const string COMMAND_UpdateConfig_Part2 = "({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})";
        private const string COMMAND_UpdateConfig_Part3 = @")
    AS source (customerid, locationid, is_active, last_database_operation, 
	last_customer_down, count_customer_down, id_customer_down, 
	last_location_down, count_location_down, id_location_down,
    last_sales_down, count_sales_down, id_sales_down)
    ON target.customerid = source.customerid and target.locationid = source.locationid
WHEN MATCHED THEN
    UPDATE
    SET target.is_active = source.is_active,
		target.last_database_operation = ISNULL(source.last_database_operation,target.last_database_operation),
		target.last_customer_down = ISNULL(source.last_customer_down,target.last_customer_down),
		target.count_customer_down = ISNULL(source.count_customer_down,target.count_customer_down),
		target.id_customer_down = ISNULL(source.id_customer_down,target.id_customer_down),
		target.last_location_down = ISNULL(source.last_location_down,target.last_location_down),
		target.count_location_down = ISNULL(source.count_location_down,target.count_location_down),
		target.id_location_down = ISNULL(source.id_location_down,target.id_location_down),
        target.last_sales_down = ISNULL(source.last_sales_down,target.last_sales_down),
		target.count_sales_down = ISNULL(source.count_sales_down,target.count_sales_down),
		target.id_sales_down = ISNULL(source.id_sales_down,target.id_sales_down)
WHEN NOT MATCHED BY target THEN
    INSERT(customerid, locationid, is_active, last_database_operation,
	last_customer_down, count_customer_down, id_customer_down, 
	last_location_down, count_location_down, id_location_down,
    last_sales_down, count_sales_down, id_sales_down)
    VALUES (source.customerid, source.locationid, source.is_active, source.last_database_operation,
	source.last_customer_down, source.count_customer_down, source.id_customer_down, 
	source.last_location_down, source.count_location_down, source.id_location_down,
    source.last_sales_down, source.count_sales_down, source.id_sales_down)
WHEN NOT MATCHED BY source THEN
    UPDATE
    SET target.is_active = 0;
";
        private const string COMMAND_UpdateConfig_Part4 = @"
    UPDATE dbo.location 
    SET serial_number = '{0}'
    WHERE customerid = {1} AND id = {2}
    AND (serial_number IS NULL OR serial_number != '{0}'); 
";
        private const string COMMAND_UpdateCategories_Part1 = @"
MERGE [dbo].[category] WITH (HOLDLOCK) AS target
USING (VALUES ";
        private const string COMMAND_UpdateCategories_Part2 = "('{0}','{1}',{2},{3})";
        private const string COMMAND_UpdateCategories_Part3 = @")
    AS source (code, [name], [description], pick_order)
	ON target.code = source.code
WHEN MATCHED THEN
    UPDATE
    SET target.[name] = source.[name],
		target.[description] = source.[description],
		target.pick_order = source.pick_order
WHEN NOT MATCHED THEN
    INSERT(code, [name], [description], pick_order)
    VALUES (source.code, source.[name], source.[description], source.pick_order);
";
        private const string COMMAND_UpdateManufacturers_Part1 = @"
MERGE [dbo].[manufacturer] WITH (HOLDLOCK) AS target
USING (VALUES ";
        private const string COMMAND_UpdateManufacturers_Part2 = "('{0}','{1}')";
        private const string COMMAND_UpdateManufacturers_Part3 = @")
    AS source (code, [name])
	ON target.code = source.code
WHEN MATCHED THEN
    UPDATE
    SET target.[name] = source.[name],
		target.is_active = 1
WHEN NOT MATCHED BY target THEN
    INSERT(code, [name], is_active)
    VALUES (source.code, source.[name], 1)
WHEN NOT MATCHED BY source THEN
	UPDATE 
	SET target.is_active = 0;
";
        private const string COMMAND_UpdateItems_Part1 = @"
;WITH items_source AS
(
   SELECT source.uniqueident, source.upc, source.barcode, source.[name], source.[description], m.id as mid, c.id as cid, [count], source.size, source.unitcost, source.price, source.is_taxable, source.price_tax_excluded, source.tax_percent, source.tax, source.crv, source.avgshelflife, source.pickorder
   FROM (VALUES ";
        private const string COMMAND_UpdateItems_Part2 = "('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})";
        private const string COMMAND_UpdateItems_Part3 = @")
      AS SOURCE(uniqueident, upc, barcode, [name], [description], mcode, ccode, [count], size, unitcost, price, is_taxable, price_tax_excluded, tax_percent, tax, crv, avgshelflife, pickorder)
   LEFT OUTER JOIN [dbo].[manufacturer] m ON m.code = source.mcode
   LEFT OUTER JOIN [dbo].[category] c ON c.code = source.ccode AND c.parentid IS NULL -- Added just for testing to make this works, remove once we make sure codes are unique
)
MERGE [dbo].[item] WITH (HOLDLOCK) AS target
USING items_source
    --ON target.uniqueident = items_source.uniqueident
	--ON target.upc = items_source.upc
	ON (target.unique_identifier = items_source.uniqueident OR target.barcode = items_source.barcode OR target.barcode = items_source.upc)
WHEN MATCHED THEN
    UPDATE
    SET target.[upc] = items_source.[upc],
		target.[name] = items_source.[name],
		target.[manufacturerid] = items_source.[mid],
		target.[categoryid] = items_source.[cid],
		target.[barcode] = items_source.[barcode],
		target.[has_barcode] = CASE WHEN ((items_source.[barcode] is null) or (items_source.[barcode] = '')) THEN 0 ELSE 1 END,
		target.[description] = items_source.[description],
		target.[count] = items_source.[count],
        --target.[size] = items_source.[size],
		target.[unitcost] = items_source.[unitcost],
		target.[avgshelflife] = items_source.[avgshelflife],
		target.[pickorder] = items_source.[pickorder],
		target.[price] = items_source.[price_tax_excluded],
        target.[is_taxable] = items_source.[is_taxable],
        target.[price_tax_included] = items_source.[price],
        target.[tax_percent] = items_source.[tax_percent],
        target.[tax] = items_source.[tax],
        target.[crv] = items_source.[crv],
        target.[is_active] = 1
WHEN NOT MATCHED BY target THEN
    INSERT(/*[uniqueident], */[upc], [name], 
	[manufacturerid], [categoryid], 
    [is_active],
	[barcode], [has_barcode],
	[description], [count], [unitcost], 
	[avgshelflife], [pickorder], 
    [price], [is_taxable], [price_tax_included], [tax_percent], [tax], [crv])
    VALUES (/*items_source.[uniqueident], */ items_source.[upc], items_source.[name], 
	items_source.[mid], items_source.[cid], 
    1,
	items_source.[barcode], CASE WHEN ((items_source.[barcode] is null) or (items_source.[barcode] = '')) THEN 0 ELSE 1 END,
	items_source.[description], items_source.[count], items_source.[unitcost], 
	items_source.[avgshelflife], items_source.[pickorder], 
    items_source.[price_tax_excluded], items_source.[is_taxable], items_source.[price], items_source.[tax_percent], items_source.[tax], items_source.[crv])
WHEN NOT MATCHED BY source THEN
    UPDATE 
    SET target.is_active = 0;
";

        private const string COMMAND_UpdateItemsPerLocation_Part1 = @"
;WITH items_source AS
(
   SELECT source.customerid, source.locationid, source.machineid, source.item_unique_identifier, i.id as [item_id], source.source_counter, source.counter, source.level, source.par, source.depletion_level, source.quantity, source.unitcost, source.price, source.desired_price, source.is_taxable, source.price_tax_excluded, source.tax_percent, source.tax, source.crv, source.item_count
   FROM (VALUES ";
        private const string COMMAND_UpdateItemsPerLocation_Part2 = "({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18})";
        private const string COMMAND_UpdateItemsPerLocation_Part3 = @")
      AS SOURCE(customerid, locationid, machineid, item_unique_identifier, source_counter, counter, level, par, depletion_level, quantity, unitcost, price, desired_price, is_taxable, price_tax_excluded, tax_percent, tax, crv, item_count)
   INNER JOIN [dbo].[item] i 
      ON i.barcode = source.item_unique_identifier 
      --ON i.uniqueident = source.item_unique_identifier
      --ON i.upc = source.item_unique_identifier
)
MERGE [dbo].[location_item] WITH (HOLDLOCK) AS target
USING items_source
    ON target.itemid = items_source.item_id
    AND target.customerid = items_source.customerid
    AND target.locationid = items_source.locationid
WHEN MATCHED THEN
    UPDATE
    SET target.[is_taxable] = items_source.[is_taxable],
        target.[price] = items_source.[price_tax_excluded],
        target.[tax] = items_source.[tax],
        target.[price_tax_included] = items_source.[price],
        target.[tax_percent] = items_source.[tax_percent],
        target.[crv] = items_source.[crv],
        target.[par] = items_source.[par],
        target.[quantity] = items_source.[quantity],
        target.[depletion_level] = items_source.[depletion_level],
        target.[is_active] = 1
WHEN NOT MATCHED BY target THEN
    INSERT([customerid], [locationid], [itemid], 
	--[discountid],
    [price], [is_taxable], [price_tax_included], [tax_percent], [tax], [crv],
    [par], [quantity], [depletion_level], 
    [created_date_time],
    [is_active])
    VALUES (items_source.[customerid], items_source.[locationid], items_source.[item_id], 
    --discount_id, nemam ga
    items_source.[price_tax_excluded], items_source.[is_taxable], items_source.[price], items_source.[tax_percent], items_source.[tax], items_source.[crv],
    items_source.[par], items_source.[quantity], items_source.[depletion_level],
    GETDATE(), 
	1)
WHEN NOT MATCHED BY source THEN
    UPDATE 
    SET target.is_active = 0;
";
        private const string COMMAND_RefreshConfig_GetItems = @"USE [{0}];
UPDATE [dbo].[outapi_config]
   SET [last_item_up] = GETDATE()
      ,[count_item_up] = @count_item_up
      ,[id_item_up] = @id_item_up
 WHERE [customerid] = @customerid AND [locationid] = @locationid;
";
        private const string COMMAND_RefreshConfig_GetItemsPerLocations = @"USE [{0}];
UPDATE [dbo].[outapi_config]
   SET [last_planogram_up] = GETDATE()
      ,[count_planogram_up] = @count_planogram_up
      ,[id_planogram_up] = @id_planogram_up
 WHERE [customerid] = @customerid AND [locationid] = @locationid;
";
        private const string COMMAND_RefreshConfig_GetSales = @"USE [{0}];
UPDATE [dbo].[outapi_config]
   SET [last_sales_down] = GETDATE()
      ,[count_sales_down] = @count_sales_down
      ,[id_sales_down] = @id_sales_down
 WHERE [customerid] = @customerid AND [locationid] = @locationid
";
        private const string COMMAND_SendSales = @"USE [{0}];
SELECT DB_ID() as [databaseid], scd.[customerid], scd.[locationid], i.[unique_identifier], count(scd.id) as [count], max(scd.id) as [id], max(scd.created_date_time) as [created_date_time], scd.price_tax_included
  FROM [dbo].[shoppingcartdetail] scd
  INNER JOIN [dbo].[item] i ON scd.itemid = i.id
  INNER JOIN [dbo].[outapi_config] c ON c.customerid = scd.customerid AND c.locationid = scd.locationid 
  AND c.is_active = 1 AND ISNULL(c.last_sales_down,GETDATE()-1) < scd.created_date_time AND ISNULL(c.id_sales_down,0) < scd.id
  AND i.barcode != 'ACCOUNT_REFILL_BARCODE' 
  {1}
  GROUP BY scd.[customerid], scd.[locationid], i.[unique_identifier], scd.price_tax_included

";
        #endregion

        public static void Initialize() {
            Common.sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["deOROToOutAPI.Properties.Settings.deORO_Server"].ConnectionString);

            Common.sqlCommand_SendDatabases.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_SendDatabases.Connection = Common.sqlConnection;
            Common.sqlCommand_SendDatabases.CommandText = COMMAND_SendDatabases;

            Common.sqlCommand_Initialize1Database.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_Initialize1Database.Connection = Common.sqlConnection;

            Common.sqlCommand_Update1Database.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_Update1Database.Connection = Common.sqlConnection;

            Common.sqlCommand_SendCustomerLocations.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_SendCustomerLocations.Connection = Common.sqlConnection;

            Common.sqlCommand_GetProducts.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_GetProducts.Connection = Common.sqlConnection;

            Common.sqlCommand_GetItemsPerLocations.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_GetItemsPerLocations.Connection = Common.sqlConnection;

            Common.sqlCommand_SendSales.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_SendSales.Connection = Common.sqlConnection;

            Common.sqlCommand_LogAction.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_LogAction.Connection = Common.sqlConnection;
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@customerid",System.Data.SqlDbType.Int));
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@locationid", System.Data.SqlDbType.Int));
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@machineid", System.Data.SqlDbType.VarChar, 128));
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@api", System.Data.SqlDbType.VarChar, 256));
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@description", System.Data.SqlDbType.VarChar));
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@type", System.Data.SqlDbType.VarChar, 50));
            Common.sqlCommand_LogAction.Parameters.Add(new SqlParameter("@message", System.Data.SqlDbType.VarChar));

            Common.sqlCommand_RefreshConfigGetItems.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_RefreshConfigGetItems.Connection = Common.sqlConnection;
            Common.sqlCommand_RefreshConfigGetItems.Parameters.Add(new SqlParameter("@customerid", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetItems.Parameters.Add(new SqlParameter("@locationid", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetItems.Parameters.Add(new SqlParameter("@count_item_up", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetItems.Parameters.Add(new SqlParameter("@id_item_up", System.Data.SqlDbType.Int));

            Common.sqlCommand_RefreshConfigGetItemsPerLocations.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_RefreshConfigGetItemsPerLocations.Connection = Common.sqlConnection;
            Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters.Add(new SqlParameter("@customerid", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters.Add(new SqlParameter("@locationid", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters.Add(new SqlParameter("@count_planogram_up", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters.Add(new SqlParameter("@id_planogram_up", System.Data.SqlDbType.Int));

            Common.sqlCommand_RefreshConfigGetSales.CommandType = System.Data.CommandType.Text;
            Common.sqlCommand_RefreshConfigGetSales.Connection = Common.sqlConnection;
            Common.sqlCommand_RefreshConfigGetSales.Parameters.Add(new SqlParameter("@customerid", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetSales.Parameters.Add(new SqlParameter("@locationid", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetSales.Parameters.Add(new SqlParameter("@count_sales_down", System.Data.SqlDbType.Int));
            Common.sqlCommand_RefreshConfigGetSales.Parameters.Add(new SqlParameter("@id_sales_up", System.Data.SqlDbType.Int));
        }

        public static Databases SendDatabases() {
            Databases ret = new Databases()
            {
                server = Common.SERVER_NAME,
                dbs = new List<Database>()
            };
            string current_database_name;
            Database current_database;
            System.Data.SqlClient.SqlDataReader rdr = Common.sqlCommand_SendDatabases.ExecuteReader();
            while (rdr.Read())
            {
                current_database_name = (string)rdr["name"];
                current_database = new Database()
                {
                    name = current_database_name,
                    code = current_database_name,
                    id = int.Parse(rdr["dbid"].ToString())
                };
                ret.dbs.Add(current_database);
                //SendCustomerLocations(current_database);
            }
            rdr.Close();
            rdr = null;
            
            return ret;
        }

        public static void SendCustomerLocations(Database database) {
            Common.sqlCommand_SendCustomerLocations.CommandText = string.Format(COMMAND_SendCustomerLocations, database.name);
            System.Data.SqlClient.SqlDataReader rdr = Common.sqlCommand_SendCustomerLocations.ExecuteReader();
            while (rdr.Read()) {
                database.customer.Add(new Customer()
                {
                    id =(int)rdr["id"],
                    name = rdr["name"] == DBNull.Value?null:(string)rdr["name"],
                    address = rdr["address"] == DBNull.Value ? null : (string)rdr["address"],
                    city = rdr["city"] == DBNull.Value ? null : (string)rdr["city"],
                    state = rdr["state"] == DBNull.Value ? null : (string)rdr["state"],
                    zip = rdr["zip"] == DBNull.Value ? null : (string)rdr["zip"],
                    phone = rdr["phone"] == DBNull.Value ? null : (string)rdr["phone"],
                    fax = rdr["fax"] == DBNull.Value ? null : (string)rdr["fax"],
                    email = rdr["email_address"] == DBNull.Value ? null : (string)rdr["email_address"]
                });
            };
            Customer current_customer = null;
            int current_customer_id = -99;
            rdr.NextResult();
            while (rdr.Read())
            {
                if (current_customer_id != (int)rdr["customerid"]) {
                    current_customer_id = (int)rdr["customerid"];
                    current_customer = database.customer.Find(x => x.id == current_customer_id);
                }
                current_customer.location.Add(new Location()
                {
                    id = (int)rdr["id"],
                    customerid = current_customer_id,
                    name = rdr["name"] == DBNull.Value ? null : (string)rdr["name"],
                    address = rdr["address"] == DBNull.Value ? null : (string)rdr["address"],
                    city = rdr["city"] == DBNull.Value ? null : (string)rdr["city"],
                    state = rdr["state"] == DBNull.Value ? null : (string)rdr["state"],
                    zip = rdr["zip"] == DBNull.Value ? null : (string)rdr["zip"],
                    phone = rdr["phone"] == DBNull.Value ? null : (string)rdr["phone"],
                    fax = rdr["fax"] == DBNull.Value ? null : (string)rdr["fax"],
                    email = rdr["email_address"] == DBNull.Value ? null : (string)rdr["email_address"],
                    serial_number = rdr["serial_number"] == DBNull.Value ? null : (string)rdr["serial_number"]
                });
            };
            rdr.Close();
            rdr = null;
        }

        public static void ReceiveDatabases(List<APIResponse_DatabaseConfig> databases) {
            foreach (var db in databases)
            {
                if (db.is_active)
                {
                    Initialize1Database(db.name);
                    Update1Database(db);
                }                
            }
        }

        private static void Initialize1Database(string database_name) {
            Common.sqlCommand_Initialize1Database.CommandText = string.Format(COMMAND_Initialize1Database, database_name);
            Common.sqlCommand_Initialize1Database.ExecuteNonQuery();
        }

        private static void Update1Database(APIResponse_DatabaseConfig db_config)
        {
            Common.sbUtils.Clear();
            Common.sbUtils.AppendFormat(COMMAND_UseDatabase, db_config.name);
            Common.sbUtils.Append(COMMAND_UpdateConfig_Part1);
            List<string> values = new List<string>();
            StringBuilder serial_numbers = new StringBuilder();
            int l = db_config.custloc.Count;
            values.Add(String.Format(COMMAND_UpdateConfig_Part2, -1, -1, db_config.is_active?1:0, "GETDATE()", 
                "GETDATE()", db_config.custloc.GroupBy(cl => cl.customerid).Count(), l>0?db_config.custloc.Max(cl => cl.customerid):0, 
                "GETDATE()", db_config.custloc.Count(), l>0?db_config.custloc.Max(cl => cl.locationid):0,
                l>0?(db_config.custloc.Min(cl => cl.last_sales_down)==null?"NULL":string.Format("'{0}'", db_config.custloc.Min(cl => cl.last_sales_down).Value.ToString("yyyy-MM-dd HH:mm:ss.fff"))):"NULL", l>0?(db_config.custloc.Sum(cl => cl.count_sales_down)==0?"NULL":db_config.custloc.Sum(cl => cl.count_sales_down).ToString()):"NULL", l>0?(db_config.custloc.Min(cl => cl.id_sales_down)!=null?db_config.custloc.Min(cl => cl.id_sales_down).ToString():"NULL"):"NULL"));
            foreach(APIResponse_CustomerLocationConfig cl in db_config.custloc)
            {
                values.Add(String.Format(COMMAND_UpdateConfig_Part2, cl.customerid, cl.locationid, cl.is_active ? 1 : 0, "GETDATE()",
                "NULL", "NULL", "NULL",
                "NULL", "NULL", "NULL",
                cl.last_sales_down == null ? "NULL" : string.Format("'{0}'", cl.last_sales_down.Value.ToString("yyyy-MM-dd HH:mm:ss.fff")), cl.count_sales_down == null ? "NULL" : cl.count_sales_down.ToString(), cl.id_sales_down == null ? "NULL" : cl.id_sales_down.ToString()));

                if (cl.serial_number != null)
                    serial_numbers.AppendFormat(COMMAND_UpdateConfig_Part4, cl.serial_number, cl.customerid, cl.locationid, cl.serial_number);
            }
            Common.sbUtils.Append(String.Join(",",values));
            values.Clear();
            values = null;
            Common.sbUtils.Append(COMMAND_UpdateConfig_Part3);
            Common.sbUtils.Append(serial_numbers.ToString());
            Common.sqlCommand_Update1Database.CommandText = Common.sbUtils.ToString();
            Common.sbUtils.Clear();
            serial_numbers.Clear();
            Common.sqlCommand_Update1Database.ExecuteNonQuery();
        }

        public static void GetItemsCatalog(Databases databases, ItemFull itemFull, string url, string msg) {
            Common.sbUtils.Clear();
            Common.sbSubCmd.Clear();
            bool safe2cont = false;
            
            List<string> values = new List<string>();
            if (itemFull.categories.Count > 0)
            {
                safe2cont = true;
                Common.sbSubCmd.Append(COMMAND_UpdateCategories_Part1);
                foreach (ItemCategory c in itemFull.categories)
                {
                    values.Add(string.Format(COMMAND_UpdateCategories_Part2, c.code.Replace("'","''"), c.name.Replace("'", "''"), c.description == null ? "NULL" : string.Format("'{0}'", c.description.Replace("'", "''")), c.pickorder));
                }
                Common.sbSubCmd.Append(String.Join(",", values));
                Common.sbSubCmd.Append(COMMAND_UpdateCategories_Part3);

                values.Clear();
            }
            if (itemFull.manufacturers.Count > 0)
            {
                safe2cont = true;
                Common.sbSubCmd.Append(COMMAND_UpdateManufacturers_Part1);
                foreach (ItemManufacturer m in itemFull.manufacturers)
                {
                    values.Add(string.Format(COMMAND_UpdateManufacturers_Part2, m.code.Replace("'", "''"), m.name.Replace("'", "''")));
                }
                Common.sbSubCmd.Append(String.Join(",", values));
                Common.sbSubCmd.Append(COMMAND_UpdateManufacturers_Part3);

                values.Clear();
            }

            if (itemFull.items.Count > 0)
            {
                safe2cont = true;
                Common.sbSubCmd.Append(COMMAND_UpdateItems_Part1);
                foreach (Item i in itemFull.items) {
                    values.Add(string.Format(COMMAND_UpdateItems_Part2,
                        i.uniqident.Replace("'", "''"),
                        i.upc == null ? "NULL" : string.Format("'{0}'", i.upc.Replace("'", "''")) ,
                        i.barcode == null ? "NULL" : string.Format("'{0}'", i.barcode.Replace("'", "''")),
                        i.name == null ? "NULL" : string.Format("'{0}'", i.name.Replace("'", "''")),
                        i.description == null ? "NULL" : string.Format("'{0}'", i.description.Replace("'", "''")),
                        i.mcode == null ? "NULL" : string.Format("'{0}'", i.mcode.Replace("'", "''")),
                        i.ccode == null ? "NULL" : string.Format("'{0}'", i.ccode.Replace("'", "''")),
                        i.count == null ? "NULL" : string.Format("{0}", i.count),
                        i.size == null ? "NULL" : string.Format("'{0}'", i.size),
                        i.unitcost == null ? "NULL" : string.Format("{0}", i.unitcost),
                        i.price == null ? "NULL" : string.Format("{0}", i.price),
                        i.is_taxable ? 1 : 0,
                        i.price_tax_excluded == null ? "NULL" : string.Format("{0}", i.price_tax_excluded),
                        i.tax_percent == null ? "NULL" : string.Format("{0}", i.tax_percent),
                        i.tax == null ? "NULL" : string.Format("{0}", i.tax),
                        i.crv == null ? "NULL" : string.Format("{0}", i.crv),
                        i.avgshelflife == null ? "NULL" : string.Format("'{0}'", i.avgshelflife.Replace("'", "''")),
                        i.pickorder == null ? "NULL" : string.Format("{0}", i.pickorder)));
                }
                Common.sbSubCmd.Append(String.Join(",", values));
                Common.sbSubCmd.Append(COMMAND_UpdateItems_Part3);

                values.Clear();
            }
            values = null;

            if (!safe2cont)
                return;

            safe2cont = false;
            SqlTransaction tran = Common.sqlConnection.BeginTransaction();
            Common.sqlCommand_LogAction.Transaction = tran;
            Common.sqlCommand_GetProducts.Transaction = tran;
            Common.sqlCommand_RefreshConfigGetItems.Transaction = tran;
            try
            {
                foreach (Database d in databases.dbs)
                {
                    if (d.is_active)
                    {
                        safe2cont = true;

                        Common.sbUtils.AppendFormat(COMMAND_UseDatabase, d.name);
                        Common.sbUtils.Append(Common.sbSubCmd.ToString());

                        LogAction(d, -1, -1, String.Empty, url, msg, Common.TYPE_SUCCESS, System.DBNull.Value);
                        RefreshConfig(RefreshConfig_Flag.GET_ITEMS, d.name, -1, -1, new object[] { itemFull.items.Count, System.DBNull.Value });
                    }
                }
                if (safe2cont)
                {
                    Common.sqlCommand_GetProducts.CommandText = Common.sbUtils.ToString();
                    Common.sqlCommand_GetProducts.ExecuteNonQuery();
                }
                tran.Commit();

                Common.sqlCommand_GetProducts.Transaction = null;
                Common.sqlCommand_LogAction.Transaction = null;
                Common.sqlCommand_RefreshConfigGetItems.Transaction = null;
                Common.sbUtils.Clear();
                Common.sbSubCmd.Clear();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Common.sqlCommand_GetProducts.Transaction = null;
                Common.sqlCommand_LogAction.Transaction = null;
                Common.sqlCommand_RefreshConfigGetItems.Transaction = null;
                throw;
            }
        }

        public static void GetItemsPerLocations(Databases databases, ItemFull itemFull, string url, string msg)
        {
            Common.sbUtils.Clear();
            Common.sbSubCmd.Clear();
            bool safe2cont = false;

            List<string> values = new List<string>();
            List<object[]> customer_location_ids = new List<object[]>();
            if (itemFull.items4databasecustomerlocation.Count > 0)
            {
                safe2cont = true;
                Common.sbSubCmd.Append(COMMAND_UpdateItemsPerLocation_Part1);
                foreach (Item4DatabaseCustomerLocation idcl in itemFull.items4databasecustomerlocation)
                {
                    foreach (Item4CustomerLocation i in idcl.items) {
                        if (customer_location_ids.FindIndex(x => (int)x[0] == i.customerid && (int)x[1] == i.locationid) == -1)
                        {
                            customer_location_ids.Add(new object[] { i.customerid, i.locationid, i.machineid });
                            databases.dbs.Find(x => x.name == idcl.db).customer.Find(x => x.id == i.customerid).location.Find(x => x.id == i.locationid).serial_number = i.machineid;
                        }
                        values.Add(string.Format(COMMAND_UpdateItemsPerLocation_Part2,
                            i.customerid, 
                            i.locationid,
                            string.Format("'{0}'", i.machineid.Replace("'", "''")),
                            string.Format("'{0}'", i.item_unique_identifier.Replace("'", "''")),
                            i.source_counter == null ? "NULL" : string.Format("{0}", i.source_counter),
                            i.counter == null ? "NULL" : string.Format("{0}", i.counter),
                            i.level == null ? "NULL" : string.Format("{0}", i.level),
                            i.par == null ? "NULL" : string.Format("{0}", i.par),
                            i.depletion_level == null ? "NULL" : string.Format("{0}", i.depletion_level),
                            i.quantity == null ? "NULL" : string.Format("{0}", i.quantity),
                            i.unitcost == null ? "NULL" : string.Format("{0}", i.unitcost),
                            i.price == null ? "NULL" : string.Format("{0}", i.price),
                            i.desired_price == null ? "NULL" : string.Format("{0}", i.desired_price),
                            i.is_taxable ? 1 : 0,
                            i.price_tax_excluded == null ? "NULL" : string.Format("{0}", i.price_tax_excluded),
                            i.tax_percent == null ? "NULL" : string.Format("{0}", i.tax_percent),
                            i.tax == null ? "NULL" : string.Format("{0}", i.tax),
                            i.crv == null ? "NULL" : string.Format("{0}", i.crv),
                            i.item_count == null ? "NULL" : string.Format("{0}", i.item_count)
                            ));
                    }
                }
                Common.sbSubCmd.Append(String.Join(",", values));
                Common.sbSubCmd.Append(COMMAND_UpdateItemsPerLocation_Part3);

                values.Clear();
            }
            values = null;

            if (!safe2cont)
            {
                customer_location_ids.Clear();
                customer_location_ids = null;
                return;
            }

            safe2cont = false;
            SqlTransaction tran = Common.sqlConnection.BeginTransaction();
            Common.sqlCommand_LogAction.Transaction = tran;
            Common.sqlCommand_GetItemsPerLocations.Transaction = tran;
            Common.sqlCommand_RefreshConfigGetItemsPerLocations.Transaction = tran;
            try
            {
                foreach (Item4DatabaseCustomerLocation idcl in itemFull.items4databasecustomerlocation)
                {
                    safe2cont = true;
                    Common.sbUtils.AppendFormat(COMMAND_UseDatabase, idcl.db);
                    Common.sbUtils.Append(Common.sbSubCmd.ToString());

                    LogAction(idcl.db, -1, -1, String.Empty, url, msg, Common.TYPE_SUCCESS, System.DBNull.Value);
                    RefreshConfig(RefreshConfig_Flag.GET_ITEMS_LOCATIONS, idcl.db, -1, -1, new object[] { idcl.items.Count, System.DBNull.Value });
                    foreach (object[] icl in customer_location_ids) {
                        LogAction(idcl.db, (int)icl[0], (int)icl[1], (string)icl[2], url, msg, Common.TYPE_SUCCESS, System.DBNull.Value);
                        RefreshConfig(RefreshConfig_Flag.GET_ITEMS_LOCATIONS, idcl.db, (int)icl[0], (int)icl[1], new object[] { idcl.items.FindAll(x => x.customerid == (int)icl[0] && x.locationid == (int)icl[1]).Count, System.DBNull.Value });
                    }

                }
                if (safe2cont)
                {
                    Common.sqlCommand_GetItemsPerLocations.CommandText = Common.sbUtils.ToString();
                    Common.sqlCommand_GetItemsPerLocations.ExecuteNonQuery();
                }
                tran.Commit();

                Common.sqlCommand_GetItemsPerLocations.Transaction = null;
                Common.sqlCommand_LogAction.Transaction = null;
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.Transaction = null;
                Common.sbUtils.Clear();
                Common.sbSubCmd.Clear();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Common.sqlCommand_GetItemsPerLocations.Transaction = null;
                Common.sqlCommand_LogAction.Transaction = null;
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.Transaction = null;
                throw;
            }
            finally
            {
                customer_location_ids.Clear();
                customer_location_ids = null;
            }
        }

        public static Sales SendSales(List<APIResponse_DatabaseConfig> dbconfigs, string database = null, int? customerid = null, int? locationid = null) {
            Sales ret = new Sales() { server=Common.SERVER_NAME, database=database, customerid = customerid, locationid=locationid };
            Common.sbUtils.Clear();
            if (database != null && dbconfigs.Find(x => x.is_active && x.name == database) != null) {
                string where = string.Empty;
                if (customerid != null) {
                    where = string.Format(" WHERE scd.[customerid] = {0} ", customerid);
                    if (locationid != null) {
                        where += string.Format(" AND scd.[locationid] = {0} ", locationid);
                    }
                }
                Common.sbUtils.Append(String.Format(COMMAND_SendSales, database, where));
            }
            else {
                foreach (APIResponse_DatabaseConfig d in dbconfigs) {
                    if (d.is_active) {
                        Common.sbUtils.Append(String.Format(COMMAND_SendSales, d.name, ""));
                    }
                }
            }
            Common.sqlCommand_SendSales.CommandText = Common.sbUtils.ToString();
            Common.sbUtils.Clear();

            System.Data.SqlClient.SqlDataReader rdr = Common.sqlCommand_SendSales.ExecuteReader();

            do {
                while (rdr.Read())
                {
                    ret.sales.Add(new Sale() {
                        d = int.Parse(rdr["databaseid"].ToString()),
                        c = (int)rdr["customerid"],
                        l = (int)rdr["locationid"],
                        u = (string)rdr["unique_identifier"],
                        i = (int)rdr["id"],
                        t = (DateTime)rdr["created_date_time"],
                        n = (int)rdr["count"],
                        p = float.Parse(rdr["price_tax_included"].ToString())
                    });
                }
            } while (rdr.NextResult());
            rdr.Close();
            rdr = null;
            
            return ret;
        }

        public static void LogAction(Database database, int customerid, int locationid, string machineid, object api, object description, object type, object message) {
            Common.sqlCommand_LogAction.CommandText = string.Format(COMMAND_LogAction, database.name);
            Common.sqlCommand_LogAction.Parameters["@customerid"].Value = customerid;
            Common.sqlCommand_LogAction.Parameters["@locationid"].Value = locationid;
            Common.sqlCommand_LogAction.Parameters["@machineid"].Value = machineid;
            Common.sqlCommand_LogAction.Parameters["@api"].Value = api;
            Common.sqlCommand_LogAction.Parameters["@description"].Value = description;
            Common.sqlCommand_LogAction.Parameters["@type"].Value = type;
            Common.sqlCommand_LogAction.Parameters["@message"].Value = message;
            Common.sqlCommand_LogAction.ExecuteNonQuery();
        }
        public static void LogAction(string database, int customerid, int locationid, string machineid, object api, object description, object type, object message)
        {
            Common.sqlCommand_LogAction.CommandText = string.Format(COMMAND_LogAction, database);
            Common.sqlCommand_LogAction.Parameters["@customerid"].Value = customerid;
            Common.sqlCommand_LogAction.Parameters["@locationid"].Value = locationid;
            Common.sqlCommand_LogAction.Parameters["@machineid"].Value = machineid;
            Common.sqlCommand_LogAction.Parameters["@api"].Value = api;
            Common.sqlCommand_LogAction.Parameters["@description"].Value = description;
            Common.sqlCommand_LogAction.Parameters["@type"].Value = type;
            Common.sqlCommand_LogAction.Parameters["@message"].Value = message;
            Common.sqlCommand_LogAction.ExecuteNonQuery();
        }
        public static void RefreshConfig(RefreshConfig_Flag flag, string database, int customerid, int locationid, object[] parameters) {
            if (flag == RefreshConfig_Flag.GET_ITEMS) {
                Common.sqlCommand_RefreshConfigGetItems.CommandText = string.Format(COMMAND_RefreshConfig_GetItems, database);
                Common.sqlCommand_RefreshConfigGetItems.Parameters["@customerid"].Value = customerid;
                Common.sqlCommand_RefreshConfigGetItems.Parameters["@locationid"].Value = locationid;
                Common.sqlCommand_RefreshConfigGetItems.Parameters["@count_item_up"].Value = parameters[0];
                Common.sqlCommand_RefreshConfigGetItems.Parameters["@id_item_up"].Value = parameters[1];
                Common.sqlCommand_RefreshConfigGetItems.ExecuteNonQuery();
            }
            else if (flag == RefreshConfig_Flag.GET_ITEMS_LOCATIONS) {
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.CommandText = string.Format(COMMAND_RefreshConfig_GetItemsPerLocations, database);
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters["@customerid"].Value = customerid;
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters["@locationid"].Value = locationid;
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters["@count_planogram_up"].Value = parameters[0];
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.Parameters["@id_planogram_up"].Value = parameters[1];
                Common.sqlCommand_RefreshConfigGetItemsPerLocations.ExecuteNonQuery();
            }
            else if (flag == RefreshConfig_Flag.GET_SALES) {
                Common.sqlCommand_RefreshConfigGetSales.CommandText = string.Format(COMMAND_RefreshConfig_GetSales, database);
                Common.sqlCommand_RefreshConfigGetSales.Parameters["@customerid"].Value = customerid;
                Common.sqlCommand_RefreshConfigGetSales.Parameters["@locationid"].Value = locationid;
                Common.sqlCommand_RefreshConfigGetSales.Parameters["@count_sales_up"].Value = parameters[0];
                Common.sqlCommand_RefreshConfigGetSales.Parameters["@id_sales_up"].Value = parameters[1];
                Common.sqlCommand_RefreshConfigGetSales.ExecuteNonQuery();
            }
            else { return; }
        }
    }
}
