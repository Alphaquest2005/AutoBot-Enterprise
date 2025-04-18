USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[DeleteInventoryMappingFromInventory]    Script Date: 3/27/2025 1:48:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Alter date: <Alter Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteInventoryMappingFromInventory]
	-- Add the parameters for the stored procedure here
	@appsettingId int
AS
BEGIN

DELETE FROM InventoryItemAlias
FROM      InventoryItemAlias INNER JOIN
                InventoryItems ON InventoryItemAlias.InventoryItemId = InventoryItems.Id INNER JOIN
                InventoryItems AS AliasItems ON InventoryItemAlias.AliasItemId = AliasItems.Id INNER JOIN
                InventoryMapping ON InventoryItems.ItemNumber = InventoryMapping.POSItemCode AND AliasItems.ItemNumber = InventoryMapping.AsycudaItemCode AND InventoryItems.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId

END
GO
