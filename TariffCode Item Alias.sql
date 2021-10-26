--- clear Item alias

DELETE FROM InventoryItemAlias
FROM    InventoryItems INNER JOIN
                 InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.InventoryItemId
WHERE (InventoryItems.ApplicationSettingsId = 6)

---- Import sales with tariff codes set to necessary level of detail -Columbian emeralds = 5 letters

---- Create aliases for  Asycuda Items 

INSERT INTO InventoryMapping
                 (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
select distinct ItemNumber, alias, mapping.ApplicationSettingsId from 
(SELECT ItemNumber, LEFT(TariffCode, 6) AS alias, ApplicationSettingsId--, Commercial_Description, CustomsProcedure, TariffCode
FROM    AsycudaDocumentItem
WHERE (ItemNumber <> LEFT(TariffCode, 6)) AND (ApplicationSettingsId = 6) AND (CustomsProcedure LIKE '7500%')) as mapping left outer join InventoryMapping on ItemNumber = POSItemCode and alias = AsycudaItemCode and mapping.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId
where InventoryMapping.POSItemCode is null



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



