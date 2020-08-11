SELECT DISTINCT
       o.name AS Object_Name, 
       o.type_desc, m.definition
  FROM sys.sql_modules m
       INNER JOIN
       sys.objects o
         ON m.object_id = o.object_id
where m.definition like '%''9074''%'