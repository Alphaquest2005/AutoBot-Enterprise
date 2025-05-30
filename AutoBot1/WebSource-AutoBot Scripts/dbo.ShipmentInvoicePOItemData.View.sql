USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE view [dbo].[ShipmentInvoicePOItemData]
as
SELECT DISTINCT 
                isnull(cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int),0) AS Id,  PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                 PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription,isnull(PODetails.TariffCode, INVDetails.TariffCode) as TariffCode, PODetails.Cost AS POCost, 
                 INVDetails.Cost AS INVCost, PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, PODetails.AliasItemId, INVDetails.POInventoryItemId, PODetails.InventoryItemId
FROM    ShipmentInvoicePOItemMISINVDetails AS INVDetails FULL OUTER JOIN
                     ShipmentInvoicePOItemMISPODetails AS PODetails ON  (isnull(INVDetails.POInventoryItemId,0) = isnull(PODetails.InventoryItemId,0) or isnull(INVDetails.InventoryItemId,0) = isnull(PODetails.AliasItemId,0)) AND 
                 INVDetails.EntryData_Id = PODetails.EntryData_Id AND INVDetails.InvoiceId = PODetails.InvoiceId  AND 
                 INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId

GO
