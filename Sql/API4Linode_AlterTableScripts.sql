 ALTER TABLE [dbo].[location] ADD [serial_number] VARCHAR(75) NULL;
 GO
 ALTER TABLE [dbo].[item] ADD [unique_identifier] VARCHAR(75) NULL;
 GO
 UPDATE [dbo].[item] SET [unique_identifier] = [barcode];
 GO

