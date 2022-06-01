
declare @qry nvarchar(max);
select @qry = (

    select  'IF EXISTS(SELECT * FROM sys.indexes WHERE name='''+ i.name +''' AND object_id = OBJECT_ID(''['+s.name+'].['+o.name+']''))      drop index ['+i.name+'] ON ['+s.name+'].['+o.name+'];  '
    from sys.indexes i 
        inner join sys.objects o on  i.object_id=o.object_id
        inner join sys.schemas s on o.schema_id = s.schema_id
    where o.type<>'S' and is_primary_key<>1 and index_id>0
    and s.name!='sys' and s.name!='sys' and is_unique_constraint=0  and i.Name like 'NCIDX_BB%'
for xml path(''));

exec sp_executesql @qry
