USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemQueryMatches-Match]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[ShipmentInvoicePOItemQueryMatches-Match]
as
---- matches
SELECT  PODetailsId AS PODetailsId, INVDetailsId, POId, INVId, PONumber, InvoiceNo, POItemCode, INVItemCode, PODescription, INVDescription, POCost, INVCost, POQuantity, INVQuantity, Gallons, POTotalCost, 
                 INVTotalCost, INVInventoryItemId, POInventoryItemId, MAX(RankNo) AS RankNo
FROM    (SELECT PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.EntryDataId AS PONumber, INVDetails.InvoiceNo, 
                 PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, 
                 INVDetails.Cost / INVDetails.SalesFactor AS INVCost, PODetails.Quantity AS POQuantity, INVDetails.Quantity * INVDetails.SalesFactor AS INVQuantity, INVDetails.Gallons, ROUND(PODetails.totalcost, 2) AS POTotalCost, 
                 ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, INVDetails.InventoryItemId AS INVInventoryItemId, PODetails.InventoryItemId AS POInventoryItemId, 
                 COALESCE (
				 
					 CASE WHEN (INVDetails.ItemNumber LIKE '%' + PODetails.ItemNumber + '%' OR
					 PODetails.ItemNumber LIKE '%' + INVDetails.ItemNumber + '%') THEN 4 ELSE NULL END,

					 CASE WHEN (INVDetails.ItemNumber = replace(PODetails.ItemNumber,'0','o') OR
					 PODetails.ItemNumber = replace(INVDetails.ItemNumber,'0','o') ) THEN 3 ELSE NULL END,
					 
					 CASE WHEN INVDetails.PoInventoryItemId = PODetails.InventoryItemId THEN 2 ELSE null END, 
				 
					 CASE WHEN (ROUND(INVDetails.Cost / INVDetails.SalesFactor, 2) = ROUND(PODetails.Cost, 2) AND 
					 INVDetails.Quantity * INVDetails.SalesFactor = PODetails.Quantity AND ROUND(INVDetails.TotalCost, 2) = ROUND(PODetails.TotalCost, 2)) THEN 1 ELSE 0 END

					 
				 ) AS RankNo
		FROM    ShipmentInvoicePOs INNER JOIN
							 (SELECT ApplicationSettingsId, DetailId, InvoiceId, InvoiceNo, ItemNumber, ItemDescription, Cost, Quantity, Gallons, SalesFactor, TotalCost, InventoryItemId, POInventoryItemId
							  FROM     ShipmentInvoiceDetailsEx) AS INVDetails ON INVDetails.InvoiceId = ShipmentInvoicePOs.InvoiceId INNER JOIN
							 (SELECT EntryDataDetailsEx.ApplicationSettingsId, EntryDataDetailsEx.EntryDataDetailsId, EntryDataDetailsEx.EntryData_Id, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.ItemNumber, 
											   EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.Cost, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.Total AS totalcost, EntryDataDetailsEx.InventoryItemId
							  FROM     EntryDataDetailsEx INNER JOIN
											   EntryData_PurchaseOrders ON EntryDataDetailsEx.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id) AS PODetails ON ShipmentInvoicePOs.EntryData_Id = PODetails.EntryData_Id AND 
						 COALESCE (
						 CASE WHEN (INVDetails.ItemNumber LIKE '%' + left(PODetails.ItemNumber,4) + '%' OR  PODetails.ItemNumber LIKE '%' + INVDetails.ItemNumber + '%') THEN 1 ELSE NULL END,
						 CASE WHEN (INVDetails.ItemDescription LIKE '%' + substring(PODetails.ItemNumber,5, len(PODetails.ItemNumber)) + '%' OR  PODetails.ItemDescription LIKE '%' + INVDetails.ItemNumber + '%') THEN 1 ELSE NULL END,
						 --CASE WHEN (ROUND(INVDetails.Cost / INVDetails.SalesFactor, 2) = ROUND(PODetails.Cost, 2) AND INVDetails.Quantity * INVDetails.SalesFactor = PODetails.Quantity AND ROUND(INVDetails.TotalCost, 2) = ROUND(PODetails.TotalCost, 2)) THEN 1 ELSE NULL END,-- //// creating too much mapping when is same price for multiple items
						 CASE WHEN INVDetails.PoInventoryItemId = PODetails.InventoryItemId THEN 1 ELSE null END
						 ) = 1 AND INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId LEFT OUTER JOIN
						 InventoryItemAlias AS InventoryItemAlias_1 ON INVDetails.InventoryItemId = InventoryItemAlias_1.AliasItemId
		WHERE (InventoryItemAlias_1.AliasId IS NULL)) AS t
where   rankno > 0
GROUP BY PODetailsId, INVDetailsId, POId, INVId, PONumber, InvoiceNo, POItemCode, INVItemCode, PODescription, INVDescription, POCost, INVCost, POQuantity, INVQuantity, Gallons, POTotalCost, INVTotalCost, INVInventoryItemId, 
                 POInventoryItemId

GO
