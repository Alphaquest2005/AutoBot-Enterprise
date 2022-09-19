select * from InventoryItems where id = 0


SELECT        InventoryItems.Id, InventoryItemAlias.AliasId, InventoryItemAlias.InventoryItemId, InventoryItemAlias.AliasName, InventoryItemAlias.AliasItemId
FROM            InventoryItems RIGHT OUTER JOIN
                         InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.AliasItemId
WHERE        (InventoryItems.Id IS NULL)


DELETE FROM InventoryItemAlias
FROM            InventoryItems RIGHT OUTER JOIN
                         InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.AliasItemId
WHERE        (InventoryItems.Id IS NULL)