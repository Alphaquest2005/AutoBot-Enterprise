USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[create_procedure]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[create_procedure]
@table		varchar(200),
@DeveloperName  varchar(200),
@Createtable varchar(20)
 AS
set nocount on
declare @testTable varchar(8000)

declare @testTable2 varchar(8000)
declare @testTable3 varchar(8000)
declare @opration	varchar(8000)
declare @final	varchar(8000)
declare @OP varchar(100)

set @testTable=''
set @testTable2=''
set @final=''
set @testTable3=''
set @opration=''
declare @Datetime varchar(50)
set @Datetime=getdate()


select @testTable=@testTable+ ',
			'+column_name from information_schema.columns where table_name=@table and (COLUMNPROPERTY(OBJECT_ID(@table), column_name, 'isidentity') = 0)  AND (column_default IS NULL)
select @testTable2=@testTable2+ ',
@'+column_name+'  '+data_type+'(' + cast(character_maximum_length as varchar(10)) +')' +    case is_nullable when 'no' then ' ' when 'yes' then '=null'  end  from information_schema.columns where table_name=@table  and (COLUMNPROPERTY(OBJECT_ID(@table), column_name, 'isidentity') = 0)and character_maximum_length<>null   AND (column_default IS NULL)and data_type<>'text'
select @testTable2=@testTable2+ ',
@'+column_name+'  '+data_type  from information_schema.columns where table_name=@table  and (COLUMNPROPERTY(OBJECT_ID(@table), column_name, 'isidentity') = 0)and (character_maximum_length=null  or  data_type='text' ) AND (column_default IS NULL) 
select @testTable3=@testTable3+ ',
			@'+column_name    from information_schema.columns where table_name=@table  and (COLUMNPROPERTY(OBJECT_ID(@table), column_name, 'isidentity') = 0)  AND (column_default IS NULL)

set @testTable=SUBSTRING(@testTable,2,len(@testTable))
set @testTable2=SUBSTRING(@testTable2,4,len(@testTable2))
set @testTable3=SUBSTRING(@testTable3,2,len(@testTable3))

set @opration=' insert into [' +@table+'] 
			(
			'+@testTable+'
			) 
		values
			(
			'+ @testTable3 +'
			)'
set @OP='InsertNew'+@table
set @final='/*
----------------------------------------------------------------------------------------
Store Procedure Name :  SP__'+@OP  +'
----------------------------------------------------------------------------------------
1- Creation Date :'+convert (varchar,getdate(),103) +'
2- Last Update   :'+convert (varchar,getdate(),103)+'
3- Parametars No:6
4- Creation By :'+@DeveloperName+'
5- Last Update By :'+@DeveloperName+'
6- Return Value : Dataset

---------------------------------------------------------------------------------------
*/
Create  PROCEDURE  SP__'+@OP+'
(
  '+ @testTable2 + '  
)
AS
 set nocount on 
' + @opration + '
Select * from   [' +@table +']'

exec (@final)
GO
