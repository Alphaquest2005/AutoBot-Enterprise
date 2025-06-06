USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71FromEntryDataErrors]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-C71FromEntryDataErrors]
AS
SELECT 'FOB' AS Code, 0 AS ASYCUDA_Id, dbo.AsycudaDocumentSetEx.ApplicationSettingsId, ISNULL(dbo.EntryDataEx.InvoiceTotal, 0) AS InvoiceTotal, dbo.EntryDataEx.ExpectedTotal, dbo.EntryDataEx.Type, 
                 dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, ISNULL(dbo.EntryDataEx.Currency, dbo.[TODO-PODocSet].Currency_Code) AS Currency, dbo.EntryDataEx.SupplierCode, 
                 dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId, case when dbo.AsycudaDocumentEntryData.Id IS not NULL then 'Already has attached C71' when dbo.AsycudaDocumentSetEx.ImportedInvoices <> dbo.AsycudaDocumentSetEx.TotalInvoices then 'Imported Invoices and Total Expected Invoices are different' else null end as status
FROM    dbo.AsycudaDocumentSetEntryData INNER JOIN
                 dbo.AsycudaDocumentSetEx INNER JOIN
                 dbo.[TODO-PODocSet] ON dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId = dbo.[TODO-PODocSet].AsycudaDocumentSetId INNER JOIN
                 dbo.EntryDataEx ON dbo.AsycudaDocumentSetEx.ApplicationSettingsId = dbo.EntryDataEx.ApplicationSettingsId ON 
                 dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId AND 
                 dbo.AsycudaDocumentSetEntryData.EntryData_Id = dbo.EntryDataEx.EntryData_Id LEFT OUTER JOIN
                 dbo.AsycudaDocumentEntryData ON dbo.EntryDataEx.EntryData_Id = dbo.AsycudaDocumentEntryData.EntryData_Id
WHERE (dbo.AsycudaDocumentEntryData.Id IS not NULL) or (dbo.AsycudaDocumentSetEx.ImportedInvoices <> dbo.AsycudaDocumentSetEx.TotalInvoices)
GROUP BY dbo.AsycudaDocumentSetEx.ApplicationSettingsId,dbo.EntryDataEx.InvoiceTotal, dbo.EntryDataEx.Type, dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, dbo.EntryDataEx.Currency, 
                 dbo.[TODO-PODocSet].Currency_Code, dbo.EntryDataEx.SupplierCode, dbo.EntryDataEx.ExpectedTotal, dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId,AsycudaDocumentEntryData.Id,AsycudaDocumentSetEx.ImportedInvoices,
				 AsycudaDocumentSetEx.TotalInvoices
GO
