SELECT DISTINCT EntryData.EntryDataDate, EntryData.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Cost
FROM   EntryData INNER JOIN
             EntryDataDetails INNER JOIN
             InventoryItems ON EntryDataDetails.ItemNumber = InventoryItems.ItemNumber ON EntryData.EntryDataId = EntryDataDetails.EntryDataId LEFT OUTER JOIN
             xcuda_HScode ON InventoryItems.ItemNumber = xcuda_HScode.Precision_4
WHERE (xcuda_HScode.Precision_4 IS NULL) AND (EntryData.EntryDataDate >=
                 (SELECT TOP (1) OpeningStockDate
                 FROM    ApplicationSettings))