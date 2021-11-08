select * from AsycudaDocumentItem where Commercial_Description like '%Thermal%'

select * from entrydata where entrydataid like '%009129%'

select * from EntryDataDetails where EntryData_Id = 463320

select * from AsycudaSalesAllocationsEx where ApplicationSettingsId = 2 and EntryDataDetailsId = 1669593

select * from AsycudaSalesAllocationsEx where ApplicationSettingsId = 2

SELECT AsycudaSalesAllocations.EntryDataDetailsId, EntryData_Sales.INVNumber, EntryData_Sales.CustomerName, EntryData.ApplicationSettingsId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Quantity, EntryDataDetails.EntryDataId, EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.TotalCost, EntryDataDetails.QtyAllocated, EntryDataDetails.DoNotAllocate, 
                 EntryDataDetails.Freight, EntryDataDetails.Weight, EntryDataDetails.TaxAmount, EntryDataDetails.InventoryItemId, EntryDataDetails.EntryDataDetailsKey
FROM    EntryData_Sales INNER JOIN
                 EntryDataDetails ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 EntryData ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData.EntryData_Id LEFT OUTER JOIN
                 AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId
WHERE (AsycudaSalesAllocations.EntryDataDetailsId IS NULL) and left(ItemNumber,1) = 'T' and Quantity <> 0 and ApplicationSettingsId = 2

SELECT AsycudaSalesAllocations.EntryDataDetailsId, EntryData_Sales.INVNumber, EntryData_Sales.CustomerName, EntryData.ApplicationSettingsId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Quantity, EntryDataDetails.EntryDataId, EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.TotalCost, EntryDataDetails.QtyAllocated, EntryDataDetails.DoNotAllocate, 
                 EntryDataDetails.Freight, EntryDataDetails.Weight, EntryDataDetails.TaxAmount, EntryDataDetails.InventoryItemId, EntryDataDetails.EntryDataDetailsKey
FROM    EntryData_Sales INNER JOIN
                 EntryDataDetails ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 EntryData ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData.EntryData_Id inner JOIN
                 AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId
WHERE left(ItemNumber,1) = 'T' and Quantity <> 0 and ApplicationSettingsId = 2

TCH/F6NO_38BK


select * from ItemSalesAsycudaPiSummary where ItemNumber = '23973152' order by EntryDataDate

select * from ItemSalesAsycudaPiSummary where ItemNumber = '65152760' order by EntryDataDate

select * from ItemSalesAsycudaPiSummary where ItemNumber = '390206' order by EntryDataDate

select * from ItemSalesAsycudaPiSummary where ItemNumber = '15594S' order by EntryDataDate

select * from ItemSalesAsycudaPiSummary where ItemNumber = '15458' or ItemNumber = '015458'order by EntryDataDate

select * from AsycudaSalesAndAdjustmentAllocationsEx where ApplicationSettingsId = 6 and itemnumber = 'TBH0195834'

select * from AsycudaSalesAndAdjustmentAllocationsEx where ApplicationSettingsId = 6 and itemnumber = 'WAM03600'

select * from AsycudaDocumentItem where CNumber = '30255' and linenumber = 109 and CustomsProcedure like '7500%'

select * from asycudadocumentitem where ItemNumber = '30255' or ItemNumber = '015458'

select * from AsycudaDocument where CustomsProcedure like '7400%'

65152760
23973152
select * from AsycudaDocumentItem where cnumber is null

select * from AsycudaItemPiQuantityData where ItemNumber = '15458' or ItemNumber = '015458'

select * from InventoryItemAliasEx where ItemNumber = '14534' or AliasName = '14534'

second ex9 bucket 
'20021'
'BG1007P-12'
'137'