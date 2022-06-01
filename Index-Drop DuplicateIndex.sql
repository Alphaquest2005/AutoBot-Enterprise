SELECT
	'DROP INDEX [' + i.name + '] On ' +  s.Name + '.[' + t.Name + ']' ,
	s.Name + '.' + t.Name AS TableName	
	, i.name AS IndexName1	
	,DupliIDX.name AS IndexName2
	,c.name AS ColumnNae
FROM sys.tables AS t 
JOIN sys.indexes AS i
	ON t.object_id = i.object_id
JOIN sys.index_columns ic 
	ON ic.object_id = i.object_id 
		AND ic.index_id = i.index_id 
		AND ic.index_column_id = 1  
JOIN sys.columns AS c 
	ON c.object_id = ic.object_id 
		AND c.column_id = ic.column_id      
JOIN sys.schemas AS s 
	ON t.schema_id = s.schema_id
CROSS APPLY
(
	SELECT 
	   ind.index_id
	   ,ind.name
	FROM sys.indexes AS ind 
	JOIN sys.index_columns AS ico 
	   ON ico.object_id = ind.object_id
	   AND ico.index_id = ind.index_id
	   AND ico.index_column_id = 1  
	WHERE ind.object_id = i.object_id 
	   AND ind.index_id > i.index_id
	   AND ico.column_id = ic.column_id
) DupliIDX     
ORDER BY
    s.name,t.name,i.index_id
GO