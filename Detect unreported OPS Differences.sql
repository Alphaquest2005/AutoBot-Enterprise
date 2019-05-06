declare @startdate datetime, @endDate datetime, @FirstOPSNumber varchar(50), @OPSAsycudaRef varchar(50), @SecondOPSNumber varchar(50), @ItemNumber varchar(50)
set @startdate = '9/1/2018'
set @endDate = '10/31/2018'
set @OPSAsycudaRef = 'OPS-2018-08-31'
set @FirstOPSNumber = 'OPS-2018-08-31'
set @SecondOPSNumber = 'OPS-10-31-2018'
set @ItemNumber = 'WHA/WX7154' --TAY/314,FAS/SSMA10X60


drop table #AdjustmentData

SELECT        EntryData.EntryDataId, EntryData.EntryDataDate, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE entrydatadetails.ItemNumber END AS ItemNumber, EntryDataDetails.ItemDescription, 
                         SUM(EntryDataDetails.Quantity) AS Quantity, AVG(EntryDataDetails.Cost) AS Cost
into #AdjustmentData
FROM            EntryData_Adjustments INNER JOIN
                         EntryData ON EntryData_Adjustments.EntryDataId = EntryData.EntryDataId INNER JOIN
                         EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId LEFT OUTER JOIN
                         InventoryItemAlias ON EntryDataDetails.ItemNumber = InventoryItemAlias.AliasName
WHERE        (EntryData.EntryDataDate BETWEEN @startdate AND @endDate)
GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE entrydatadetails.ItemNumber END, EntryDataDetails.ItemDescription, EntryData.EntryDataId, EntryData.EntryDataDate

select 'Adjustment Data'
select * from  #AdjustmentData where itemnumber = @ItemNumber

drop table #Adjustments
SELECT        ItemNumber, ItemDescription, SUM(Quantity) AS Quantity, 
                         AVG(Cost) AS Cost
into #Adjustments
FROM   #AdjustmentData         
WHERE        (EntryDataDate BETWEEN @startdate AND @endDate)
GROUP BY ItemNumber , ItemDescription

select 'Adjustment'
select * from  #Adjustments where itemnumber = @ItemNumber

------------------------------------------------------------------

drop table #salesData

SELECT        EntryData.EntryDataId, EntryData.EntryDataDate, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE entrydatadetails.ItemNumber END AS ItemNumber, EntryDataDetails.ItemDescription, 
                         SUM(EntryDataDetails.Quantity) AS Quantity, AVG(EntryDataDetails.Cost) AS Cost
into #salesData
FROM            EntryData_Sales INNER JOIN
                         EntryData ON EntryData_Sales.EntryDataId = EntryData.EntryDataId INNER JOIN
                         EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId LEFT OUTER JOIN
                         InventoryItemAlias ON EntryDataDetails.ItemNumber = InventoryItemAlias.AliasName
WHERE        (EntryData.EntryDataDate BETWEEN @startdate AND @endDate)
GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE entrydatadetails.ItemNumber END, EntryDataDetails.ItemDescription, EntryData.EntryDataId, EntryData.EntryDataDate

select 'Sales Data'
select * from  #salesData where itemnumber = @ItemNumber

drop table #sales
SELECT        ItemNumber, ItemDescription, SUM(Quantity) AS Quantity, 
                         AVG(Cost) AS Cost
into #sales
FROM   #salesData         
WHERE        (EntryDataDate BETWEEN @startdate AND @endDate)
GROUP BY ItemNumber , ItemDescription

select 'Sales'
select * from  #sales where itemnumber = @ItemNumber


drop table #AsycudaData
----

SELECT        'C#' + AsycudaDocumentRegistrationDate.CNumber + '-' + CAST(xcuda_Item.LineNumber AS varchar(50)) AS PrevDoc, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description AS Description, 
                         SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, 
                         SUM(CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, AVG(CAST(AsycudaDocumentItemCost.LocalItemCost AS float)) AS Cost, 
                         ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) AS ItemNumber, AsycudaDocumentRegistrationDate.AssessmentDate, AsycudaDocumentRegistrationDate.RegistrationDate, 
                         AsycudaDocumentBasicInfo.Reference
into #AsycudaData
FROM            ApplicationSettings RIGHT OUTER JOIN
                         AsycudaDocumentRegistrationDate ON ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate AND 
                         ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate RIGHT OUTER JOIN
                         AsycudaDocumentBasicInfo RIGHT OUTER JOIN
                         xcuda_Item ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON AsycudaDocumentRegistrationDate.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                         xcuda_HScode LEFT OUTER JOIN
                         InventoryItemAlias ON xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id LEFT OUTER JOIN
                         AsycudaDocumentItemCost ON xcuda_Item.Item_Id = AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                         AscyudaItemPiQuantity ON xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                         xcuda_Goods_description ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                         Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id
WHERE       (NOT (AsycudaDocumentBasicInfo.Reference LIKE N'%' + @OPSAsycudaRef + N'%')) and (AsycudaDocumentRegistrationDate.AssessmentDate >= @StartDate) AND (AsycudaDocumentRegistrationDate.AssessmentDate <= @enddate) AND (xcuda_Item.WarehouseError IS NULL)
GROUP BY AsycudaDocumentRegistrationDate.DocumentType, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber, xcuda_Item.LineNumber, 
                         InventoryItemAlias.AliasName, AsycudaDocumentRegistrationDate.AssessmentDate, AsycudaDocumentRegistrationDate.RegistrationDate, InventoryItemAlias.ItemNumber, AsycudaDocumentBasicInfo.Reference
HAVING        (AsycudaDocumentRegistrationDate.DocumentType = 'IM7' OR
                         AsycudaDocumentRegistrationDate.DocumentType = 'OS7') AND (ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) = xcuda_HScode.Precision_4) 
                         
ORDER BY AsycudaDocumentRegistrationDate.AssessmentDate

select 'AsycudaData'						 
select * from #AsycudaData where itemnumber = @ItemNumber order by AssessmentDate asc

drop table #AsycudaSummary

select ItemNumber, Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
into #AsycudaSummary
from #AsycudaData
group by ItemNumber


select 'AsycudaSummary'
select * from #AsycudaSummary where itemnumber = @ItemNumber


drop table [#AsycudaData-ItemAlias]
SELECT   distinct    'C#' + AsycudaDocumentRegistrationDate.CNumber + '-' + cast(xcuda_Item.LineNumber as varchar(50)) AS PrevDoc, inventoryitemalias.AliasName AS Precision_4, dbo.xcuda_Goods_description.Commercial_Description AS Description, 
                         SUM(ISNULL(dbo.AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, 
                         SUM(CAST(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(dbo.AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, AVG(CAST(dbo.AsycudaDocumentItemCost.LocalItemCost AS float)) 
                         AS Cost, dbo.InventoryItemAlias.ItemNumber
into [#AsycudaData-ItemAlias]
FROM            dbo.xcuda_Item INNER JOIN
                         dbo.AsycudaDocumentRegistrationDate ON dbo.AsycudaDocumentRegistrationDate.ASYCUDA_Id = dbo.xcuda_Item.ASYCUDA_Id INNER JOIN
                         dbo.xcuda_HScode ON dbo.xcuda_Item.Item_Id = dbo.xcuda_HScode.Item_Id INNER JOIN
                         dbo.ApplicationSettings ON dbo.ApplicationSettings.OpeningStockDate <= dbo.AsycudaDocumentRegistrationDate.AssessmentDate AND 
                         dbo.ApplicationSettings.OpeningStockDate <= dbo.AsycudaDocumentRegistrationDate.AssessmentDate INNER JOIN
                         dbo.InventoryItemAlias ON dbo.xcuda_HScode.Precision_4 = dbo.InventoryItemAlias.AliasName LEFT OUTER JOIN
                         dbo.AsycudaDocumentItemCost ON dbo.xcuda_Item.Item_Id = dbo.AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                         dbo.AscyudaItemPiQuantity ON dbo.xcuda_Item.Item_Id = dbo.AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                         dbo.xcuda_Goods_description ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                         dbo.Primary_Supplementary_Unit ON dbo.xcuda_Item.Item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id
WHERE        (AsycudaDocumentRegistrationDate.AssessmentDate >= @StartDate and AsycudaDocumentRegistrationDate.AssessmentDate <= @endDate and xcuda_Item.WarehouseError is null)
GROUP BY CASE WHEN aliasname IS NOT NULL THEN inventoryitemalias.itemnumber ELSE precision_4 END, dbo.AsycudaDocumentRegistrationDate.DocumentType, dbo.InventoryItemAlias.ItemNumber, 
                         dbo.xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber,xcuda_Item.LineNumber,inventoryitemalias.AliasName
HAVING        (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM7') OR
                         --(dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM9')OR
                         (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'OS7')

select '#AsycudaData-ItemAlias'
select * from [#AsycudaData-ItemAlias] where itemnumber = @ItemNumber


drop table [#AsycudaSummary-ItemAlias]

select ItemNumber, Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
into [#AsycudaSummary-ItemAlias]
from  [#AsycudaData-ItemAlias]
group by ItemNumber

select '#AsycudaSummary-ItemAlias'
select * from [#AsycudaSummary-ItemAlias] where itemnumber = @ItemNumber

insert into [#AsycudaSummary]
  (ItemNumber, Description, Quantity, Amount, PiQuantity, Cost)
select * from [#AsycudaSummary-ItemAlias]

select '[#AsycudaSummary] with itemalias'
select * from [#AsycudaSummary] where itemnumber = @ItemNumber



drop table #FirstOPS


SELECT        coalesce(OPS.EntryDataId, (case when sales.ItemNumber is null then null else 'Sales' end) , (case when asycuda.ItemNumber is null then null else 'Asycuda' end) , (case when adjustments.ItemNumber is null then null else 'Adjustments' end)) AS EntryDataId, coalesce(OPS.ItemNumber, sales.ItemNumber, asycuda.ItemNumber, adjustments.ItemNumber) AS ItemNumber, ISNULL(OPS.Quantity, 0) - ISNULL(sales.Quantity, 0) + ISNULL(sum(asycuda.Quantity), 0)  + isnull(adjustments.Quantity, 0) AS Quantity, coalesce(OPS.ItemDescription, 
                         sales.ItemDescription, max(asycuda.ItemDescription), adjustments.itemdescription) AS ItemDescription, CAST(coalesce(OPS.Cost, sales.Cost,avg(asycuda.Cost), adjustments.cost) AS float) AS Cost, sales.Quantity AS Sales, OPS.Quantity AS OPSQuantity, sum(asycuda.Quantity) as Asycuda, sum(adjustments.Quantity) as Adjustment
into #FirstOPS
FROM            (SELECT        [#Sales].ItemNumber, [#Sales].Quantity, [#Sales].ItemDescription, [#Sales].Cost
                          FROM            [#Sales]) AS sales FULL OUTER JOIN
                             (SELECT        EntryData_OpeningStock_1.EntryDataId, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float) AS Cost
                               FROM            EntryData_OpeningStock AS EntryData_OpeningStock_1 INNER JOIN
                                                         EntryDataDetails AS EntryDataDetails_1 ON EntryData_OpeningStock_1.EntryDataId = EntryDataDetails_1.EntryDataId
                               WHERE        (EntryData_OpeningStock_1.EntryDataId = @FirstOPSNumber)
                               GROUP BY EntryData_OpeningStock_1.EntryDataId, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float)) AS OPS ON 
                         sales.ItemNumber = OPS.ItemNumber full outer JOIN
                             (SELECT ItemNumber, Description as ItemDescription, Quantity, PiQuantity, Amount as QuantityOnHand, Cost
                               FROM            [#AsycudaSummary]) AS asycuda on OPS.ItemNumber = asycuda.ItemNumber
							   full outer JOIN
                             (SELECT ItemNumber, ItemDescription as ItemDescription, Quantity, Cost
                               FROM            [#Adjustments]) AS adjustments on OPS.ItemNumber = adjustments.ItemNumber
GROUP BY sales.Cost, sales.ItemNumber, OPS.ItemNumber, sales.Quantity, OPS.Quantity, OPS.EntryDataId, OPS.ItemDescription, sales.ItemDescription, OPS.Cost, sales.Cost
	     , asycuda.ItemNumber, adjustments.ItemNumber, adjustments.ItemDescription, adjustments.Cost, adjustments.Quantity


select 'First OPS'
select * from #FirstOPS where itemnumber = @ItemNumber


drop table #SecondOPS

SELECT        EntryDataId, ItemNumber, ISNULL(Quantity, 0) AS Quantity, ItemDescription, CAST(Cost AS float) AS Cost, Quantity AS QuantityOnHand
into #SecondOPS
FROM            (SELECT        EntryData_OpeningStock_1.EntryDataId, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float) AS Cost
                          FROM            EntryData_OpeningStock AS EntryData_OpeningStock_1 INNER JOIN
                                                    EntryDataDetails AS EntryDataDetails_1 ON EntryData_OpeningStock_1.EntryDataId = EntryDataDetails_1.EntryDataId
                          WHERE        (EntryData_OpeningStock_1.EntryDataId = @SecondOPSNumber)
                          GROUP BY EntryData_OpeningStock_1.EntryDataId, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float)) AS OPS
GROUP BY ItemNumber, Quantity, EntryDataId, ItemDescription, Cost, ISNULL(Quantity, 0), CAST(Cost AS float)
select 'Second OPS'
select * from #SecondOPS where itemnumber = @ItemNumber


drop table [#StockDifferences]

SELECT        isnull(#OPS.EntryDataId, 'FirstOPS') as EntryDataId, ISNULL(#OPS.ItemNumber, #AsycudaData.ItemNumber) AS ItemNumber, ISNULL(#OPS.ItemDescription, 
                         #AsycudaData.ItemDescription) AS Description, ISNULL(#OPS.Quantity, sum(#AsycudaData.Quantity)) AS Quantity, #OPS.Quantity AS [OPS-Quantity], 
                         ISNULL(#OPS.Cost, 0) AS OPSCost, sum(#AsycudaData.Quantity) AS [FirstOPS-Quantity], ISNULL(avg(#AsycudaData.Cost), 0) AS [FirstOPS-Cost], 
                         COALESCE (#OPS.Quantity, 0) - COALESCE (sum(#AsycudaData.Quantity), 0) AS Diff, ISNULL(#OPS.Cost, avg(#AsycudaData.Cost)) AS Cost, 
                         (COALESCE (#OPS.Quantity, 0) - COALESCE (sum(#AsycudaData.Quantity), 0)) * ISNULL(#OPS.Cost, avg(#AsycudaData.Cost)) AS TotalCost
into   [#StockDifferences]
FROM            #FirstOPS as #AsycudaData FULL OUTER JOIN
                 #SecondOPS  as #OPS ON #AsycudaData.ItemNumber = #OPS.ItemNumber
group by #OPS.EntryDataId, #AsycudaData.ItemNumber,#OPS.ItemNumber,#OPS.ItemDescription,#AsycudaData.ItemDescription,#OPS.Quantity,#OPS.Cost


select '#StockDifferences'
select * from [#StockDifferences]  where itemnumber = @ItemNumber



drop table #results

SELECT        EntryDataId, [#StockDifferences].ItemNumber as ItemNumber, max(Description) as Description, avg(isnull(Quantity,0)) AS Quantity, isnull(avg([OPS-Quantity]),0) AS [OPS-Quantity], AVG(isnull(OPSCost,0)) AS OPSCost, sum(isnull([FirstOPS-Quantity],0)) AS [FirstOPS-Quantity], AVG(isnull([FirstOPS-Cost],0)) AS [FirstOPS-Cost], (isnull(avg([OPS-Quantity]),0) - isnull(sum([FirstOPS-Quantity]), 0))
                         AS Diff, AVG(isnull(Cost,0)) AS Cost, AVG(isnull(TotalCost,0)) AS TotalCost
into #Results
FROM            [#StockDifferences] left outer join [InventoryItems-NonStock] on [#StockDifferences].ItemNumber = [InventoryItems-NonStock].ItemNumber
where [InventoryItems-NonStock].ItemNumber is null
GROUP BY EntryDataId, [#StockDifferences].ItemNumber
having((isnull(avg([OPS-Quantity]),0) - isnull(sum([FirstOPS-Quantity]), 0)) <> 0)
order by abs((isnull(avg([OPS-Quantity]),0) - isnull(sum([FirstOPS-Quantity]), 0))) asc, [#StockDifferences].ItemNumber

select '#Results'
select * from #Results where ItemNumber = @ItemNumber

select 'Sales -> to OPS - Overs'
drop table [#Overs]
SELECT  distinct     [#Results].*
into [#Overs]
FROM     [#Results]                        
WHERE        (diff > 0) 


select * from [#Overs] where ItemNumber = @ItemNumber

select 'Sales -> to OPS - Shorts'
drop table [#Shorts]
SELECT  distinct     [#Results].*
into [#Shorts]
FROM     [#Results]                          
WHERE        (diff < 0 and [OPS-Quantity] > 0) 

select * from [#Shorts] where ItemNumber = @ItemNumber

