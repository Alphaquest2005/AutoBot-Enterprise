USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[InventoryItemAliasEx-NoReverseMappings]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[InventoryItemAliasEx-NoReverseMappings]
AS
with cte(AliasId)
as
(
SELECT   
                InventoryItemAlias_1.AliasId AS Expr1
FROM      InventoryItemAlias INNER JOIN
                InventoryItems ON InventoryItemAlias.InventoryItemId = InventoryItems.Id INNER JOIN
                InventoryItems AS AliasItems ON InventoryItemAlias.AliasItemId = AliasItems.Id INNER JOIN
                InventoryItemAlias AS InventoryItemAlias_1 ON AliasItems.Id = InventoryItemAlias_1.InventoryItemId
WHERE   (InventoryItems.ItemNumber <> N'') AND (AliasItems.ItemNumber <> N'') AND (InventoryItemAlias.InventoryItemId < InventoryItemAlias.AliasItemId) /*AND (InventoryItems.ItemNumber IN ('SEH/1002GH', 'SEH/1002G', 'SEH-1002GH', 'SEH1002GH'))*/
)
SELECT   InventoryItemAlias.AliasId, InventoryItems.ApplicationSettingsId, InventoryItemAlias.InventoryItemId, InventoryItems.ItemNumber, AliasItems.ItemNumber AS AliasName, InventoryItemAlias.AliasItemId, cte.AliasId as ep
FROM      InventoryItemAlias INNER JOIN
                InventoryItems ON InventoryItemAlias.InventoryItemId = InventoryItems.Id INNER JOIN
                InventoryItems AS AliasItems ON InventoryItemAlias.AliasItemId = AliasItems.Id
				left outer join cte on InventoryItemAlias.AliasId = cte.AliasId
WHERE   (InventoryItems.ItemNumber <> N'') AND (AliasItems.ItemNumber <> N'') AND (InventoryItemAlias.InventoryItemId < InventoryItemAlias.AliasitemId) 
		 and cte.AliasId is null
GO
