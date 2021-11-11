SELECT Asycuda.Id, Asycuda.ItemNumber, Asycuda.ApplicationSettingsId, Asycuda.Name
FROM    (SELECT InventoryItems.Id, InventoryItems.ItemNumber, InventoryItems.ApplicationSettingsId, InventorySources.Name
                 FROM     InventoryItems INNER JOIN
                                  InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                                  InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id
                 WHERE  (InventorySources.Name = N'POS') AND (InventoryItems.ApplicationSettingsId = 6)) AS POS RIGHT OUTER JOIN
                     (SELECT InventoryItems_1.Id, InventoryItems_1.ItemNumber, InventoryItems_1.ApplicationSettingsId, InventorySources_1.Name
                      FROM     InventoryItems AS InventoryItems_1 INNER JOIN
                                       InventoryItemSource AS InventoryItemSource_1 ON InventoryItems_1.Id = InventoryItemSource_1.InventoryId INNER JOIN
                                       InventorySources AS InventorySources_1 ON InventoryItemSource_1.InventorySourceId = InventorySources_1.Id
                      WHERE  (InventorySources_1.Name = N'Asycuda') AND (InventoryItems_1.ApplicationSettingsId = 6)) AS Asycuda ON POS.ItemNumber = Asycuda.ItemNumber AND 
                 POS.ApplicationSettingsId = Asycuda.ApplicationSettingsId
WHERE (POS.Id IS NULL)



