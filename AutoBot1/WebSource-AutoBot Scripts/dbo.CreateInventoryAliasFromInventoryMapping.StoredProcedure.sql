USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[CreateInventoryAliasFromInventoryMapping]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
CREATE PROCEDURE [dbo].[CreateInventoryAliasFromInventoryMapping] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN

INSERT INTO InventoryItemAlias
                 (InventoryItemId, AliasItemId)
SELECT case when InventoryItems_1.Id < InventoryItems.Id then  InventoryItems_1.Id else InventoryItems.Id end, case when InventoryItems_1.Id < InventoryItems.Id then InventoryItems.Id  else InventoryItems_1.Id end AS AliasITemId
FROM    InventoryItems AS InventoryItems_1 INNER JOIN
                 InventoryMapping ON InventoryItems_1.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND InventoryItems_1.ItemNumber = InventoryMapping.POSItemCode INNER JOIN
                 InventoryItems ON InventoryMapping.ApplicationsSettingsId = InventoryItems.ApplicationSettingsId AND InventoryMapping.AsycudaItemCode = InventoryItems.ItemNumber LEFT OUTER JOIN
                 InventoryItemAlias AS InventoryItemAlias_1 ON InventoryItems.Id = InventoryItemAlias_1.AliasItemId AND InventoryItems_1.Id = InventoryItemAlias_1.InventoryItemId
WHERE (InventoryItemAlias_1.AliasId IS NULL) and (InventoryMapping.POSItemCode <> InventoryMapping.AsycudaItemCode)
GROUP BY InventoryItems_1.Id, InventoryMapping.AsycudaItemCode, InventoryItems.Id
	
INSERT INTO InventoryItemAlias
                 (AliasItemId,InventoryItemId )
SELECT case when InventoryItems_1.Id < InventoryItems.Id then  InventoryItems_1.Id else InventoryItems.Id end, case when InventoryItems_1.Id < InventoryItems.Id then InventoryItems.Id  else InventoryItems_1.Id end AS AliasITemId
FROM    InventoryItems AS InventoryItems_1 INNER JOIN
                 InventoryMapping ON InventoryItems_1.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND InventoryItems_1.ItemNumber = InventoryMapping.POSItemCode INNER JOIN
                 InventoryItems ON InventoryMapping.ApplicationsSettingsId = InventoryItems.ApplicationSettingsId AND InventoryMapping.AsycudaItemCode = InventoryItems.ItemNumber LEFT OUTER JOIN
                 InventoryItemAlias AS InventoryItemAlias_1 ON InventoryItems.Id = InventoryItemAlias_1.AliasItemId AND InventoryItems_1.Id = InventoryItemAlias_1.InventoryItemId
WHERE (InventoryItemAlias_1.AliasId IS NULL) and (InventoryMapping.POSItemCode <> InventoryMapping.AsycudaItemCode)
GROUP BY InventoryItems_1.Id, InventoryMapping.AsycudaItemCode, InventoryItems.Id

END
GO
