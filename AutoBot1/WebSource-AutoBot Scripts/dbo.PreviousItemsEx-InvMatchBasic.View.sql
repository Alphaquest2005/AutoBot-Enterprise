USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[PreviousItemsEx-InvMatchBasic]    Script Date: 4/3/2025 10:23:54 PM ******/
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
