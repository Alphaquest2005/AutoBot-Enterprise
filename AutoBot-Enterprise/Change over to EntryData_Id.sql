update EntryDataDetails
set EntryData_Id = entrydata.Id
from entrydatadetails inner join entrydata on entrydata.EntryDataId = entrydatadetails.EntryDataId

update EntryData_Adjustments
set EntryData_Id = entrydata.Id
from EntryData_Adjustments inner join entrydata on entrydata.EntryDataId = EntryData_Adjustments.EntryDataId

update EntryData_DocumentType
set EntryData_Id = entrydata.Id
from EntryData_DocumentType inner join entrydata on entrydata.EntryDataId = EntryData_DocumentType.EntryDataId

update EntryData_Invoices
set EntryData_Id = entrydata.Id
from EntryData_Invoices inner join entrydata on entrydata.EntryDataId = EntryData_Invoices.EntryDataId

update EntryData_OpeningStock
set EntryData_Id = entrydata.Id
from EntryData_OpeningStock inner join entrydata on entrydata.EntryDataId = EntryData_OpeningStock.EntryDataId

update EntryData_PurchaseOrders
set EntryData_Id = entrydata.Id
from EntryData_PurchaseOrders inner join entrydata on entrydata.EntryDataId = EntryData_PurchaseOrders.EntryDataId

update EntryData_Sales
set EntryData_Id = entrydata.Id
from EntryData_Sales inner join entrydata on entrydata.EntryDataId = EntryData_Sales.EntryDataId

EntryDataFiles
update EntryDataFiles
set EntryData_Id = entrydata.Id
from EntryDataFiles inner join entrydata on entrydata.EntryDataId = EntryDataFiles.EntryDataId

AsycudaDocumentSetEntryData
update AsycudaDocumentSetEntryData
set EntryData_Id = entrydata.Id
from AsycudaDocumentSetEntryData inner join entrydata on entrydata.EntryDataId = AsycudaDocumentSetEntryData.EntryDataId

AsycudaDocumentEntryData
update AsycudaDocumentEntryData
set EntryData_Id = entrydata.Id
from AsycudaDocumentEntryData inner join entrydata on entrydata.EntryDataId = AsycudaDocumentEntryData.EntryDataId


update EntryDataDetails
set AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId
from EntryDataDetails inner join AsycudaDocumentSetEntryData on EntryDataDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id



INSERT INTO xcuda_Inventory_Item
                 (Id, InventoryItemId)
SELECT xcuda_HScode.Item_Id, InventoryItems.Id--, InventoryItems.ItemNumber, InventoryItems.Description, InventorySources.Name
FROM    InventorySources INNER JOIN
                 InventoryItemSource ON InventorySources.Id = InventoryItemSource.InventorySourceId RIGHT OUTER JOIN
                 AsycudaDocumentItem INNER JOIN
                 xcuda_HScode ON AsycudaDocumentItem.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                 InventoryItems ON AsycudaDocumentItem.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND xcuda_HScode.Precision_4 = InventoryItems.ItemNumber ON 
                 InventoryItemSource.InventoryId = InventoryItems.Id
WHERE /*(xcuda_HScode.Item_Id = 19177) AND */ (InventorySources.Name <> N'POS' OR
                 InventorySources.Name IS NULL)

select * from xcuda_HScode where Item_Id = 19177

SELECT xcuda_HScode.Item_Id, InventoryItems.Id, InventoryItems.ItemNumber, InventoryItems.Description, InventorySources.Name
FROM    InventorySources INNER JOIN
                 InventoryItemSource ON InventorySources.Id = InventoryItemSource.InventorySourceId RIGHT OUTER JOIN
                 AsycudaDocumentItem INNER JOIN
                 xcuda_HScode ON AsycudaDocumentItem.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                 InventoryItems ON AsycudaDocumentItem.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND xcuda_HScode.Precision_4 = InventoryItems.ItemNumber ON 
                 InventoryItemSource.InventoryId = InventoryItems.Id
WHERE /*(xcuda_HScode.Item_Id = 19177) AND */ (InventorySources.Name <> N'POS' OR
                 InventorySources.Name IS NULL)