USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemMISINVDetails-Data]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
















CREATE VIEW [dbo].[ShipmentInvoicePOItemMISINVDetails-Data]
AS
SELECT ShipmentInvoiceDetails.Id AS DetailId, ShipmentInvoice.Id AS InvoiceId,ShipmentInvoice.InvoiceDate , ShipmentInvoice.InvoiceNo, ShipmentInvoiceDetails.ItemNumber, ShipmentInvoiceDetails.ItemDescription, ShipmentInvoiceDetails.Cost, 
                 ShipmentInvoiceDetails.Quantity, ShipmentInvoiceDetails.SalesFactor, ShipmentInvoiceDetails.TotalCost, ShipmentInvoice.ApplicationSettingsId, ShipmentInvoiceDetails.InventoryItemId, 
                 InventoryItemAlias.InventoryItemId AS POInventoryItemId, ShipmentInvoice.ImportedLines, InventoryItems.TariffCode, CASE WHEN Volume.units = 'Gallons' THEN Volume.quantity ELSE 0 END AS Gallons, SourceFile
FROM    ShipmentInvoice INNER JOIN
                 ShipmentInvoiceDetails ON ShipmentInvoice.Id = ShipmentInvoiceDetails.ShipmentInvoiceId INNER JOIN
                 InventoryItems ON ShipmentInvoiceDetails.InventoryItemId = InventoryItems.Id LEFT OUTER JOIN
                 [ShipmentInvoiceDetails-Volume] as Volume ON ShipmentInvoiceDetails.Id = Volume.Id LEFT OUTER JOIN
                 [InventoryItemAliasEx] as InventoryItemAlias ON ShipmentInvoiceDetails.InventoryItemId = InventoryItemAlias.AliasItemId
GROUP BY ShipmentInvoiceDetails.Id, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo, ShipmentInvoiceDetails.ItemNumber, ShipmentInvoiceDetails.ItemDescription, ShipmentInvoiceDetails.Cost, 
                 ShipmentInvoiceDetails.Quantity, ShipmentInvoiceDetails.SalesFactor, ShipmentInvoiceDetails.TotalCost, ShipmentInvoice.ApplicationSettingsId, ShipmentInvoiceDetails.InventoryItemId, 
                 ShipmentInvoice.ImportedLines, InventoryItems.TariffCode, Volume.Quantity, Volume.Units, ShipmentInvoice.InvoiceDate, InventoryItemAlias.InventoryItemId, SourceFile
GO
