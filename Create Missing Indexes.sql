SELECT
UPPER(DB_Name()) as 'DATABASE',
Object_Name(SQLOPS_MsgIdxDetails.object_id) as 'OBJECT NAME',
Schema_Name(SQLOPS_SysObj.schema_id) as 'SCHEMA NAME',
'CREATE INDEX ['+/*+DB_Name()+*/'SQLOPS_'+
Object_Name(SQLOPS_MsgIdxDetails.object_id)+'_' +
CONVERT (varchar, SQLOPS_MsgIdxGrp.index_group_handle) + '_' +
CONVERT (varchar, SQLOPS_MsgIdxDetails.index_handle) + '] ON ' +
SQLOPS_MsgIdxDetails.statement + '
(' + ISNULL (SQLOPS_MsgIdxDetails.equality_columns,'')
+ CASE WHEN SQLOPS_MsgIdxDetails.equality_columns IS NOT NULL
AND SQLOPS_MsgIdxDetails.inequality_columns IS NOT NULL
THEN ',' ELSE '' END + ISNULL (SQLOPS_MsgIdxDetails.inequality_columns, '')
+ ')'
+ ISNULL (' INCLUDE (' + SQLOPS_MsgIdxDetails.included_columns + ');', '')
AS 'CREATE INDEX COMMAND', --Online Index will work only if you have Enterprise edition
Cast(round(SQLOPS_MsgIdxGrpStats.avg_total_user_cost,2) as varchar)+'%'
as 'ESTIMATED CURRENT COST',
Cast(SQLOPS_MsgIdxGrpStats.avg_user_impact as varchar)+'%' as 'CAN BE IMPROVED',
SQLOPS_MsgIdxGrpStats.last_user_seek as 'LAST USER SEEK',
'SCRIPT PROVIDED BY HTTPS://SQLOPS.COM' as 'CREDITS'
FROM sys.dm_db_missing_index_groups AS SQLOPS_MsgIdxGrp
INNER JOIN sys.dm_db_missing_index_group_stats AS SQLOPS_MsgIdxGrpStats
ON SQLOPS_MsgIdxGrpStats.group_handle = SQLOPS_MsgIdxGrp.index_group_handle
INNER JOIN sys.dm_db_missing_index_details AS SQLOPS_MsgIdxDetails
ON SQLOPS_MsgIdxGrp.index_handle = SQLOPS_MsgIdxDetails.index_handle
INNER JOIN sys.objects as SQLOPS_SysObj
ON SQLOPS_MsgIdxDetails.object_id = SQLOPS_SysObj.object_id
--where SQLOPS_MsgIdxDetails.statement like '%' + DB_Name() + '%'
ORDER BY 4 desc
