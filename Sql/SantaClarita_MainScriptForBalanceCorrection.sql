-- MAIN SCRIPT TO RUN IN C#
USE [deORO_SantaClaritaTest];
-- This is for testing ONLY 
-- Reload clean state from 2017-11-08 16h after Kevin's import
/*
DROP TABLE [deORO_SantaClaritaTest].[dbo].[accountbalancehistory];
SELECT * 
INTO [deORO_SantaClaritaTest].[dbo].[accountbalancehistory]
FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory_original];

;
*/
-- Load current records from live deORO_SantaClarita database
/*
INSERT INTO [deORO_SantaClaritaTest].[dbo].[user]
SELECT * FROM [deORO_SantaClarita].[dbo].[user] d WHERE NOT EXISTS(SELECT * FROM [deORO_SantaClaritaTest].[dbo].[user] WHERE [deORO_SantaClaritaTest].[dbo].[user].id=d.[id]);

UPDATE n
SET n.[account_balance] = d.[account_balance]
, n.[lastaccountbalancechangedamount] = d.[lastaccountbalancechangedamount]
, n.[lastaccountbalancechangeddate] = d.[lastaccountbalancechangeddate]
, n.[lastaccountbalancechangeddescription] = d.[lastaccountbalancechangeddescription]
, n.[sync_vector] = d.[sync_vector]
FROM [deORO_SantaClaritaTest].[dbo].[user] n
INNER JOIN  [deORO_SantaClarita].[dbo].[user] d ON d.[pkid] = n.[pkid];

INSERT INTO [deORO_SantaClaritaTest].[dbo].[accountbalancehistory]
SELECT * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] d WHERE NOT EXISTS(SELECT * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE [deORO_SantaClaritaTest].[dbo].[accountbalancehistory].id=d.[id]);

INSERT INTO [deORO_SantaClaritaTest].[dbo].[shoppingcart]
SELECT * FROM [deORO_SantaClarita].[dbo].[shoppingcart] d WHERE NOT EXISTS(SELECT * FROM [deORO_SantaClaritaTest].[dbo].[shoppingcart] WHERE [deORO_SantaClaritaTest].[dbo].[shoppingcart].id=d.[id]);

INSERT INTO [deORO_SantaClaritaTest].[dbo].[shoppingcartdetail]
SELECT * FROM [deORO_SantaClarita].[dbo].[shoppingcartdetail] d WHERE NOT EXISTS(SELECT * FROM [deORO_SantaClaritaTest].[dbo].[shoppingcartdetail] WHERE [deORO_SantaClaritaTest].[dbo].[shoppingcartdetail].id=d.[id]);

INSERT INTO [deORO_SantaClaritaTest].[dbo].[payment]
SELECT * FROM [deORO_SantaClarita].[dbo].[payment] d WHERE NOT EXISTS(SELECT * FROM [deORO_SantaClaritaTest].[dbo].[payment] WHERE [deORO_SantaClaritaTest].[dbo].[payment].id=d.[id]);

*/

--USE [deORO_SantaClarita];
--USE [deORO_Arca];

USE [deORO_SantaClaritaTest];
DECLARE @UserSharedAcrosssLocations BIT, 
	@CustomerID INT, 
	@LocationID INT,
	@DaysAgoToSync INT,
	@DownloadRequired INT,
	@MinutesToCheckFor INT,
	@DescriptionCleanup VARCHAR(50);
SET @UserSharedAcrosssLocations = 1;
SET @CustomerID = 1;
SET @LocationID = 0;
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
	SELECT @pkid;

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
		VALUES (@CustomerID
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
