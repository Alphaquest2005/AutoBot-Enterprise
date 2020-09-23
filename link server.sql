USE [master]
GO

/****** Object:  LinkedServer [IWWGNDSRV]    Script Date: 9/22/2020 12:16:21 PM ******/
EXEC master.dbo.sp_addlinkedserver @server = N'IWWGNDSRV', @srvproduct=N'', @provider=N'MSOLEDBSQL', @datasrc=N'joseph-pc\SQLDEVELOPER2017', @provstr=N'initial catalog=AutoBot-EnterpriseDB;Integrated Security=true;Connect Timeout=30', @catalog=N'IWW'
 /* For security reasons the linked server remote logins password is changed with ######## */
EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname=N'IWWGNDSRV',@useself=N'False',@locallogin=NULL,@rmtuser=N'sa',@rmtpassword='pa$$word'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'collation compatible', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'data access', @optvalue=N'true'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'dist', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'pub', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'rpc', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'rpc out', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'sub', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'connect timeout', @optvalue=N'0'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'collation name', @optvalue=null
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'lazy schema validation', @optvalue=N'false'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'query timeout', @optvalue=N'0'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'use remote collation', @optvalue=N'true'
GO

EXEC master.dbo.sp_serveroption @server=N'IWWGNDSRV', @optname=N'remote proc transaction promotion', @optvalue=N'true'
GO


