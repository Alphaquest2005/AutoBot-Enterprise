declare @startdate datetime, @endDate datetime, @OPSNumber varchar(50), @ItemNumber varchar(50)
set @startdate = '9/1/2018'
set @endDate = '11/2/2018'
set @OPSNumber = 'OPS-02-OCT-2018'
set @ItemNumber = 'FAA/SSTO6X34'

drop table ##sales
SELECT        EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, SUM(EntryDataDetails.Quantity) AS Quantity, AVG(EntryDataDetails.Cost) AS Cost
into ##sales
FROM            EntryData_Sales INNER JOIN
                         EntryData ON EntryData_Sales.EntryDataId = EntryData.EntryDataId INNER JOIN
                         EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId
WHERE        (EntryData.EntryDataDate BETWEEN @startdate AND @endDate)
GROUP BY EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription

select 'Sales'
select * from  ##sales where itemnumber = @ItemNumber

drop table ##AsycudaData
----
SELECT        'C#' + AsycudaDocumentRegistrationDate.CNumber + '-' + CAST(xcuda_Item.LineNumber AS varchar(50)) AS PrevDoc, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description AS Description, 
                         SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, 
                         SUM(CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, AVG(CAST(AsycudaDocumentItemCost.ItemCost AS float)) AS Cost, 
                         xcuda_HScode.Precision_4 AS ItemNumber
into ##AsycudaData
FROM            xcuda_HScode LEFT OUTER JOIN
                         InventoryItemAlias ON xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName RIGHT OUTER JOIN
                         ApplicationSettings RIGHT OUTER JOIN
                         AsycudaDocumentRegistrationDate ON ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate AND 
                         ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate RIGHT OUTER JOIN
                         xcuda_Item ON AsycudaDocumentRegistrationDate.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON xcuda_HScode.Item_Id = xcuda_Item.Item_Id LEFT OUTER JOIN
                         AsycudaDocumentItemCost ON xcuda_Item.Item_Id = AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                         AscyudaItemPiQuantity ON xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                         xcuda_Goods_description ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                         Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id
WHERE        (AsycudaDocumentRegistrationDate.AssessmentDate <= @enddate) AND (xcuda_Item.WarehouseError IS NULL)
GROUP BY AsycudaDocumentRegistrationDate.DocumentType, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber, xcuda_Item.LineNumber, 
                         InventoryItemAlias.AliasName
HAVING        (AsycudaDocumentRegistrationDate.DocumentType = 'IM7') AND (InventoryItemAlias.AliasName IS NULL) OR
                         (AsycudaDocumentRegistrationDate.DocumentType = 'OS7')

select 'AsycudaData'						 
select * from ##AsycudaData where itemnumber = @ItemNumber

drop table ##AsycudaSummary

select ItemNumber, Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
into ##AsycudaSummary
from ##AsycudaData
group by ItemNumber


select 'AsycudaSummary'
select * from ##AsycudaSummary where itemnumber = @ItemNumber

drop table ##OPS


SELECT        ISNULL(OPS.EntryDataId, 'Sales') AS EntryDataId, ISNULL(OPS.ItemNumber, sales.ItemNumber) AS ItemNumber, ISNULL(OPS.Quantity, 0) 
                         + ISNULL(sales.Quantity, 0) AS Quantity, ISNULL(OPS.ItemDescription, sales.ItemDescription) AS ItemDescription, CAST(ISNULL(OPS.Cost, sales.Cost) AS float) AS Cost, sales.Quantity AS Sales, 
                         OPS.Quantity AS QuantityOnHand
into ##OPS
FROM            (SELECT        [##Sales].ItemNumber, [##Sales].Quantity, [##Sales].ItemDescription, [##Sales].Cost
                          FROM            [##Sales]) AS sales FULL OUTER JOIN
                             (SELECT        EntryData_OpeningStock_1.EntryDataId, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float) AS Cost
                               FROM            EntryData_OpeningStock AS EntryData_OpeningStock_1 INNER JOIN
                                                         EntryDataDetails AS EntryDataDetails_1 ON EntryData_OpeningStock_1.EntryDataId = EntryDataDetails_1.EntryDataId
                               WHERE        (EntryData_OpeningStock_1.EntryDataId = @OPSNumber) 
                               GROUP BY EntryData_OpeningStock_1.EntryDataId, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float)) AS OPS ON 
                         sales.ItemNumber = OPS.ItemNumber
GROUP BY sales.Cost, sales.ItemNumber, OPS.ItemNumber, sales.Quantity, OPS.Quantity, OPS.EntryDataId, ops.ItemDescription, sales.ItemDescription, ops.Cost, sales.cost

select 'OPS'
select * from ##OPS where itemnumber = @ItemNumber



drop table [##MappingIssues]

------------- replace '/' & '-'
SELECT        [##OPS].Itemnumber, [##AsycudaData].ItemNumber AS Alias, [##OPS].ItemDescription, [##AsycudaData].description AS AlisasDescripton
into  [##MappingIssues]
FROM            [##AsycudaData] AS [##AsycudaData] INNER JOIN
                          [##OPS] AS [##OPS] ON REPLACE(REPLACE([##AsycudaData].ItemNumber, '/', ''),'-','') = REPLACE(REPLACE([##OPS].ItemNumber, '/', ''),'-','') and [##AsycudaData].ItemNumber <> [##OPS].ItemNumber


INSERT INTO [##MappingIssues]
                         (ItemNumber,Alias,ItemDescription,AlisasDescripton)
SELECT        [##OPS].Itemnumber, [##AsycudaData].ItemNumber AS Alias, [##OPS].ItemDescription, [##AsycudaData].description AS AlisasDescripton
FROM            [##AsycudaData] AS [##AsycudaData] INNER JOIN
                         [##OPS] AS [##OPS] ON REPLACE([##AsycudaData].ItemNumber, ' ', '') = REPLACE([##OPS].ItemNumber, ' ', '') and [##AsycudaData].ItemNumber <> [##OPS].ItemNumber

--where charIndex('-',[##AsycudaData].ItemNumber) > 0-- charIndex('/',[##OPS].ItemNumber) > 0 and 

--select * from [##AsycudaData]
--where charIndex('-',[##AsycudaData].ItemNumber) > 0

--select * from [##OPS]
--where charIndex('-',[##OPS].ItemNumber) > 0

select 'Mapping Issues'
 select * from [##MappingIssues] where itemnumber = @ItemNumber



INSERT INTO InventoryItemAlias
                         (ItemNumber,AliasName)
SELECT   distinct     [##MappingIssues].ItemNumber, [##MappingIssues].Alias
FROM            [##MappingIssues] LEFT OUTER JOIN
                         InventoryItemAlias AS InventoryItemAlias_1 ON [##MappingIssues].itemnumber = InventoryItemAlias_1.ItemNumber AND [##MappingIssues].Alias = InventoryItemAlias_1.AliasName
WHERE        (InventoryItemAlias_1.ItemNumber IS NULL)


drop table [##AsycudaData-ItemAlias]
SELECT   distinct    'C#' + AsycudaDocumentRegistrationDate.CNumber + '-' + cast(xcuda_Item.LineNumber as varchar(50)) AS PrevDoc, inventoryitemalias.AliasName AS Precision_4, dbo.xcuda_Goods_description.Commercial_Description AS Description, 
                         SUM(ISNULL(dbo.AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, 
                         SUM(CAST(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(dbo.AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, AVG(CAST(dbo.AsycudaDocumentItemCost.ItemCost AS float)) 
                         AS Cost, dbo.InventoryItemAlias.ItemNumber
into [##AsycudaData-ItemAlias]
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
WHERE        (AsycudaDocumentRegistrationDate.AssessmentDate <= @endDate and xcuda_Item.WarehouseError is null)
GROUP BY CASE WHEN aliasname IS NOT NULL THEN inventoryitemalias.itemnumber ELSE precision_4 END, dbo.AsycudaDocumentRegistrationDate.DocumentType, dbo.InventoryItemAlias.ItemNumber, 
                         dbo.xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber,xcuda_Item.LineNumber,inventoryitemalias.AliasName
HAVING        (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM7') OR
                         (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM9')OR
                         (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'OS7')

select '##AsycudaData-ItemAlias'
select * from [##AsycudaData-ItemAlias] where itemnumber = @ItemNumber


drop table [##AsycudaSummary-ItemAlias]

select ItemNumber, Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
into [##AsycudaSummary-ItemAlias]
from  [##AsycudaData-ItemAlias]
group by ItemNumber

select '##AsycudaSummary-ItemAlias'
select * from [##AsycudaSummary-ItemAlias] where itemnumber = @ItemNumber

insert into [##AsycudaSummary]
  (ItemNumber, Description, Quantity, Amount, PiQuantity, Cost)
select * from [##AsycudaSummary-ItemAlias]

select '[##AsycudaSummary] with itemalias'
select * from [##AsycudaSummary] where itemnumber = @ItemNumber

drop table [##StockDifferences-ItemItemNumber]
SELECT        isnull(##OPS.EntryDataId, 'Asycuda') as EntryDataId, ISNULL(##OPS.ItemNumber, ##AsycudaData.ItemNumber) AS ItemNumber, ISNULL(##OPS.ItemDescription, 
                         ##AsycudaData.Description) AS Description, ISNULL(##OPS.Quantity, ##AsycudaData.Amount) AS Quantity, ##OPS.Quantity AS [OPS-Quantity], 
                         ISNULL(##OPS.Cost, 0) AS OPSCost, ##AsycudaData.Amount AS [Asycuda-Quantity], ISNULL(##AsycudaData.Cost, 0) AS AsycudaCost, 
                         COALESCE (##OPS.Quantity, 0) - COALESCE (##AsycudaData.Amount, 0) AS Diff, ISNULL(##OPS.Cost, ##AsycudaData.Cost) AS Cost, 
                         (COALESCE (##OPS.Quantity, 0) - COALESCE (##AsycudaData.Amount, 0)) * ISNULL(##OPS.Cost, ##AsycudaData.Cost) AS TotalCost
into  [##StockDifferences-ItemItemNumber]
FROM            ##AsycudaSummary as ##AsycudaData FULL OUTER JOIN
                 ##OPS  as ##OPS ON ##AsycudaData.ItemNumber = ##OPS.ItemNumber

select '##StockDifferences-ItemItemNumber'
select * from [##StockDifferences-ItemItemNumber] where itemnumber = @ItemNumber

drop table  [##StockDifferences-ItemAlias]
SELECT        isnull(##OPS.EntryDataId, 'Asycuda') as EntryDataId, ISNULL(##OPS.ItemNumber, [##AsycudaData-ItemAlias].ItemNumber) AS ItemNumber, ISNULL(##OPS.ItemDescription, 
                         [##AsycudaData-ItemAlias].Description) AS Description, ISNULL(##OPS.Quantity, [##AsycudaData-ItemAlias].Amount) AS Quantity, 
                         ##OPS.Quantity AS [OPS-Quantity], ISNULL(##OPS.Cost, 0) AS OPSCost, [##AsycudaData-ItemAlias].Amount AS [Asycuda-Quantity], 
                         ISNULL([##AsycudaData-ItemAlias].Cost, 0) AS AsycudaCost, COALESCE (##OPS.Quantity, 0) - COALESCE ([##AsycudaData-ItemAlias].Amount, 0) AS Diff, 
                         ISNULL(##OPS.Cost, [##AsycudaData-ItemAlias].Cost) AS Cost, (COALESCE (##OPS.Quantity, 0) - COALESCE ([##AsycudaData-ItemAlias].Amount, 0)) 
                         * ISNULL(##OPS.Cost, [##AsycudaData-ItemAlias].Cost) AS TotalCost
into [##StockDifferences-ItemAlias]
FROM           [##AsycudaSummary-ItemAlias] as [##AsycudaData-ItemAlias] INNER JOIN
                         ##OPS ON [##AsycudaData-ItemAlias].ItemNumber = ##OPS.ItemNumber

select '##StockDifferences-ItemAlias'
select * from [##StockDifferences-ItemAlias] where itemnumber = @ItemNumber


drop table [##StockDifferences]
select * into [##StockDifferences]
from
(SELECT        EntryDataId, ItemNumber, description, Quantity, [OPS-Quantity], OPSCost, [Asycuda-Quantity], AsycudaCost, Diff, Cost, TotalCost
FROM            [##StockDifferences-ItemItemNumber]) as d

select '##StockDifferences'
select * from [##StockDifferences]  where itemnumber = @ItemNumber



drop table ##results

SELECT        EntryDataId, ItemNumber, max(Description) as Description, avg(isnull(Quantity,0)) AS Quantity, isnull(avg([OPS-Quantity]),0) AS [OPS-Quantity], AVG(isnull(OPSCost,0)) AS OPSCost, sum(isnull([Asycuda-Quantity],0)) AS [Asycuda-Quantity], AVG(isnull(AsycudaCost,0)) AS AsycudaCost, (isnull(avg([OPS-Quantity]),0) - isnull(sum([Asycuda-Quantity]), 0))
                         AS Diff, AVG(isnull(Cost,0)) AS Cost, AVG(isnull(TotalCost,0)) AS TotalCost
into ##Results
FROM            [##StockDifferences]
GROUP BY EntryDataId, ItemNumber
having((isnull(avg([OPS-Quantity]),0) - isnull(sum([Asycuda-Quantity]), 0)) <> 0)
order by abs((isnull(avg([OPS-Quantity]),0) - isnull(sum([Asycuda-Quantity]), 0))) asc, ItemNumber

select '##Results'
select * from ##Results where ItemNumber = @ItemNumber
select * from ##Results where entrydataid = 'Sales'






