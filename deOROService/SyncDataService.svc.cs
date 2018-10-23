using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using deORODataAccess;

namespace deOROService
{
    public partial class SyncDataService : ISyncDataService
    {
        private ISyncDataService _syncDataServiceImplementation;

        public void UploadData(int customerId, int locationId, System.Data.DataSet data, bool usersSharedAcrossLocations = false)
        {
            System.Threading.Thread.Sleep(2000);

            foreach (DataTable dt in data.Tables)
            {
                switch (dt.TableName)
                {
                    case "User":
                        {

                            int locationIdShared;
                            if (usersSharedAcrossLocations)
                                locationIdShared = 0;
                            else locationIdShared = locationId;

                            UserRepository repo = new UserRepository(customerId, locationIdShared);
                            repo.Save(dt);
                            break;
                        }
                    case "DeleteMe":
                        {
                            var repo = new UserDeletedRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "AccountBalance":
                        {
                            var repo = new AccountBalanceHistoryRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "ShoppingCart":
                        {
                            var repo = new ShoppingCartRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "ShoppingCartDetail":
                        {
                            var repo = new ShoppingCartDetailRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "LocationItem":
                        {
                            var repo = new LocationItemRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "Payment":
                        {
                            var repo = new PaymentRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "CashCollection":
                        {
                            var repo = new CashCollectionRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "CashStatus":
                        {
                            var repo = new CashStatusRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "TransactionError":
                        {
                            var repo = new TransactionErrorRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "DeviceError":
                        {
                            var repo = new DeviceErrorRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "CashDispense":
                        {
                            var repo = new CashDispenseRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "CashCounter":
                        {
                            var repo = new CashCounterRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "LocationService":
                        {
                            var repo = new LocationServiceRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "CreditActivity":
                        {
                            var repo = new LocationCreditActivityRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                    case "EventLog":
                        {
                            var repo = new EventLogRepository(customerId, locationId);
                            repo.Save(dt);
                            break;
                        }
                }
            }
        }


        public DataSet GetScheduleAndItemsQuantityInfo(int customerId, int locationId)
        {
            System.Threading.Thread.Sleep(2000);

            DataSet ds = new DataSet();

            ScheduleDetailRepository repo1 = new ScheduleDetailRepository(customerId, locationId);
            int id = repo1.GetActiveScheduleId().Value;

            if (id != 0)
            {
                ScheduleDetailItemRepository repo2 = new ScheduleDetailItemRepository();
                var details = repo2.GetAllForPrint(id);
                ds.Tables.Add(details.ToDataTable());
            }

            return ds;
        }


        public void UpdateScheduledStatus(int customerId, int locationId, DataSet data)
        {
            System.Threading.Thread.Sleep(2000);

            LocationRepository repo1 = new LocationRepository(customerId, locationId);
            repo1.UpdateLastServicedDate();

            ScheduleDetailRepository repo2 = new ScheduleDetailRepository(customerId, locationId);
            int id = repo2.UpdateActiveScheduleStatus();

            if (id != 0)
            {
                ScheduleDetailItemRepository repo3 = new ScheduleDetailItemRepository(customerId, locationId);
                repo3.UpdateOverUnder(id, data);
            }

        }

        public bool TestService(int customerId, int locationId)
        {
            return true;
        }

        public int AdjustUnevenBalances(int customerId, int locationId, bool usersSharedAccrossLocations = false)
        {
            int ret = 0;
            string connString = System.Configuration.ConfigurationManager.ConnectionStrings["deORO_MasterEntities"].ConnectionString;
            //< add name = "deORO_MasterEntities" connectionString = "metadata=res://*/deOROModel.csdl|res://*/deOROModel.ssdl|res://*/deOROModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=.;initial catalog=deORO_Web;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;"
            int st = connString.IndexOf("\"", 0) + 1;
            int ed = connString.IndexOf("\"", st + 1);
            connString.Substring(st, ed-st);
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString.Substring(st, ed - st).Replace("App=EntityFramework",""))) {
                conn.Open();
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlCommand_AdjustUnevenBalances.Replace("{customerId}", customerId.ToString()).Replace("{locationId}", locationId.ToString()).Replace("{usersSharedAccrossLocations}", usersSharedAccrossLocations?"1":"0");
                cmd.CommandTimeout = 60;

                ret = (int)cmd.ExecuteScalar();
                conn.Close();
            }
            return ret;
        }
        
        public const string SqlCommand_AdjustUnevenBalances = @"
DECLARE @UserSharedAcrosssLocations BIT, 
	@CustomerID INT, 
	@LocationID INT,
	@DaysAgoToSync INT,
	@DownloadRequired INT,
	@MinutesToCheckFor INT,
	@DescriptionCleanup VARCHAR(50);
SET @UserSharedAcrosssLocations = {usersSharedAccrossLocations};
SET @CustomerID = {customerId};
SET @LocationID = {locationId};
SET @DaysAgoToSync = 365;
SET @DownloadRequired = 0;
SET @MinutesToCheckFor = 20;
SET @DescriptionCleanup = 'Period Closing Balance';

DECLARE @RefillItem INT;
SELECT @RefillItem = [id] FROM [dbo].[item] WHERE UPPER([barcode])='ACCOUNT_REFILL_BARCODE';
--SELECT @RefillItem;
--SELECT DISTINCT [description] FROM accountbalancehistory;

DECLARE @missing_shoppingcart_payments TABLE(
	shoppingcartpkid VARCHAR(255) NOT NULL,
    created_date_time DATETIME NOT NULL,
    payment DECIMAL(18,2) NOT NULL
);
---- Select all of the users having uneven balance in user and accountbalancehistory table
DECLARE @pkid VARCHAR(255), @OLD_account_balance DECIMAL(18,2), @NEW_account_balance DECIMAL(18,2); /*, @username VARCHAR(255)
	, @OLD_account_balance DECIMAL(18,2), @OLD_lastaccountbalancechangedamount DECIMAL(18, 2), @OLD_lastaccountbalancechangeddate DATETIME
	, @NEW_account_balance DECIMAL(18,2), @NEW_lastaccountbalancechangedamount DECIMAL(18, 2), @NEW_lastaccountbalancechangeddate DATETIME; */

DECLARE uneven_balances_cursor CURSOR FAST_FORWARD FOR
SELECT u.pkid
	/*, u.username
	, u.account_balance as OLD_account_balance, u.lastaccountbalancechangedamount as OLD_lastaccountbalancechangedamount, u.lastaccountbalancechangeddate as OLD_lastaccountbalancechangeddate 
	, n.NEW_account_balance, n.last_created_date_time */
FROM [dbo].[user] u 
INNER JOIN (
SELECT s.userpkid, s.sum_amount, sb.start_account_balance, s.sum_amount + sb.start_account_balance as NEW_account_balance,
s.last_created_date_time
FROM 
	(SELECT userpkid, SUM(amount) as sum_amount, MAX(created_date_time) AS last_created_date_time
	FROM [dbo].[accountbalancehistory] abh
	WHERE customerid = @customerid
	AND ((@UserSharedAcrosssLocations = 1) OR  (@UserSharedAcrosssLocations = 0 AND locationid = @LocationID))
	AND abh.[created_date_time] > COALESCE((SELECT TOP 1 abhi.[created_date_time] 
	FROM [dbo].[accountbalancehistory] abhi
	WHERE abhi.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abhi.[locationid] = @LocationID))
	AND abhi.[description] = @DescriptionCleanup
	ORDER BY abhi.[created_date_time] DESC
	),'2000-01-01') 
	GROUP BY abh.userpkid) AS s
	INNER JOIN (
	SELECT userpkid, account_balance-amount as start_account_balance
	FROM (
	SELECT userpkid, account_balance, amount, ROW_NUMBER() OVER(PARTITION BY userpkid ORDER BY created_date_time ASC) AS roworder
	FROM [dbo].[accountbalancehistory] abh
	WHERE customerid = @customerid
	AND ((@UserSharedAcrosssLocations = 1) OR  (@UserSharedAcrosssLocations = 0 AND locationid = @LocationID))
	AND abh.[created_date_time] >=  COALESCE((SELECT TOP 1 abhi.[created_date_time] 
	FROM [dbo].[accountbalancehistory] abhi
	WHERE abhi.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abhi.[locationid] = @LocationID))
	AND abhi.[description] = @DescriptionCleanup
	ORDER BY abhi.[created_date_time] DESC
	),'2000-01-01') 
	) temp
	WHERE roworder = 1
) AS sb ON sb.userpkid=s.userpkid) as n ON n.userpkid = u.pkid
WHERE u.customerid = @CustomerID AND ((@UserSharedAcrosssLocations = 1) OR  (@UserSharedAcrosssLocations = 0 AND locationid = @LocationID))
AND COALESCE(n.NEW_account_balance,0) != COALESCE(u.account_balance,0)
;

OPEN uneven_balances_cursor;

FETCH NEXT FROM uneven_balances_cursor   
INTO @pkid;

WHILE @@FETCH_STATUS = 0  
BEGIN
	--SELECT @pkid;

	SET @DownloadRequired = @DownloadRequired + 1;

	--INSERT INTO [dbo].[accountbalancehistory] ([id],[pkid],[customerid],[userpkid],[account_balance],[amount],[description],[created_date_time]) 
	/*SELECT CAST(NEWID() AS VARCHAR(255)),@CustomerID,CASE @UserSharedAcrosssLocations WHEN 1 THEN 0 ELSE @LocationID END, 
	NULL, shoppingcart_4pkid.shoppingcart_amount, CASE WHEN shoppingcart_4pkid.shoppingcart_amount > 0 THEN 'Purchase' ELSE '' END,
	*/
	
	--select distinct source from payment;
	
	-- Get last closing balance date
	DECLARE @last_period_closing_balance DATETIME;
	SELECT TOP 1 @last_period_closing_balance = abh.[created_date_time] 
	FROM [dbo].[accountbalancehistory] abh
	WHERE abh.[userpkid]=@pkid
	AND abh.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abh.[locationid] = @LocationID))
	AND abh.[description] = @DescriptionCleanup;
	IF @last_period_closing_balance IS NULL SET @last_period_closing_balance = '2000-01-01';
	
	--print @last_period_closing_balance
	
	-- 1. Make sure we have all of the shopping cart items listed in account balance history for given user for period of @DaysAgoToSync back
	-- These records are created without being written to accounntbalancehistory, and sometimes they get loaded before the accountbalancehisotry is uploaded
	-- Usually these are caught up within next upload sync
	-- So here I am not gonna add records in accountbalancehistory but I will add this missing payments / refills at the end to user account balance
	DELETE FROM @missing_shoppingcart_payments;

	; WITH shoppingcart_4pkid AS 
	(SELECT sc.pkid, sc.[created_date_time], (SELECT SUM((CASE scd.itemid WHEN @RefillItem THEN scd.[price_tax_included] ELSE -scd.[price_tax_included] END)) FROM  [dbo].[shoppingcartdetail] scd WHERE scd.[shoppingcartpkid] = sc.pkid) AS shoppingcart_amount -- sc.[created_date_time], p.[source]
	FROM [dbo].[shoppingcart] sc
	INNER JOIN [dbo].[payment] p ON p.[shoppingcartpkid] = sc.[pkid]
	WHERE sc.[userpkid]=@pkid
	AND sc.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND sc.[locationid] = @LocationID))
	AND sc.[created_date_time] >= DATEADD(D, -@DaysAgoToSync, GETDATE())
	AND sc.[created_date_time] >= @last_period_closing_balance
	AND p.[source] IN ('PurchaseComplete', 'CreditCardRefill', 'Reward', 'CoinRefill', 'BillRefill', 'MyAccountPay')
	GROUP BY sc.pkid, sc.[created_date_time])
	INSERT INTO @missing_shoppingcart_payments
	SELECT * 
	FROM shoppingcart_4pkid
	WHERE NOT EXISTS (SELECT * FROM [dbo].[accountbalancehistory] abh 
					WHERE abh.[amount] = shoppingcart_4pkid.shoppingcart_amount 
					AND abh.[userpkid] = @pkid
					AND abh.[customerid] = @CustomerID 
					AND ((@UserSharedAcrosssLocations = 1) OR  (@UserSharedAcrosssLocations = 0 AND abh.[locationid] = @LocationID))
					AND abh.[created_date_time] >= shoppingcart_4pkid.created_date_time
					AND abh.[created_date_time] BETWEEN DATEADD(mi,-@MinutesToCheckFor,shoppingcart_4pkid.created_date_time) AND DATEADD(mi,@MinutesToCheckFor,shoppingcart_4pkid.created_date_time))
	ORDER BY shoppingcart_4pkid.[created_date_time] ASC;
	
	-- 2. Recalculate all account_balance for given user for period of @DaysAgoToSync back
	DECLARE @start DECIMAL(18,2), @amount DECIMAL(18,2);
	SELECT TOP 1 @start = abh.account_balance - abh.amount
	FROM [dbo].[accountbalancehistory] abh
	WHERE abh.[userpkid]=@pkid
	AND abh.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abh.[locationid] = @LocationID))
	AND abh.[created_date_time] >= DATEADD(D, -@DaysAgoToSync, GETDATE())
	AND abh.[created_date_time] >= @last_period_closing_balance
	ORDER BY abh.[created_date_time] ASC;
	
	UPDATE abh
	SET abh.account_balance =
	--SELECT abh.pkid, abh.created_date_time, abh.account_balance, abh.amount, 
	(SELECT SUM(abh2.amount) FROM [dbo].[accountbalancehistory] abh2 
	WHERE abh2.[userpkid]=@pkid
	AND abh2.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abh2.[locationid] = @LocationID))
	AND abh2.[created_date_time] <= abh.[created_date_time] 
	AND abh2.[created_date_time] > @last_period_closing_balance) + @start
	FROM [dbo].[accountbalancehistory] abh
	WHERE abh.[userpkid]=@pkid
	AND abh.[customerid] = @CustomerID
 	AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abh.[locationid] = @LocationID))
	AND abh.[created_date_time] >= DATEADD(D, -@DaysAgoToSync, GETDATE())
	AND abh.[created_date_time] > @last_period_closing_balance
	;--ORDER BY abh.[created_date_time] ASC;

	-- 3. Increase sync_vector so we know this is the most recent update for kiosks to sync with
	SELECT @OLD_account_balance = [account_balance] FROM [dbo].[user] WHERE [pkid] = @pkid;
	UPDATE [dbo].[user] 
	SET [sync_vector] = COALESCE([sync_vector],0)+1
	, [last_updated_on] = GETDATE()
	, [account_balance] = (SELECT TOP 1 [account_balance] FROM [dbo].[accountbalancehistory] abh
							WHERE abh.[userpkid]=@pkid
							AND abh.[customerid] = @CustomerID
 							AND ((@UserSharedAcrosssLocations = 1) OR (@UserSharedAcrosssLocations = 0 AND abh.[locationid] = @LocationID))
							ORDER BY [created_date_time] DESC) 
						+
						COALESCE((SELECT SUM(payment) FROM @missing_shoppingcart_payments),0) -- Adding missing payments and refills, which are processed adn caught in payment but not in accountbalancehistory, this happens sometimes, and it usually synced with next upload
	--, [last_updated_by_id] = 
	WHERE [pkid]=@pkid;
	SELECT @NEW_account_balance = [account_balance] FROM [dbo].[user] WHERE [pkid] = @pkid;

	IF (COALESCE(@OLD_account_balance,0) != COALESCE(@NEW_account_balance,0))
	BEGIN
		INSERT INTO [dbo].[synclog] (
		[customerid]
		,[locationid]
		,[description]
		,[type]
		,[message]
		,[created_date_time])
		VALUES (
				@CustomerID
				, @LocationID
				, 'Account Balance for ' + @pkid + ' corrected - Old Value ' + CAST(COALESCE(@OLD_account_balance,0) as VARCHAR(50)) + ' - New Value ' + CAST(COALESCE(@NEW_account_balance,0) as VARCHAR(50))
				, 'Success'
				, 'Account Balance Corrected'
				, GETDATE());
	END;

	-- Add to event log row, before and after
	FETCH NEXT FROM uneven_balances_cursor   
	INTO @pkid;
END;

CLOSE uneven_balances_cursor;
DEALLOCATE uneven_balances_cursor;
--RETURN @DownloadRequired;
SELECT @DownloadRequired;
";
    }
}
