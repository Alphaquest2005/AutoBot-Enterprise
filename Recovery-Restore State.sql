declare @dbname varchar(50)
set @dbname = '[ColumbianEmeralds-DiscoveryDB]'

--RESTORE DATABASE @dbname WITH RECOVERY

USE master;
GO

RESTORE DATABASE [ColumbianEmeralds-DiscoveryDB] WITH RECOVERY

ALTER DATABASE [ColumbianEmeralds-DiscoveryDB]
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;

ALTER DATABASE [ColumbianEmeralds-DiscoveryDB]
set multi_user WITH ROLLBACK IMMEDIATE;