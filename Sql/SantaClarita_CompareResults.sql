-- Comparison SatnaClarita / SatnaClaritaTest
DECLARE @start_from DATETIME = '2017-11-08';
DECLARE @pkid VARCHAR(255);

SELECT '-------------------------'
SET @pkid = '72b1a875-ed19-437e-901c-3c1cfa4ea87c';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '58602ecd-0382-4c62-b204-33fc58b822ad';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '57e29707-9d10-4820-818a-f50839abc5a2';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '62515ffd-f1a9-44c0-926d-a2315cbdf8c0';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;


SELECT '-------------------------'
SET @pkid = 'd79dfb20-a54d-4013-bb8d-5d0881e8ed71';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = 'ee12926c-c52d-4a33-862d-a6c837dfd6f7';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '000409d2-466a-4942-ad55-ae5d34198d9f';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = 'd85604ee-06f4-4da5-92d4-635fab2e864f';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '09e52ebf-10d2-4106-a5ca-5d85379f5e26';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = 'dfbd674c-182d-420c-ab1d-d4c30b8a12ed';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '89760cfb-f1f0-4f9f-9e10-cf599a382376';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;

SELECT '-------------------------'
SET @pkid = '797dd300-2cd1-4583-bfb1-7cc4a5ad491f';
SELECT u1.account_balance AS 'deORO_SantaClaritaTest Balance',u2.account_balance AS 'deORO_SantaClarita Balance'
FROM [deORO_SantaClaritaTest].[dbo].[user] u1
INNER JOIN [deORO_SantaClarita].[dbo].[user] u2 on u1.pkid = u2.pkid WHERE u1.pkid = @pkid;
SELECT 'deORO_SantaClarita', * FROM [deORO_SantaClarita].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
UNION ALL
SELECT 'deORO_SantaClaritaTest', * FROM [deORO_SantaClaritaTest].[dbo].[accountbalancehistory] WHERE userpkid = @pkid AND created_date_time > @start_from
ORDER BY 1, created_date_time ASC;


