IF OBJECT_ID('tempdb.dbo.#mike', 'U') IS NOT NULL
  DROP TABLE #mike;
with indexcols as
(
select object_id as id, index_id as indid, name,
(select case keyno when 0 then NULL else colid end as [data()]
from sys.sysindexkeys as k
where k.id = i.object_id
and k.indid = i.index_id
order by keyno, colid
for xml path('')) as cols,
(select case keyno when 0 then colid else NULL end as [data()]
from sys.sysindexkeys as k
where k.id = i.object_id
and k.indid = i.index_id
order by colid
for xml path('')) as inc
from sys.indexes as i
where i.type_desc <> 'CLUSTERED' and i.is_unique = 0
)
select top 1
object_schema_name(c1.id) + '.' + object_name(c1.id) as 'table',
c1.name as 'index',
c2.name as 'exactduplicate'
into #mike
from indexcols as c1
join indexcols as c2
on c1.id = c2.id
and c1.indid < c2.indid
and c1.cols = c2.cols
and c1.inc = c2.inc;
declare @sql_command varchar(max);
set @sql_command = '';
SELECT @sql_command += 'DROP INDEX ' + [exactduplicate] + '' + char(13)+ 'ON ' + [Table] + CHAR(13) FROM #mike
print @sql_command
exec (@sql_command);
drop table #mike