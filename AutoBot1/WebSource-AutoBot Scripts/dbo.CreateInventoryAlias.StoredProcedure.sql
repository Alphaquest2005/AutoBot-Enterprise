USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[CreateInventoryAlias]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CreateInventoryAlias] 
	-- Add the parameters for the stored procedure here
	@itemNumber varchar(50),@alias varchar(50), @appSettingId int
AS
BEGIN
	
INSERT INTO [InventoryItemAlias]
SELECT TOP (1) InventoryItems_1.Id, SupplierItems.Id AS aliasItemId
FROM    InventoryItems AS InventoryItems_1 INNER JOIN
                 InventoryItems AS SupplierItems ON InventoryItems_1.ApplicationSettingsId = SupplierItems.ApplicationSettingsId LEFT OUTER JOIN
                 InventoryItemAlias ON SupplierItems.Id = InventoryItemAlias.AliasItemId AND InventoryItems_1.Id = InventoryItemAlias.InventoryItemId
WHERE (InventoryItems_1.ItemNumber = @itemNumber) AND (InventoryItems_1.ApplicationSettingsId = @appSettingId) AND (InventoryItemAlias.InventoryItemId IS NULL) AND (SupplierItems.ItemNumber = @alias)

END
GO
