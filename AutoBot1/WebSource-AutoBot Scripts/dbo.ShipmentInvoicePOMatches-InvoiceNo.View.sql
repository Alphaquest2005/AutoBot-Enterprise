USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-InvoiceNo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ShipmentInvoicePOMatches-InvoiceNo]
AS


SELECT cast(row_number() OVER ( ORDER BY ShipmentInvoice.Id) AS int) AS id, EntryData.EntryData_Id, EntryData.InvoiceNo AS PONumber, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceNo
FROM   EntryDataEx as EntryData INNER JOIN
              ShipmentInvoiceEx as ShipmentInvoice                 
				  on ShipmentInvoice.InvoiceNo = EntryData.InvoiceNo
				  and EntryData.TotalLines = ShipmentInvoice.ImportedLines
				  and  abs(EntryData.ImportedTotal - ShipmentInvoice.SubTotal) < .01
				  AND EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId AND 
                 EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId LEFT OUTER JOIN
                 ShipmentInvoicePOManualMatches ON ShipmentInvoice.InvoiceNo = ShipmentInvoicePOManualMatches.InvoiceNo AND EntryData.InvoiceNo = ShipmentInvoicePOManualMatches.PONumber
WHERE (ShipmentInvoicePOManualMatches.Id IS NULL) 
GROUP BY EntryData.EntryData_Id, EntryData.InvoiceNo, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo


GO
