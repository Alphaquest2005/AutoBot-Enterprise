DELETE FROM InventoryItemAlias
FROM            InventoryItemAliasEx LEFT OUTER JOIN
                         InventoryItems ON InventoryItemAliasEx.AliasItemId = InventoryItems.Id
WHERE        (InventoryItems.Id IS NULL)
