
SELECT        InventoryItems.ItemNumber, InventoryItems.Description, InventoryItems.Category, InventoryItems.TariffCode, InventoryItems.EntryTimeStamp, InventoryItems.Quantity
FROM            InventoryItems INNER JOIN
                         [InventoryItems-NonStock] ON InventoryItems.ItemNumber = [InventoryItems-NonStock].ItemNumber


UPDATE       EntryDataDetails
SET                DoNotAllocate = 1, Status = 'Not In Bond'
FROM            EntryDataDetails INNER JOIN
                         InventoryItems ON EntryDataDetails.ItemNumber = InventoryItems.ItemNumber INNER JOIN
                         [InventoryItems-NonStock] ON InventoryItems.ItemNumber = [InventoryItems-NonStock].ItemNumber