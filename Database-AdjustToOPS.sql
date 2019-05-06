declare @startdate datetime, @endDate datetime, @OPSNumber varchar(50),@ItemNumber varchar(50),
		 @DoNotAllocateStockIssues bit,@DoNotAllocateStockIssuesWithStockOvers bit, @DoNotAllocateStockIssuesWithSalesErrors bit,
		 @AutoMapp bit
set @startdate = '9/1/2018'
set @endDate = '10/31/2018'
set @OPSNumber = 'OPS-10-31-2018'
set @ItemNumber = 'BNE/09031' --TAY/314
set @DoNotAllocateStockIssues = 0
set @DoNotAllocateStockIssuesWithStockOvers = 0
set @DoNotAllocateStockIssuesWithSalesErrors = 0
set @AutoMapp = 0


select 'Sales Data'
select * from  dbo.[Reports-POSvsAsycuda-SalesData] where itemnumber = @ItemNumber

select 'adjustments'
select * from  dbo.[Reports-POSvsAsycuda-adjustments] where itemnumber = @ItemNumber

select 'Returns'
select * from  dbo.[reports-POSvsAsycuda-Returns] where itemnumber = @ItemNumber


select 'Sales'
select * from  dbo.[Reports-POSvsAsycuda-Sales] where itemnumber = @ItemNumber

select 'Adjustments'
select * from  dbo.[Reports-POSvsAsycuda-Adjustments] where itemnumber = @ItemNumber


select 'AsycudaData'						 
select * from dbo.[Reports-POSvsAsycuda-AsycudaData] where itemnumber = @ItemNumber order by AssessmentDate asc


select 'AsycudaSummary-ItemNumber'
select * from dbo.[Reports-POSvsAsycuda-AsycudaSummary-ItemNumber] where itemnumber = @ItemNumber


select '#OPSData'
select * from dbo.[Reports-POSvsAsycuda-OPSData] where itemnumber = @ItemNumber

select 'OPS'
select * from dbo.[Reports-POSvsAsycuda-OPS] where itemnumber = @ItemNumber






		drop table [#MappingIssues]

		------------- replace '/' & '-'
		SELECT        [#OPS].Itemnumber, [#AsycudaData].ItemNumber AS Alias, [#OPS].ItemDescription, [#AsycudaData].description AS AlisasDescripton
		into  [#MappingIssues]
		FROM            dbo.[Reports-POSvsAsycuda-AsycudaData] AS [#AsycudaData] INNER JOIN
								  dbo.[Reports-POSvsAsycuda-OPS] AS [#OPS] ON REPLACE(REPLACE([#AsycudaData].ItemNumber, '/', ''),'-','') = REPLACE(REPLACE([#OPS].ItemNumber, '/', ''),'-','') and [#AsycudaData].ItemNumber <> [#OPS].ItemNumber


		INSERT INTO [#MappingIssues]
								 (ItemNumber,Alias,ItemDescription,AlisasDescripton)
		SELECT        [#OPS].Itemnumber, [#AsycudaData].ItemNumber AS Alias, [#OPS].ItemDescription, [#AsycudaData].description AS AlisasDescripton
		FROM            dbo.[Reports-POSvsAsycuda-AsycudaData] AS [#AsycudaData] INNER JOIN
								 dbo.[Reports-POSvsAsycuda-OPS] AS [#OPS] ON REPLACE([#AsycudaData].ItemNumber, ' ', '') = REPLACE([#OPS].ItemNumber, ' ', '') and [#AsycudaData].ItemNumber <> [#OPS].ItemNumber

		--where charIndex('-',[#AsycudaData].ItemNumber) > 0-- charIndex('/',[#OPS].ItemNumber) > 0 and 

		--select * from [#AsycudaData]
		--where charIndex('-',[#AsycudaData].ItemNumber) > 0

		--select * from [#OPS]
		--where charIndex('-',[#OPS].ItemNumber) > 0

		select 'Mapping Issues'
		 select * from [#MappingIssues] where itemnumber = @ItemNumber

if @AutoMapp = 1 
Begin

		INSERT INTO InventoryItemAlias
								 (ItemNumber,AliasName)
		SELECT   distinct     [#MappingIssues].ItemNumber, [#MappingIssues].Alias
		FROM            [#MappingIssues] LEFT OUTER JOIN
								 InventoryItemAlias AS InventoryItemAlias_1 ON [#MappingIssues].itemnumber = InventoryItemAlias_1.ItemNumber AND [#MappingIssues].Alias = InventoryItemAlias_1.AliasName
		WHERE        (InventoryItemAlias_1.ItemNumber IS NULL)

end 



select '#AsycudaData-ItemAlias'
select * from dbo.[Reports-POSvsAsycuda-AsycudaData-ItemAlias] where itemnumber = @ItemNumber


select '#AsycudaSummary-ItemAlias'
select * from dbo.[Reports-POSvsAsycuda-AsycudaSummary-ItemAlias] where itemnumber = @ItemNumber


select '[#AsycudaSummary]'
select * from dbo.[Reports-POSvsAsycuda-AsycudaSummary] where itemnumber = @ItemNumber


select '#StockDifferences-ItemNumber'
select * from dbo.[Reports-POSvsAsycuda-StockDifferences-ItemNumber] where itemnumber = @ItemNumber


select '#StockDifferences-ItemAlias'
select * from dbo.[Reports-POSvsAsycuda-StockDifferences-ItemAlias] where itemnumber = @ItemNumber



select '#StockDifferences'
select * from dbo.[Reports-POSvsAsycuda-StockDifferences]  where itemnumber = @ItemNumber

select 'OPS to Zero - Overs'
select * from dbo.[Reports-POSvsAsycuda-OPS to Zero - Overs] where ItemNumber = @ItemNumber


select 'P2O-Returns'
select * from dbo.[Reports-POSvsAsycuda-P2O-Returns] where ItemNumber = @ItemNumber


select '#Results'
select * from dbo.[Reports-POSvsAsycuda-Results] where ItemNumber = @ItemNumber





select 'P2O-Overs'
select * from dbo.[Reports-POSvsAsycuda-P2O-Overs] where ItemNumber = @ItemNumber


select 'P2O-Shorts'
select * from dbo.[Reports-POSvsAsycuda-P2O-Shorts] where ItemNumber = @ItemNumber

select 'A2O-Overs'
select * from dbo.[Reports-POSvsAsycuda-A2O-Overs] where ItemNumber = @ItemNumber


select 'A2O-Shorts'
select * from dbo.[Reports-POSvsAsycuda-A2O-Shorts] where ItemNumber = @ItemNumber

select 'Unresolved Issues'
select * from dbo.[Reports-POSvsAsycuda-Unresolved Issues] 

select 'Non-Stock-Differences'
select * from dbo.[Reports-POSvsAsycuda-Non-Stock-Differences] where ItemNumber = @ItemNumber

select 'Sales Errors -> to OPS - Overs'
select * from dbo.[Reports-POSvsAsycuda-Sales Errors -> to OPS - Overs] where ItemNumber = @ItemNumber


----------------------- Do Not Allocate Sales With Stock Issues-----------------------------------------
if(@DoNotAllocateStockIssues = 1)
Begin
update EntryDataDetails 
set DoNotAllocate = Null, [Status] = Null
where [Status] = 'Stock Issues'



update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM    dbo.[Reports-POSvsAsycuda-Results] as [#Results] inner join  EntryDataDetails  on #Results.ItemNumber = Entrydatadetails.ItemNumber  INNER JOIN
                         EntryData_Sales ON EntryDataDetails.EntryDataId = EntryData_Sales.EntryDataId INNER JOIN
                         EntryData ON EntryData_Sales.EntryDataId = EntryData.EntryDataId
                         
WHERE        ([#Results].InvoiceNo <> 'Asycuda') and (EntryDataDetails.[Status] = 'Stock Issues' or EntryDataDetails.[Status] is null) AND (diff <> 0) AND (EntryData.EntryDataDate BETWEEN @startDate AND @enddate)
End


if(@DoNotAllocateStockIssuesWithStockOvers = 1)
Begin
update EntryDataDetails 
set DoNotAllocate = Null, [Status] = Null
where [Status] = 'Stock Issues'



update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM    dbo.[Reports-POSvsAsycuda-Results] as [#Results] inner join  EntryDataDetails  on #Results.ItemNumber = Entrydatadetails.ItemNumber  INNER JOIN
                         EntryData_Sales ON EntryDataDetails.EntryDataId = EntryData_Sales.EntryDataId INNER JOIN
                         EntryData ON EntryData_Sales.EntryDataId = EntryData.EntryDataId
                         
WHERE        ([#Results].InvoiceNo <> 'Asycuda') and (EntryDataDetails.[Status] = 'Stock Issues' or EntryDataDetails.[Status] is null) AND (diff > 0) AND (EntryData.EntryDataDate BETWEEN @startDate AND @enddate)
End


--if(@DoNotAllocateStockIssuesWithSalesErrors = 1)
--Begin
--update EntryDataDetails 
--set DoNotAllocate = Null, [Status] = Null
--where [Status] = 'Stock Issues'



--update EntryDataDetails 
--set DoNotAllocate = 1, [Status] = 'Stock Issues'
--FROM     [#Results] inner join  EntryDataDetails  on #Results.ItemNumber = Entrydatadetails.ItemNumber INNER JOIN
--                         AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
--                         EntryData_Sales ON EntryDataDetails.EntryDataId = EntryData_Sales.EntryDataId INNER JOIN
--                         EntryData ON EntryData_Sales.EntryDataId = EntryData.EntryDataId
                         
--WHERE        (EntryDataDetails.EntryDataId <> 'Asycuda') and (EntryDataDetails.[Status] = 'Stock Issues' or EntryDataDetails.[Status] is null) AND (diff > 0) AND (EntryData.EntryDataDate BETWEEN @startDate AND @enddate) and AsycudaSalesAllocations.Status is not null
--End

--select 'Asycuda Issues'
--select * from dbo.[Reports-POSvsAsycuda-Results] as #Results where [#Results].EntryDataId = 'Asycuda' and ItemNumber = @ItemNumber

----------------------------------------------------------------------------------------------

select 'Sales with Stock Issues'
SELECT        EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.LineNumber, EntryData_Sales.TaxAmount, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, 
                         EntryDataDetails.Cost, EntryDataDetails.DoNotAllocate, EntryDataDetails.Status
FROM            EntryData INNER JOIN
                         EntryData_Sales ON EntryData.EntryDataId = EntryData_Sales.EntryDataId INNER JOIN
                         EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId
where EntryDataDetails.[Status] = 'Stock Issues' 