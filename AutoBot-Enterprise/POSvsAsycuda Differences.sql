declare @startdate datetime, @endDate datetime, @OPSNumber varchar(50)
set @startdate = '9/1/2018'
set @endDate = '9/1/2018'
set @OPSNumber = 'OPS-2018-08-31'

drop table ##AsycudaData
--

SELECT        xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description AS Description, SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, 
                         SUM(Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, SUM(CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float)) 
                         AS Amount, AVG(CAST(AsycudaDocumentItemCost.ItemCost AS float)) AS Cost, CASE WHEN aliasname IS NOT NULL THEN inventoryitemalias.itemnumber ELSE precision_4 END AS ItemNumber
into ##AsycudaData
FROM            ApplicationSettings RIGHT OUTER JOIN
                         AsycudaDocumentRegistrationDate ON ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate AND 
                         ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate RIGHT OUTER JOIN
                         xcuda_Item ON AsycudaDocumentRegistrationDate.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                         xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id LEFT OUTER JOIN
                         InventoryItemAlias ON xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName LEFT OUTER JOIN
                         AsycudaDocumentItemCost ON xcuda_Item.Item_Id = AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                         AscyudaItemPiQuantity ON xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                         xcuda_Goods_description ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                         Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id
WHERE        (AsycudaDocumentRegistrationDate.AssessmentDate <= @endDate) AND (xcuda_Item.WarehouseError IS NULL)
GROUP BY AsycudaDocumentRegistrationDate.DocumentType, CASE WHEN aliasname IS NOT NULL THEN inventoryitemalias.itemnumber ELSE precision_4 END, xcuda_HScode.Precision_4, 
                         xcuda_Goods_description.Commercial_Description
HAVING        (AsycudaDocumentRegistrationDate.DocumentType = 'IM7') OR
                         (AsycudaDocumentRegistrationDate.DocumentType = 'OS7')

--select * from ##AsycudaData where itemnumber = 'CB/B114X112'


drop table [##AsycudaData-ItemAlias]
SELECT        CASE WHEN aliasname IS NOT NULL THEN inventoryitemalias.itemnumber ELSE precision_4 END AS Precision_4, dbo.xcuda_Goods_description.Commercial_Description AS Description, 
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
                         dbo.xcuda_Goods_description.Commercial_Description
HAVING        (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM7') OR
                         (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM9')OR
                         (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'OS7')

--select * from [##AsycudaData-ItemAlias] where itemnumber = 'CB/B114X112'

drop table ##OPS
SELECT        dbo.EntryData_OpeningStock.EntryDataId, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.Quantity, dbo.EntryDataDetails.ItemDescription, CAST(dbo.EntryDataDetails.Cost AS float) AS Cost
into ##OPS
FROM            dbo.EntryData_OpeningStock INNER JOIN
                         dbo.EntryDataDetails ON dbo.EntryData_OpeningStock.EntryDataId = dbo.EntryDataDetails.EntryDataId
WHERE        (dbo.EntryData_OpeningStock.EntryDataId = @OPSNumber)

-- select * from ##OPS

drop table [##StockDifferences-ItemItemNumber]
SELECT        ##OPS.EntryDataId, ISNULL(##OPS.ItemNumber, ##AsycudaData.ItemNumber) AS ItemNumber, ISNULL(##OPS.ItemDescription, 
                         ##AsycudaData.Description) AS Description, ISNULL(##OPS.Quantity, ##AsycudaData.Amount) AS Quantity, ##OPS.Quantity AS [OPS-Quantity], 
                         ISNULL(##OPS.Cost, 0) AS OPSCost, ##AsycudaData.Amount AS [Asycuda-Quantity], ISNULL(##AsycudaData.Cost, 0) AS AsycudaCost, 
                         COALESCE (##OPS.Quantity, 0) - COALESCE (##AsycudaData.Amount, 0) AS Diff, ISNULL(##OPS.Cost, ##AsycudaData.Cost) AS Cost, 
                         (COALESCE (##OPS.Quantity, 0) - COALESCE (##AsycudaData.Amount, 0)) * ISNULL(##OPS.Cost, ##AsycudaData.Cost) AS TotalCost
into  [##StockDifferences-ItemItemNumber]
FROM            ##AsycudaData as ##AsycudaData FULL OUTER JOIN
                 ##OPS  as ##OPS ON ##AsycudaData.ItemNumber = ##OPS.ItemNumber


-- select * from [##StockDifferences-ItemItemNumber] where itemnumber = 'CB/B114X112'

drop table  [##StockDifferences-ItemAlias]
SELECT        ##OPS.EntryDataId, ISNULL(##OPS.ItemNumber, [##AsycudaData-ItemAlias].Precision_4) AS ItemNumber, ISNULL(##OPS.ItemDescription, 
                         [##AsycudaData-ItemAlias].Description) AS Description, ISNULL(##OPS.Quantity, [##AsycudaData-ItemAlias].Amount) AS Quantity, 
                         ##OPS.Quantity AS [OPS-Quantity], ISNULL(##OPS.Cost, 0) AS OPSCost, [##AsycudaData-ItemAlias].Amount AS [Asycuda-Quantity], 
                         ISNULL([##AsycudaData-ItemAlias].Cost, 0) AS AsycudaCost, COALESCE (##OPS.Quantity, 0) - COALESCE ([##AsycudaData-ItemAlias].Amount, 0) AS Diff, 
                         ISNULL(##OPS.Cost, [##AsycudaData-ItemAlias].Cost) AS Cost, (COALESCE (##OPS.Quantity, 0) - COALESCE ([##AsycudaData-ItemAlias].Amount, 0)) 
                         * ISNULL(##OPS.Cost, [##AsycudaData-ItemAlias].Cost) AS TotalCost
into [##StockDifferences-ItemAlias]
FROM            [##AsycudaData-ItemAlias] INNER JOIN
                         ##OPS ON [##AsycudaData-ItemAlias].Precision_4 = ##OPS.ItemNumber

-- select * from [##StockDifferences-ItemAlias]


drop table [##StockDifferences]
select * into [##StockDifferences]
from
(SELECT        EntryDataId, ItemNumber, description, Quantity, [OPS-Quantity], OPSCost, [Asycuda-Quantity], AsycudaCost, Diff, Cost, TotalCost
FROM            [##StockDifferences-ItemAlias]
UNION
SELECT        EntryDataId, ItemNumber, description, Quantity, [OPS-Quantity], OPSCost, [Asycuda-Quantity], AsycudaCost, Diff, Cost, TotalCost
FROM            [##StockDifferences-ItemItemNumber]) as d

-- select * from [##StockDifferences]

SELECT        EntryDataId, ItemNumber, description, AVG(Quantity) AS Quantity, AVG([OPS-Quantity]) AS [OPS-Quantity], AVG(OPSCost) AS OPSCost, AVG([Asycuda-Quantity]) AS [Asycuda-Quantity], AVG(AsycudaCost) AS AsycudaCost, AVG(Diff) 
                         AS Diff, AVG(Cost) AS Cost, AVG(TotalCost) AS TotalCost
FROM            [##StockDifferences]
GROUP BY EntryDataId, ItemNumber, description
having(avg(diff) <> 0)
order by ItemNumber



