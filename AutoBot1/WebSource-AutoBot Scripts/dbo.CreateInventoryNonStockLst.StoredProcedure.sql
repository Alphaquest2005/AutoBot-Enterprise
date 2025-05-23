USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[CreateInventoryNonStockLst]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [dbo].[CreateInventoryNonStockLst] 

AS
BEGIN
	
INSERT INTO [InventoryItems-NonStock]
SELECT InventoryItems.Id
FROM    InventoryItems LEFT OUTER JOIN
                 [InventoryItems-NonStock] AS [InventoryItems-NonStock_1] ON InventoryItems.Id = [InventoryItems-NonStock_1].InventoryItemId
				 inner join [InventoryItems-NonStocklst] on InventoryItems.ItemNumber = [InventoryItems-NonStocklst].itemnumber
WHERE  ([InventoryItems-NonStock_1].InventoryItemId IS NULL)

END
GO
