USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-Totals]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[ShipmentInvoicePOMatches-Totals]
AS
select * from
(
SELECT cast(row_number() OVER ( ORDER BY ShipmentInvoice.Id) AS int) AS id, EntryData.EntryData_Id, EntryData.InvoiceNo AS PONumber, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceNo, ROW_NUMBER() over (partition by EntryData.EntryData_Id order by shipmentinvoice.invoicedate desc, ABS(EntryData.SubTotal - ShipmentInvoice.SubTotal) ) as rownum1
FROM   shipmentpos as EntryData INNER JOIN
              ShipmentInvoiceEx as ShipmentInvoice LEFT OUTER JOIN
                 ShipmentInvoiceExtraInfo ON ShipmentInvoice.Id = ShipmentInvoiceExtraInfo.InvoiceId ON (                 
				   EntryData.ImportedLine = ShipmentInvoice.ImportedLines
				  and  abs(EntryData.SubTotal - ShipmentInvoice.SubTotal) < 1
				  AND EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId) AND 
                 EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId LEFT OUTER JOIN
                 ShipmentInvoicePOManualMatches ON ShipmentInvoice.InvoiceNo = ShipmentInvoicePOManualMatches.InvoiceNo AND EntryData.InvoiceNo = ShipmentInvoicePOManualMatches.PONumber
WHERE ((ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber') AND (ShipmentInvoicePOManualMatches.Id IS NULL) OR
                 (ShipmentInvoiceExtraInfo.Info IS NULL)) --and entrydata.InvoiceNo  = '01937' 
		and (abs(datediff(hh,entrydata.emaildate, shipmentinvoice.emaildate)) < 48)
GROUP BY EntryData.EntryData_Id, EntryData.InvoiceNo, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo, ShipmentInvoice.SubTotal, EntryData.SubTotal, ShipmentInvoice.InvoiceDate 
--order by abs(EntryData.SubTotal - ShipmentInvoice.SubTotal)
) as data
where rownum1 = 1
GO
