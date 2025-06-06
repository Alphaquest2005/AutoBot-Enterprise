USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[PreviousItemsEx-InvMatchBasic]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[PreviousItemsEx-InvMatchBasic]
 
as
SELECT InventoryItems.Id as InventoryItemsId, InventoryItems.ApplicationSettingsId, CASE WHEN aliasname IS NULL THEN inventoryitems.itemnumber ELSE AliasName END AS ItemNumber
FROM   dbo.[InventoryItemAliasEx] as InventoryItemAlias RIGHT OUTER JOIN
                 dbo.InventoryItems ON  InventoryItemAlias.InventoryItemId = dbo.InventoryItems.Id
GO
