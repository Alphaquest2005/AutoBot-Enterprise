USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-old]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ShipmentInvoicePOMatches-old]
AS
SELECT EntryData.EntryData_Id, EntryData.EntryDataId AS PONumber, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceNo
FROM    EntryData INNER JOIN
                 ShipmentInvoice LEFT OUTER JOIN
                 ShipmentInvoiceExtraInfo ON ShipmentInvoice.Id = ShipmentInvoiceExtraInfo.InvoiceId ON (trim(ShipmentInvoiceExtraInfo.Value) LIKE N'%' + CASE WHEN trim(EntryData.EntryDataId) 
                 = '' THEN 'unknown' ELSE trim(EntryData.EntryDataId) END + N'%' OR
                 EntryData.InvoiceTotal = ShipmentInvoice.InvoiceTotal AND EntryData.SupplierCode = ShipmentInvoice.SupplierCode AND EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId) AND 
                 EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId LEFT OUTER JOIN
                 ShipmentInvoicePOManualMatches ON ShipmentInvoice.InvoiceNo = ShipmentInvoicePOManualMatches.InvoiceNo AND EntryData.EntryDataId = ShipmentInvoicePOManualMatches.PONumber
WHERE (ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber') AND (ShipmentInvoicePOManualMatches.Id IS NULL) OR
                 (ShipmentInvoiceExtraInfo.Info IS NULL)
GROUP BY EntryData.EntryData_Id, EntryData.EntryDataId, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo


GO
