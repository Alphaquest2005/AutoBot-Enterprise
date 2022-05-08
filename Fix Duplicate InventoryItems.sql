
drop table #inventorywithRow

select id,itemnumber, (ROW_NUMBER() over (partition by itemnumber order by id desc))  row
into #inventorywithRow
from InventoryItems

select * from #inventorywithRow where row > 1 order by ItemNumber

select * from #inventorywithRow where itemnumber = 'AB111510'

UPDATE EntryDataDetails
SET         InventoryItemId = original.Id
FROM    EntryDataDetails INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow]
                      WHERE  (row = 1)  ) AS original ON EntryDataDetails.ItemNumber = original.ItemNumber INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow] AS [#inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON EntryDataDetails.ItemNumber = Dups.ItemNumber AND EntryDataDetails.InventoryItemId = Dups.id

UPDATE InventoryItemAlias
SET         InventoryItemId = original.Id
FROM    InventoryItemAlias INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow]
                      WHERE  (row = 1) ) AS original ON InventoryItemAlias.InventoryItemId = original.Id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow] AS [#inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON InventoryItemAlias.InventoryItemId = Dups.id


UPDATE [InventoryItems-Lumped]
SET         InventoryItemId = original.Id
FROM    [InventoryItems-Lumped] INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow]
                      WHERE  (row = 1) ) AS original ON [InventoryItems-Lumped].InventoryItemId = original.Id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow] AS [#inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON [InventoryItems-Lumped].InventoryItemId = Dups.id

UPDATE [InventoryItems-NonStock]
SET         InventoryItemId = original.Id
FROM    [InventoryItems-NonStock] INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow]
                      WHERE  (row = 1) ) AS original ON [InventoryItems-NonStock].InventoryItemId = original.Id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow] AS [#inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON [InventoryItems-NonStock].InventoryItemId = Dups.id

UPDATE [InventoryItemSource]
SET         InventoryId = original.Id
FROM    [InventoryItemSource] INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow]
                      WHERE  (row = 1) ) AS original ON [InventoryItemSource].InventoryId = original.Id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow] AS [#inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON [InventoryItemSource].InventoryId = Dups.id

UPDATE [xcuda_Inventory_Item]
SET         InventoryItemId = original.Id
FROM    [xcuda_Inventory_Item] INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow]
                      WHERE  (row = 1) ) AS original ON [xcuda_Inventory_Item].InventoryItemId = original.Id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [#inventorywithRow] AS [#inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON [xcuda_Inventory_Item].InventoryItemId = Dups.id

--select * from [xcuda_Inventory_Item] where InventoryItemId in (select id from #inventorywithRow where row > 1)
delete from InventoryItems where id in (select id from #inventorywithRow where row > 1 )


------------------------------------------------fix iww problem

--drop table #descriptionwithRow

--select EntryDataDetailsId, entrydataid, itemnumber,ItemDescription, (ROW_NUMBER() over (partition by itemnumber order by EntryDataDetailsId asc))  row
--into #descriptionwithRow
--from EntryDataDetails  

--update EntryDataDetails
--set ItemDescription = good.itemdescription
--from
----select entrydatadetails.ItemDescription, good.ItemDescription from
--EntryDataDetails inner join 
--(select * from #descriptionwithRow where row > 1 and entrydataid like 'A2O%') err on EntryDataDetails.EntryDataDetailsId = err.EntryDataDetailsId inner join 
--(select * from #descriptionwithRow where row = 1 ) good on EntryDataDetails.ItemNumber = good.ItemNumber and EntryDataDetails.ItemDescription <> good.ItemDescription


--update InventoryItems
--set Description = good.itemdescription
--from
----select inventoryitems.Description ,entrydatadetails.ItemDescription, good.ItemDescription from
--InventoryItems inner join 
--EntryDataDetails on InventoryItems.id = EntryDataDetails.InventoryItemId inner join 
--(select * from #descriptionwithRow where row > 1 and entrydataid like 'A2O%') err on EntryDataDetails.EntryDataDetailsId = err.EntryDataDetailsId inner join 
--(select * from #descriptionwithRow where row = 1 ) good on EntryDataDetails.ItemNumber = good.ItemNumber 