declare @startdate datetime, @endDate datetime, @ItemNumber varchar(50)
		
set @startdate = '9/1/2018'
set @endDate = '2/28/2019'




drop table #AsycudaData
----
SELECT AsycudaDocumentRegistrationDate.CNumber, xcuda_Item.LineNumber, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description AS Description, 
                 SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(ISNULL(xcuda_Item.DFQtyAllocated, 0) + ISNULL(xcuda_Item.DPQtyAllocated, 0)) AS QtyAllocated, 
                 SUM(Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, SUM(CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) 
                 AS float)) AS Amount, AVG(CAST(AsycudaDocumentItemCost.LocalItemCost AS float)) AS Cost, ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) AS ItemNumber, 
                 AsycudaDocumentRegistrationDate.AssessmentDate, AsycudaDocumentRegistrationDate.RegistrationDate, AsycudaDocumentBasicInfo.Reference, AsycudaDocumentItemValueWeights.Net_weight_itm
into #AsycudaData
FROM    ApplicationSettings RIGHT OUTER JOIN
                 AsycudaDocumentRegistrationDate ON ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate AND 
                 ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate RIGHT OUTER JOIN
                 AsycudaDocumentItemValueWeights RIGHT OUTER JOIN
                 xcuda_Item ON AsycudaDocumentItemValueWeights.Item_Id = xcuda_Item.Item_Id ON AsycudaDocumentRegistrationDate.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_HScode LEFT OUTER JOIN
                 InventoryItemAlias ON xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id LEFT OUTER JOIN
                 AsycudaDocumentItemCost ON xcuda_Item.Item_Id = AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                     (SELECT Item_Id, SUM(PiQuantity) AS PiQuantity, SUM(PiWeight) AS PiWeight
                      FROM     AsycudaItemPiQuantityData
                      WHERE  (AssessmentDate <= @endDate)
                      GROUP BY Item_Id) AS AscyudaItemPiQuantity ON xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                 xcuda_Goods_description ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                 Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id LEFT OUTER JOIN
                 AsycudaDocumentBasicInfo ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (AsycudaDocumentRegistrationDate.AssessmentDate <= @endDate) AND (xcuda_Item.WarehouseError IS NULL) AND (ISNULL(AsycudaDocumentBasicInfo.Cancelled, 0) <> 1) AND 
                 (ISNULL(AsycudaDocumentBasicInfo.DoNotAllocate, 0) <> 1) and (ISNULL(AsycudaDocumentBasicInfo.IsManuallyAssessed, 0) <> 1)
GROUP BY AsycudaDocumentRegistrationDate.DocumentType, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber, xcuda_Item.LineNumber, 
                 InventoryItemAlias.AliasName, AsycudaDocumentRegistrationDate.AssessmentDate, AsycudaDocumentRegistrationDate.RegistrationDate, InventoryItemAlias.ItemNumber, AsycudaDocumentBasicInfo.Reference, 
                 AsycudaDocumentItemValueWeights.Net_weight_itm
HAVING (AsycudaDocumentRegistrationDate.DocumentType = 'IM7' OR
                 AsycudaDocumentRegistrationDate.DocumentType = 'OS7')/* AND (ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) = xcuda_HScode.Precision_4)*/
ORDER BY AsycudaDocumentRegistrationDate.AssessmentDate

------------------------Bad Weights---------------------------------------------

drop table [#Bad Weights]					 
select * 
into [#Bad Weights]	
from #AsycudaData 
Where round(Net_weight_itm/Quantity,2) < 0.01 and PiQuantity <> Quantity

select * from [#AsycudaData] where ItemNumber = 'POR/2684488'



drop table [#BadWeights-Shorts]
SELECT  distinct 'CBWShorts-' + FORMAT(@startdate, 'MMMyy') + '-' + FORMAT(@endDate, 'MMMyy') as [Invoice #], @startdate as [Date],[#Results].CNumber, [#Results].ItemNumber,[#Results].Description, [#Results].[Amount] as [From Quantity], 0 as [To Quantity], [#Results].Cost, [#Results].Quantity as Quantity, 'XCD' as Currency
into [#BadWeights-Shorts]
FROM     [#Bad Weights] as [#Results] 

drop table [#BadWeights-Overs]
SELECT  distinct 'CBWOvers-' + FORMAT(@startdate, 'MMMyy') + '-' + FORMAT(@endDate, 'MMMyy') as [Invoice #], @startdate as [Date],[#Results].CNumber, [#Results].ItemNumber,[#Results].Description, 0 as [From Quantity], [#Results].[Amount] as [To Quantity], [#Results].Cost, [#Results].Quantity as Quantity, 'XCD' as Currency
into [#BadWeights-Overs]
FROM     [#Bad Weights] as [#Results] 

select 'Bad Weight Shorts'
select * from [#BadWeights-Shorts]
select 'Bad Weight Overs'
select * from [#BadWeights-Overs]


set @ItemNumber = 'POR/2684488' 
select 'Bad Weights'
select * from [#Bad Weights] where itemnumber = @ItemNumber order by AssessmentDate asc

select 'Bad Weight Shorts'
select * from [#BadWeights-Shorts] where itemnumber = @ItemNumber

select 'Bad Weight Overs'
select * from [#BadWeights-Overs] where itemnumber = @ItemNumber

--update xcuda_Weight_itm
--set Gross_weight_itm = ItemQuantity *.01, Net_weight_itm = ItemQuantity *.01
--from xcuda_Weight_itm inner join AsycudaDocumentItem on xcuda_Weight_itm.Valuation_item_Id = AsycudaDocumentItem.Item_Id
--where CNumber is null and round(AsycudaDocumentItem.Net_weight_itm/ItemQuantity,2) < 0.01

--SELECT AsycudaDocumentRegistrationDate.CNumber, xcuda_Item.LineNumber, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description AS Description, SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 
--                 0)) AS PiQuantity, SUM(ISNULL(xcuda_Item.DFQtyAllocated, 0) + ISNULL(xcuda_Item.DPQtyAllocated, 0)) AS QtyAllocated, SUM(Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, 
--                 SUM(CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, 
--                 AVG(CAST(AsycudaDocumentItemCost.LocalItemCost AS float)) AS Cost, ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) AS ItemNumber, AsycudaDocumentRegistrationDate.AssessmentDate, 
--                 AsycudaDocumentRegistrationDate.RegistrationDate, AsycudaDocumentBasicInfo.Reference, AsycudaDocumentItemValueWeights.Net_weight_itm
--FROM    ApplicationSettings RIGHT OUTER JOIN
--                 AsycudaDocumentRegistrationDate ON ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate AND 
--                 ApplicationSettings.OpeningStockDate >= AsycudaDocumentRegistrationDate.AssessmentDate RIGHT OUTER JOIN
--                 AsycudaDocumentItemValueWeights RIGHT OUTER JOIN
--                 xcuda_Item ON AsycudaDocumentItemValueWeights.Item_Id = xcuda_Item.Item_Id ON AsycudaDocumentRegistrationDate.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
--                 xcuda_HScode LEFT OUTER JOIN
--                 InventoryItemAlias ON xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id LEFT OUTER JOIN
--                 AsycudaDocumentItemCost ON xcuda_Item.Item_Id = AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
--                     (SELECT Item_Id, SUM(PiQuantity) AS PiQuantity, SUM(PiWeight) AS PiWeight
--                      FROM     AsycudaItemPiQuantityData
--                      WHERE  (AssessmentDate <= '3/1/2019')
--                      GROUP BY Item_Id) AS AscyudaItemPiQuantity ON xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
--                 xcuda_Goods_description ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
--                 Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id LEFT OUTER JOIN
--                 AsycudaDocumentBasicInfo ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
--WHERE (xcuda_HScode.Precision_4 = 'POR2684488') AND (AsycudaDocumentRegistrationDate.AssessmentDate <= '3/1/2019') AND (xcuda_Item.WarehouseError IS NULL) AND (ISNULL(AsycudaDocumentBasicInfo.Cancelled, 0) 
--                 <> 1) AND (ISNULL(AsycudaDocumentBasicInfo.DoNotAllocate, 0) <> 1) AND (ISNULL(AsycudaDocumentBasicInfo.IsManuallyAssessed, 0) <> 1)
--GROUP BY AsycudaDocumentRegistrationDate.DocumentType, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber, xcuda_Item.LineNumber, 
--                 InventoryItemAlias.AliasName, AsycudaDocumentRegistrationDate.AssessmentDate, AsycudaDocumentRegistrationDate.RegistrationDate, InventoryItemAlias.ItemNumber, AsycudaDocumentBasicInfo.Reference, 
--                 AsycudaDocumentItemValueWeights.Net_weight_itm
--HAVING (AsycudaDocumentRegistrationDate.DocumentType = 'IM7' OR
--                 AsycudaDocumentRegistrationDate.DocumentType = 'OS7') AND ( InventoryItemAlias.ItemNumber is null or ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) = xcuda_HScode.Precision_4)
--ORDER BY AsycudaDocumentRegistrationDate.AssessmentDate











