
 DECLARE @MinimumPageCount int  
 SET @MinimumPageCount = 500

SELECT top 1 Databases.name AS [Database],  
 object_name(Indexes.object_id) AS [Table],  
 Indexes.name AS [Index],  
 Indexes.is_unique_constraint,
 UsageStats.user_scans,
  UsageStats.user_seeks,
 PhysicalStats.page_count as [Page_Count],  
 CONVERT(decimal(18,2), PhysicalStats.page_count * 8 / 1024.0) AS [Total Size (MB)],  
 CONVERT(decimal(18,2), PhysicalStats.avg_fragmentation_in_percent) AS [Frag %],  
 ParititionStats.row_count AS [Row Count],  
 CONVERT(decimal(18,2), (PhysicalStats.page_count * 8.0 * 1024)  
 / ParititionStats.row_count) AS [Index Size/Row (Bytes)]  
 into #mike 
 FROM sys.dm_db_index_usage_stats UsageStats  
 INNER JOIN sys.indexes Indexes  
 ON Indexes.index_id = UsageStats.index_id  
 AND Indexes.object_id = UsageStats.object_id  
 INNER JOIN SYS.databases Databases  
 ON Databases.database_id = UsageStats.database_id  
 INNER JOIN sys.dm_db_index_physical_stats (DB_ID(),NULL,NULL,NULL,NULL)  
 AS PhysicalStats  
 ON PhysicalStats.index_id = UsageStats.Index_id  
 and PhysicalStats.object_id = UsageStats.object_id  
 INNER JOIN SYS.dm_db_partition_stats ParititionStats  
 ON ParititionStats.index_id = UsageStats.index_id  
 and ParititionStats.object_id = UsageStats.object_id  
 WHERE UsageStats.user_scans = 0  
 AND UsageStats.user_seeks = 0  
 AND PhysicalStats.page_count > @MinimumPageCount  
 AND Indexes.type_desc != 'CLUSTERED'
 and Indexes.name not like 'PK%'
 and INdexes.is_unique <> 1
 ORDER BY [Page_Count] DESC
 

declare @sql_command varchar(max);
set @sql_command = '';

SELECT @sql_command += 'DROP INDEX ' + [index] + '' + char(13)+ 'ON ' + [Table] + CHAR(13) FROM #mike

print @sql_command
exec (@sql_command);

drop table #mike