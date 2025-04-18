USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemMISData-Data]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[ShipmentInvoicePOItemMISData-Data]
as
SELECT DISTINCT 
                  cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id, PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                  PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, INVDetails.Cost AS INVCost, 
                  PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, 
                  ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) AS POAmount, 
                  INVDetails.POInventoryItemId, PODetails.InventoryItemId, PODetails.AliasItemId
FROM   InventoryItemAliasEx as  InventoryItemAlias INNER JOIN
                  ShipmentInvoicePOItemMISPODetails AS PODetails ON InventoryItemAlias.AliasItemId = PODetails.AliasItemId FULL OUTER JOIN
                  ShipmentInvoicePOItemMISINVDetails AS INVDetails ON InventoryItemAlias.AliasName = INVDetails.ItemNumber AND (ISNULL(INVDetails.POInventoryItemId, 0) = ISNULL(PODetails.InventoryItemId, 0)/* OR
                  ISNULL(INVDetails.InventoryItemId, 0) = ISNULL(PODetails.AliasItemId, 0)*/) AND PODetails.EntryData_Id = INVDetails.EntryData_Id AND PODetails.InvoiceId = INVDetails.InvoiceId AND 
                  PODetails.ApplicationSettingsId = INVDetails.ApplicationSettingsId
WHERE            --(
		(PODetails.EntryData_Id IS not NULL) and
                  (INVDetails.InvoiceId IS not NULL) and       (INVDetails.Quantity <> PODetails.Quantity)--) and (PODetails.InvoiceNo = '02714' or INVDetails.InvoiceNo = '15115') and (PODetails.ItemNumber = 'FAA/SSTD8X58')
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, INVDetails.ItemDescription, 
                  PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), ROUND(ISNULL(INVDetails.TotalCost, 
                  INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, PODetails.InventoryItemId, PODetails.ItemNumber, PODetails.AliasItemId

union   

SELECT DISTINCT 
                  cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id, PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                  PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, INVDetails.Cost AS INVCost, 
                  PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, 
                  ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) AS POAmount, 
                  INVDetails.POInventoryItemId, PODetails.InventoryItemId, MAX(PODetails.AliasItemId) AS AliasItemId
FROM     InventoryItems INNER JOIN
                  ShipmentInvoicePOItemMISINVDetails AS INVDetails ON InventoryItems.Id = INVDetails.POInventoryItemId FULL OUTER JOIN
                  InventoryItemAliasEx as InventoryItemAlias INNER JOIN
                  ShipmentInvoicePOItemMISPODetails AS PODetails ON InventoryItemAlias.AliasItemId = PODetails.AliasItemId ON InventoryItems.ItemNumber = PODetails.ItemNumber AND INVDetails.ItemNumber = InventoryItemAlias.AliasName AND 
                  (/*ISNULL(INVDetails.POInventoryItemId, 0) = ISNULL(PODetails.InventoryItemId, 0) OR*/
                  ISNULL(INVDetails.InventoryItemId, 0) = ISNULL(PODetails.AliasItemId, 0)) AND INVDetails.EntryData_Id = PODetails.EntryData_Id AND INVDetails.InvoiceId = PODetails.InvoiceId AND 
                  INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId
WHERE  --(
		(PODetails.EntryData_Id IS NULL) OR
                  (INVDetails.InvoiceId IS NULL)--) and (PODetails.InvoiceNo = '02714' or INVDetails.InvoiceNo = '15115') and (PODetails.ItemNumber = 'FAA/SSTD8X58')
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, INVDetails.ItemDescription, 
                  PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), ROUND(ISNULL(INVDetails.TotalCost, 
                  INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, PODetails.InventoryItemId, PODetails.ItemNumber

union
----- remove itemalias contraint

SELECT DISTINCT 
                cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id, PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                 PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, 
                 INVDetails.Cost AS INVCost, PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, 
                 ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, 
                 PODetails.Quantity * PODetails.Cost), 2) AS POAmount, INVDetails.POInventoryItemId, PODetails.InventoryItemId, MAX(PODetails.AliasItemId)AS AliasItemId --over (partition by PODetails.InventoryItemId) 
FROM    InventoryItems INNER JOIN
                 ShipmentInvoicePOItemMISPODetails AS PODetails ON InventoryItems.ItemNumber = PODetails.ItemNumber FULL OUTER JOIN
                 ShipmentInvoicePOItemMISINVDetails AS INVDetails ON PODetails.Quantity = INVDetails.Quantity AND ISNULL(PODetails.AliasItemId, 0) = ISNULL(INVDetails.InventoryItemId, 0) AND 
                 PODetails.EntryData_Id = INVDetails.EntryData_Id AND PODetails.InvoiceId = INVDetails.InvoiceId AND PODetails.ApplicationSettingsId = INVDetails.ApplicationSettingsId AND 
                 InventoryItems.Id = INVDetails.POInventoryItemId
WHERE --(
		(PODetails.EntryData_Id is null) OR
                 (INVDetails.InvoiceId is null)--) and (PODetails.InvoiceNo = '02714' or INVDetails.InvoiceNo = '15115') and (PODetails.ItemNumber = 'FAA/SSTD8X58')
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, 
                 INVDetails.ItemDescription, PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), 
                 ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, 
                 PODetails.InventoryItemId, PODetails.ItemNumber--, PODetails.AliasItemId
GO
