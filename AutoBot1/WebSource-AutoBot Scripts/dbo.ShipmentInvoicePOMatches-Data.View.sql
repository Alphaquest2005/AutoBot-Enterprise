USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-Data]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ShipmentInvoicePOMatches-Data]
AS


SELECT EntryData.EntryData_Id, EntryData.InvoiceNo AS PONumber, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceNo,EntryData.InvoiceDate,  ROW_NUMBER() over (partition by  ShipmentInvoice.InvoiceNo order by EntryData.InvoiceDate desc) as RowNumber
FROM   shipmentPOs as EntryData INNER JOIN
              ShipmentInvoiceEx as ShipmentInvoice on EntryData.ImportedLine = ShipmentInvoice.ImportedLines
				  --and  abs(EntryData.ImportedTotal - ShipmentInvoice.SubTotal) <5
				  AND EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId AND 
                 EntryData.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId LEFT OUTER JOIN
                 ShipmentInvoiceExtraInfo ON ShipmentInvoice.Id = ShipmentInvoiceExtraInfo.InvoiceId       
				   LEFT OUTER JOIN
                 ShipmentInvoicePOManualMatches ON ShipmentInvoice.InvoiceNo = ShipmentInvoicePOManualMatches.InvoiceNo AND EntryData.InvoiceNo = ShipmentInvoicePOManualMatches.PONumber
WHERE (((ShipmentInvoiceExtraInfo.Info IS not NULL) and (ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber')
	     AND(((trim(entrydata.InvoiceNo) <> trim(ShipmentInvoice.InvoiceNo)) and (trim(ShipmentInvoiceExtraInfo.Value) LIKE N'%' + CASE WHEN trim(EntryData.InvoiceNo) 
                 = '' THEN 'unknown' ELSE trim(EntryData.InvoiceNo) END + N'%') )) OR (ShipmentInvoiceExtraInfo.Info IS NULL))) AND (ShipmentInvoicePOManualMatches.Id IS NULL) 

GROUP BY EntryData.EntryData_Id, EntryData.InvoiceNo, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo, EntryData.InvoiceDate


GO
