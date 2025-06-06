INSERT INTO InventoryMapping
                 (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
select distinct ItemNumber, alias, mapping.ApplicationSettingsId from 
(SELECT ItemNumber, LEFT(TariffCode, 4) AS alias, ApplicationSettingsId--, Commercial_Description, CustomsProcedure, TariffCode
FROM    AsycudaDocumentItem
WHERE (ItemNumber <> LEFT(TariffCode, 4)) AND (ApplicationSettingsId = 7) AND (CustomsProcedure LIKE '7500%')) as mapping left outer join InventoryMapping on ItemNumber = POSItemCode and alias = AsycudaItemCode and mapping.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId
where InventoryMapping.POSItemCode is null