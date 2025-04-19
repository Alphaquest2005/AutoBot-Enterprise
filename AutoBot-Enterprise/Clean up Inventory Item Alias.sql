INSERT INTO InventoryMapping
                 (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT InventoryItems.ItemNumber, LEFT(InventoryItems.TariffCode, 4) AS Alias, InventoryItems.ApplicationSettingsId
FROM    InventoryItems INNER JOIN
                 InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                 InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id LEFT OUTER JOIN
                 InventoryMapping AS InventoryMapping_1 ON InventoryItems.ItemNumber = InventoryMapping_1.POSItemCode AND InventoryItems.ApplicationSettingsId = InventoryMapping_1.ApplicationsSettingsId AND 
                 LEFT(InventoryItems.TariffCode, 4) = InventoryMapping_1.AsycudaItemCode
WHERE (InventorySources.Name = N'Asycuda') AND (InventoryItems.ApplicationSettingsId = 6) AND (InventoryMapping_1.POSItemCode IS NULL)


EXEC  [dbo].[CreateInventoryAliasFromInventoryMapping]

WITH CTE([ApplicationSettingsId],[ItemNumber],     
    DuplicateCount)
AS (SELECT [ApplicationSettingsId],[ItemNumber], 
           ROW_NUMBER() OVER(PARTITION BY [ApplicationSettingsId],[ItemNumber]
                                          
           ORDER BY ID) AS DuplicateCount
    FROM InventoryItems)
DELETE FROM CTE
WHERE DuplicateCount > 1;

delete from InventoryItemAlias where AliasItemId is null


WITH CTE(InventoryItemId,AliasItemId,    
    DuplicateCount)
AS (SELECT InventoryItemId,AliasItemId, 
           ROW_NUMBER() OVER(PARTITION BY InventoryItemId,AliasItemId
                                          
           ORDER BY aliasid) AS DuplicateCount
    FROM InventoryItemAlias)
DELETE FROM CTE
WHERE DuplicateCount > 1;