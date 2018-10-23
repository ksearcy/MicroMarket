-- IMPORTANT
-- This script MUST be executed before SantaClarita code starts running
-- It sets up current Account Balance as balance from user's table, and sets up a starting point for comparison

-- This is just for me to recover from mess I made while testing, to restore state as it was the begining
/*
UPDATE n
SET n.[account_balance] = (SELECT TOP 1 abh.account_balance FROM [dbo].[accountbalancehistory] abh where abh.userpkid = n.pkid and abh.created_date_time <= '2017-11-08 16:27:14' order by abh.created_date_time DESC)
FROM [deORO_SantaClaritaTest].[dbo].[user] n;
*/

-- Creates "Period Closing Balance" record in [accounthistorybalance]
-- This is the first step to run when upgrading the system for SantaClarita bug

SET IDENTITY_INSERT [dbo].[accountbalancehistory] ON;

DECLARE @customerid INT, @locationid INT, @userpkid VARCHAR(255);
SET @customerid = NULL;
SET @locationid = NULL;
SET @userpkid = NULL;

DECLARE @DescriptionCleanup VARCHAR(255), @MaxId INT, @decimal_zero DECIMAL(18,2);
SET @DescriptionCleanup = 'Period Closing Balance';
SET @decimal_zero = CAST(0.00 AS DECIMAL(18,2));

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

BEGIN TRANSACTION;

SELECT @MaxId = MAX(id) FROM  [dbo].[accountbalancehistory] WITH (HOLDLOCK, TABLOCK) ;

INSERT INTO [dbo].[accountbalancehistory] WITH (HOLDLOCK, TABLOCK) 
([id],[pkid],[customerid],[locationid],[userpkid],[account_balance],[amount],[description],[created_date_time])
SELECT @MaxId + row_number() over (order by (select NULL))
, CAST(NEWID() AS VARCHAR(255))
, u.customerid
, u.locationid
, u.pkid
, u.account_balance
, @decimal_zero
, @DescriptionCleanup
, GETDATE()
FROM [dbo].[user] u WITH (HOLDLOCK, TABLOCK) 
WHERE u.customerid = COALESCE(@customerid, u.customerid)
AND u.locationid = COALESCE(@locationid, u.locationid)
AND u.pkid = COALESCE(@userpkid, u.pkid);

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

SET IDENTITY_INSERT [dbo].[accountbalancehistory] OFF;

/*
update [dbo].[accountbalancehistory]
set created_date_time = '2017-11-08 16:27:14'
where description = 'Period Closing Balance';
*/

--select * from dbo.[user]
--select top 2500 * from [dbo].[accountbalancehistory] order by created_date_time desc;