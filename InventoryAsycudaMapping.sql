SELECT InventoryItems.ItemNumber AS Expr1, InventoryItems.Description, InventoryItems.TariffCode, InventoryAsycudaMapping.ItemNumber, InventoryAsycudaMapping.Item_Id, 
             InventoryAsycudaMapping.InventoryAsycudaMappingId, AsycudaDocumentItem.Item_price, AsycudaDocumentItem.ItemQuantity, AsycudaDocumentItem.Currency_rate, 
             AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.DocumentType, EntryDataDetails.Cost,
             AsycudaDocumentItem.Item_price / AsycudaDocumentItem.ItemQuantity * AsycudaDocumentItem.Currency_rate AS AsycudaCost
FROM   InventoryAsycudaMapping INNER JOIN
             InventoryItems ON InventoryAsycudaMapping.ItemNumber = InventoryItems.ItemNumber INNER JOIN
             AsycudaDocumentItem ON InventoryAsycudaMapping.Item_Id = AsycudaDocumentItem.Item_Id INNER JOIN
             EntryDataDetails ON InventoryItems.ItemNumber = EntryDataDetails.ItemNumber INNER JOIN
             EntryData_OpeningStock ON EntryDataDetails.EntryDataId = EntryData_OpeningStock.EntryDataId INNER JOIN
             AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id


SELECT InventoryItems.ItemNumber , InventoryItems.Description, InventoryItems.TariffCode, InventoryAsycudaMapping.ItemNumber, InventoryAsycudaMapping.Item_Id, 
             InventoryAsycudaMapping.InventoryAsycudaMappingId, AsycudaDocumentItem.Item_price, AsycudaDocumentItem.ItemQuantity, AsycudaDocumentItem.Currency_rate, AsycudaDocument.CNumber, 
             AsycudaDocument.RegistrationDate, AsycudaDocument.DocumentType, EntryDataDetails.Cost, 
             AsycudaDocumentItem.Item_price / AsycudaDocumentItem.ItemQuantity * AsycudaDocumentItem.Currency_rate AS AsycudaCost
FROM   InventoryAsycudaMapping RIGHT OUTER JOIN
             AsycudaDocument INNER JOIN
             AsycudaDocumentItem ON AsycudaDocument.ASYCUDA_Id = AsycudaDocumentItem.AsycudaDocumentId ON InventoryAsycudaMapping.Item_Id = AsycudaDocumentItem.Item_Id RIGHT OUTER JOIN
             EntryDataDetails INNER JOIN
             InventoryItems ON EntryDataDetails.ItemNumber = InventoryItems.ItemNumber INNER JOIN
             EntryData_OpeningStock ON EntryDataDetails.EntryDataId = EntryData_OpeningStock.EntryDataId ON InventoryAsycudaMapping.ItemNumber = InventoryItems.ItemNumber
WHERE (InventoryAsycudaMapping.ItemNumber is null)

SELECT InventoryAsycudaMapping.ItemNumber AS Expr1, InventoryAsycudaMapping.Item_Id AS Expr2, InventoryAsycudaMapping.InventoryAsycudaMappingId, AsycudaDocumentItem.*
FROM   InventoryAsycudaMapping INNER JOIN
             AsycudaDocumentItem ON InventoryAsycudaMapping.Item_Id = AsycudaDocumentItem.Item_Id
WHERE (InventoryAsycudaMapping.ItemNumber = '10260')

select * from subitems where itemnumber = '10260'