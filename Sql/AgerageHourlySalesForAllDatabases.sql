USE [master];
DECLARE @cmd VARCHAR(MAX), @cmd1 VARCHAR(MAX), @db SYSNAME, @is_first BIT;
SET @cmd1 = '

SELECT ''DBNAME'', [Day], [Hour], [DayN], AVG(Totals) AS [Avg]
FROM
  (
        SELECT 
          [Day]  = DATENAME(WEEKDAY, [created_date_time]),
          [DayN] = DATEPART(WEEKDAY, [created_date_time]),
          [Hour] = DATEPART(HOUR, [created_date_time]),
		  [DayMn] = DATEPART(DAY, [created_date_time]),
          Totals = COUNT(*)
        FROM [DBNAME].[dbo].[shoppingcartdetail]
            WHERE [created_date_time] >= DATEADD(d,-45,GETDATE() )
        GROUP BY 
		  DATEPART(DAY, [created_date_time]),
          DATENAME(WEEKDAY, [created_date_time]),
          DATEPART(WEEKDAY, [created_date_time]),
          DATEPART(HOUR, [created_date_time])
) AS q
GROUP BY [Day], [Hour], [DayN]


';
SET @cmd = CAST('' AS VARCHAR(MAX));
SET @is_first = 1;

DECLARE dbs CURSOR FAST_FORWARD FOR
SELECT [name] FROM [master].[dbo].[sysdatabases]
WHERE [name] LIKE 'deORO_%' AND [name] != 'deORO_items' AND [name] != 'deORO_Master_items' AND [name] != 'deORO_Vending';
	
OPEN dbs;
FETCH NEXT FROM dbs INTO @db;
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @cmd = @cmd + CAST(CASE WHEN @is_first = 1 THEN ' ' ELSE 'UNION ' END + REPLACE(@cmd1, 'DBNAME', @db) AS VARCHAR(MAX))
	SET @is_first = 0;
	FETCH NEXT FROM dbs INTO @db;
END
--SELECT @cmd

CLOSE dbs;
DEALLOCATE dbs;

EXEC(@cmd + ' ORDER BY 1,2,3;');
--smart box, android studio sdk 
--smart glass company,
