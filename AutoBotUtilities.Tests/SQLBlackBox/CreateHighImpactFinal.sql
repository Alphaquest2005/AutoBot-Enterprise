create proc uspCreateIndexes
with encryption 
as
BEGIN 
-- comment: sp_password
IF OBJECT_ID('tempdb.dbo.#max', 'U') IS NOT NULL
DROP TABLE #max;
SELECT top 1
migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) AS improvement_measure,
'CREATE INDEX [NCIDX_BB' + '_' + CONVERT (varchar, mid.index_handle)
+ '_' + LEFT (PARSENAME(mid.statement, 1), 32) + ']'
+ ' ON ' + mid.statement
+ ' (' + ISNULL (mid.equality_columns,'')
+ CASE WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ',' ELSE '' END
+ ISNULL (mid.inequality_columns, '')
+ ')'
+ ISNULL (' INCLUDE (' + mid.included_columns + ')', '') AS create_index_statement,
migs.*, mid.database_id, mid.[object_id]
into #max
FROM sys.dm_db_missing_index_groups mig
INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
and  migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10000
ORDER BY migs.avg_total_user_cost * migs.avg_user_impact * (migs.user_seeks + migs.user_scans) DESC

declare @sql_command varchar(max);
set @sql_command = '';
SELECT @sql_command += [create_index_statement] + '' + char(13)FROM #max where create_index_statement <> '%tempdb%'
print @sql_command
exec (@sql_command);
drop table #max
END