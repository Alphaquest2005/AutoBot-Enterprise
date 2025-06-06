USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-AdjustmentData]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




Create VIEW [dbo].[Reports-POSvsAsycuda-AdjustmentData]
AS

SELECT AdjustmentDetails.EntryDataId AS InvoiceNo, AdjustmentDetails.EffectiveDate, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END AS ItemNumber, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.InventoryItemId ELSE AdjustmentDetails.InventoryItemId END AS InventoryItemId, 
                 AdjustmentDetails.ItemDescription, sum( distinct ISNULL(AdjustmentDetails.ReceivedQty, 0) - ISNULL(AdjustmentDetails.InvoiceQty, 0)) AS Quantity, AVG(AdjustmentDetails.Cost) AS Cost, AdjustmentDetails.IsReconciled
--into #adjmentsData
FROM    AsycudaDocumentItemEntryDataDetails  with (nolock)RIGHT OUTER JOIN
                 AdjustmentDetails with (nolock) LEFT OUTER JOIN
                inventoryitemaliasEX as InventoryItemAlias with (nolock) ON AdjustmentDetails.ItemNumber = InventoryItemAlias.AliasName ON AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = AdjustmentDetails.EntryDataDetailsId
				inner join [Reports-POSvsAsycudaData] on AdjustmentDetails.EffectiveDate BETWEEN [Reports-POSvsAsycudaData].StartDate AND [Reports-POSvsAsycudaData].EndDate
				inner join ApplicationSettings on InventoryItemAlias.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId and AdjustmentDetails.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId
WHERE (ISNULL(AdjustmentDetails.ReceivedQty, 0) < ISNULL(AdjustmentDetails.InvoiceQty, 0))
     -- 7IT/PR-LBL-2X1 --- DID only show shorts because they have to act like sales, not include overs
      --AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL or ( AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL and isnull(AdjustmentDetails.IsReconciled, 0) = 1))
GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END, AdjustmentDetails.ItemDescription, AdjustmentDetails.EntryDataId, 
                 AdjustmentDetails.EffectiveDate,  AdjustmentDetails.IsReconciled, AdjustmentDetails.EntryDataDetailsId, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.InventoryItemId ELSE AdjustmentDetails.InventoryItemId END


--SELECT        ItemNumber, MAX(ItemDescription) AS ItemDescription, SUM(Quantity) AS Quantity, AVG(Cost) AS Cost, InventoryItemId
--FROM            dbo.[Reports-POSvsAsycuda-SalesData] AS [#salesData]
--WHERE        (InvoiceNo LIKE '%2O-Shorts%')
--GROUP BY ItemNumber,InventoryItemId
GO
