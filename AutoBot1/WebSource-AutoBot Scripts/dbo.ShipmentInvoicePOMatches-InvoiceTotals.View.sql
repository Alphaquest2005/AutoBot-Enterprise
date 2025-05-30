USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-InvoiceTotals]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[ShipmentInvoicePOMatches-InvoiceTotals]
AS

select * from 
(
SELECT  cast(row_number() OVER ( ORDER BY ShipmentInvoice.Id) AS int) AS id,EntryData.EntryData_Id, EntryData.InvoiceNo AS PONumber, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceNo
, ROW_NUMBER() over (partition by EntryData.EntryData_Id order by shipmentinvoice.invoicedate desc ) as rownum1
, ROW_NUMBER() over (partition by ShipmentInvoice.Id order by shipmentinvoice.invoicedate desc ) as rownum2
FROM    ShipmentPOs AS EntryData INNER JOIN
                 ShipmentInvoiceEx AS ShipmentInvoice ON ((
				 ABS(EntryData.SubTotal - ShipmentInvoice.SubTotal) < 2 OR
                 ABS(EntryData.InvoiceTotal - ShipmentInvoice.SubTotal) < 2 OR
                 ABS(EntryData.InvoiceTotal - ShipmentInvoice.InvoiceTotal) < 2 or
				 ABS(EntryData.SubTotal - ShipmentInvoice.InvoiceTotal) < 2
				 )) 
				 AND ABS(DATEDIFF(day, EntryData.emaildate, ShipmentInvoice.emaildate)) < 48 AND 
                 EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId  LEFT OUTER JOIN
                 ShipmentInvoicePOManualMatches ON ShipmentInvoice.InvoiceNo = ShipmentInvoicePOManualMatches.InvoiceNo AND EntryData.InvoiceNo = ShipmentInvoicePOManualMatches.PONumber
--WHERE (EntryData.InvoiceNo = '01937')
GROUP BY EntryData.EntryData_Id, EntryData.InvoiceNo, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo, ShipmentInvoice.InvoiceDate
) as data
where rownum1 = 1 and rownum2 = 1
GO
