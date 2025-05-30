USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-Perfect]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ShipmentInvoicePOMatches-Perfect]
AS


SELECT cast(row_number() OVER ( ORDER BY ShipmentInvoice.Id) AS int) AS id, EntryData.EntryData_Id, EntryData.InvoiceNo AS PONumber, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceNo
FROM   EntryDataEx as EntryData INNER JOIN
              ShipmentInvoiceEx as ShipmentInvoice LEFT OUTER JOIN
                 ShipmentInvoiceExtraInfo ON ShipmentInvoice.Id = ShipmentInvoiceExtraInfo.InvoiceId ON (trim(ShipmentInvoiceExtraInfo.Value) LIKE N'%' + CASE WHEN trim(EntryData.InvoiceNo) 
                 = '' THEN 'unknown' ELSE trim(EntryData.InvoiceNo) END + N'%'                   
				  AND EntryData.TotalLines = ShipmentInvoice.ImportedLines
				  and  abs(EntryData.ImportedTotal - ShipmentInvoice.SubTotal) < .01
				  AND EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId) AND 
                 EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId LEFT OUTER JOIN
                 ShipmentInvoicePOManualMatches ON ShipmentInvoice.InvoiceNo = ShipmentInvoicePOManualMatches.InvoiceNo AND EntryData.InvoiceNo = ShipmentInvoicePOManualMatches.PONumber
WHERE (ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber') AND (ShipmentInvoicePOManualMatches.Id IS NULL) OR
                 (ShipmentInvoiceExtraInfo.Info IS NULL)
GROUP BY EntryData.EntryData_Id, EntryData.InvoiceNo, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo


GO
