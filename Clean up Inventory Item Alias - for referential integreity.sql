select * from InventoryItems where id = 0


SELECT        InventoryItems.Id, InventoryItemAlias.AliasId, InventoryItemAlias.InventoryItemId, InventoryItemAlias.AliasName, InventoryItemAlias.AliasItemId
FROM            InventoryItems RIGHT OUTER JOIN
                         InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.AliasItemId
WHERE        (InventoryItems.Id IS NULL)


DELETE FROM InventoryItemAlias
FROM            InventoryItems RIGHT OUTER JOIN
                         InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.AliasItemId
WHERE        (InventoryItems.Id IS NULL)


Select * FROM InventoryItems 
WHERE        (InventoryItems.ItemNumber = '*')


select *
FROM           InventoryItemAlias  inner JOIN
                      InventoryItems    ON InventoryItems.Id = InventoryItemAlias.InventoryItemId
WHERE        (InventoryItems.ItemNumber = '*')

DELETE FROM InventoryItemAlias
FROM            InventoryItems RIGHT OUTER JOIN
                         InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.AliasItemId
WHERE        (InventoryItems.ItemNumber = '*')


select * from EntryDataDetails where ItemNumber = '*'