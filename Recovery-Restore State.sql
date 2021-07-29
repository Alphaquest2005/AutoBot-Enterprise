declare @dbname varchar(50)
set @dbname = '[Rouge-DiscoveryDB]'

--RESTORE DATABASE @dbname WITH RECOVERY

USE master;
GO

RESTORE DATABASE [Rouge-DiscoveryDB] WITH RECOVERY

ALTER DATABASE [Rouge-DiscoveryDB]
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;