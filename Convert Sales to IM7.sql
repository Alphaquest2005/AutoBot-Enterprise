----Sales ------------ to im7

--1 import sales as normal
--2 run this query and save as csv import as opening stock
--have to handle RETURNS in sales sum sales
SELECT 'Mar20-S2OS' AS [Invoice #]]], EntryData.EntryDataDate AS Date, EntryDataDetails.ItemNumber AS [Item Code], EntryDataDetails.ItemDescription AS Description, EntryDataDetails.Quantity, EntryDataDetails.Cost
FROM    EntryData_Sales INNER JOIN
                 EntryData ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 InventoryItems ON EntryDataDetails.InventoryItemId = InventoryItems.Id
WHERE (EntryData.EntryDataDate BETWEEN CONVERT(DATETIME, '2020-03-01 00:00:00', 102) AND CONVERT(DATETIME, '2020-03-31 00:00:00', 102))


--make im7
--assess
--re import 
--allocate sales 
--check for errors - should be perfect
--set the effective assessment date
