DECLARE @cutoff DATE;
SET @cutoff = DATEADD(MONTH, -3, GETDATE());
exec msdb.dbo.sp_delete_backuphistory @oldest_date = @cutoff