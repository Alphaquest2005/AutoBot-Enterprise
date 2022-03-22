

SELECT objtype AS [CacheType],
    COUNT_BIG(*) AS [Total Plans],
    SUM(CAST(size_in_bytes AS DECIMAL(18, 2))) / 1024 / 1024 AS [Total MBs],
    AVG(usecounts) AS [Avg Use Count],
    SUM(CAST((CASE WHEN usecounts = 1 THEN size_in_bytes
        ELSE 0
        END) AS DECIMAL(18, 2))) / 1024 / 1024 AS [Total MBs – USE Count 1],
    SUM(CASE WHEN usecounts = 1 THEN 1
        ELSE 0
        END) AS [Total Plans – USE Count 1]
		into #plans
FROM sys.dm_exec_cached_plans
GROUP BY objtype
ORDER BY [Total MBs – USE Count 1] DESC

declare @CN varchar (200)

set @CN = (select top 1 CacheType from #plans where CacheType = 'Adhoc')

IF @CN = 'Adhoc'
  BEGIN 
      EXEC sys.sp_configure N'show advanced options', N'1'  RECONFIGURE WITH OVERRIDE
      EXEC sys.sp_configure N'optimize for ad hoc workloads', N'1'
	  EXEC sys.sp_configure N'show advanced options', N'0'  RECONFIGURE WITH OVERRIDE
  END 


 drop table #plans