DECLARE @sql NVARCHAR(MAX) = N'';

SELECT /*@sql +=*/ N'DROP INDEX ' 
    + QUOTENAME(SCHEMA_NAME(o.[schema_id]))
    + '.' + QUOTENAME(o.name) 
    + '.' + QUOTENAME(i.name) + ';'
    FROM sys.indexes AS i
    INNER JOIN sys.tables AS o
    ON i.[object_id] = o.[object_id]
WHERE i.is_primary_key = 0
AND i.index_id <> 0
AND o.is_ms_shipped = 0 ;--and o.name = 'xcuda_Item'

PRINT @sql;
 EXEC sp_executesql @sql;


 SELECT 
   'DROP INDEX ' + name + ' ON ' + 
   OBJECT_SCHEMA_NAME(object_id) + '.' + OBJECT_NAME(object_Id) 
FROM sys.indexes
WHERE is_primary_key = 0
AND name IS NOT NULL
AND OBJECT_SCHEMA_NAME(object_id) <> 'sys'

