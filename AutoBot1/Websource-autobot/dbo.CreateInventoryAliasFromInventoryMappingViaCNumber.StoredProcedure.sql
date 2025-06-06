USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[CreateInventoryAliasFromInventoryMappingViaCNumber]    Script Date: 3/27/2025 1:48:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CreateInventoryAliasFromInventoryMappingViaCNumber] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN

INSERT INTO InventoryItemAlias
                 (InventoryItemId, AliasItemId)
SELECT case when POSInventory.Id < AsycudaInventory.Id then  POSInventory.Id  else AsycudaInventory.Id end, case when POSInventory.Id < AsycudaInventory.Id then AsycudaInventory.Id  else POSInventory.Id  end AS AliasITemId
FROM    InventoryItems AS POSInventory INNER JOIN
                 [InventoryMapping-ViaCNumber] AS inventorymapping ON POSInventory.ItemNumber = inventorymapping.POSItemCode AND POSInventory.ApplicationSettingsId = inventorymapping.ApplicationSettingsId INNER JOIN
                 AsycudaItemBasicInfo ON inventorymapping.LineNumber = AsycudaItemBasicInfo.LineNumber AND inventorymapping.CNumber = AsycudaItemBasicInfo.CNumber INNER JOIN
                 InventoryItems AS AsycudaInventory ON AsycudaItemBasicInfo.ItemNumber = AsycudaInventory.ItemNumber AND inventorymapping.ApplicationSettingsId = AsycudaInventory.ApplicationSettingsId LEFT OUTER JOIN
                 InventoryItemAlias AS InventoryItemAlias_1 ON AsycudaInventory.Id = InventoryItemAlias_1.AliasItemId AND POSInventory.Id = InventoryItemAlias_1.InventoryItemId
WHERE (InventoryItemAlias_1.AliasId IS NULL)
GROUP BY POSInventory.Id, AsycudaItemBasicInfo.ItemNumber, AsycudaInventory.Id
	


END
GO
