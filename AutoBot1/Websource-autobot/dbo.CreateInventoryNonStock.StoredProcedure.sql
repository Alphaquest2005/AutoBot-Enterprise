USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[CreateInventoryNonStock]    Script Date: 3/27/2025 1:48:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CreateInventoryNonStock] 
	-- Add the parameters for the stored procedure here
	@itemNumber varchar(50), @appSettingId int
AS
BEGIN
	
INSERT INTO [InventoryItems-NonStock]
SELECT InventoryItems.Id
FROM    InventoryItems LEFT OUTER JOIN
                 [InventoryItems-NonStock] AS [InventoryItems-NonStock_1] ON InventoryItems.Id = [InventoryItems-NonStock_1].InventoryItemId
WHERE (InventoryItems.ItemNumber = @itemNumber) AND (InventoryItems.ApplicationSettingsId = @appSettingId) AND ([InventoryItems-NonStock_1].InventoryItemId IS NULL)

END
GO
