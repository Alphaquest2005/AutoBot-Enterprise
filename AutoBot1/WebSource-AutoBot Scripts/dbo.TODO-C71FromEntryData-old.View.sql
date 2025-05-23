USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71FromEntryData-old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-C71FromEntryData-old]
AS
SELECT 'FOB' AS Code, 0 AS ASYCUDA_Id, AsycudaDocumentSetEx.ApplicationSettingsId, ISNULL(EntryDataEx.InvoiceTotal, 0) AS InvoiceTotal, EntryDataEx.ExpectedTotal, EntryDataEx.Type, EntryDataEx.InvoiceDate, 
                 EntryDataEx.InvoiceNo, ISNULL(EntryDataEx.Currency, [TODO-PODocSet].Currency_Code) AS Currency, EntryDataEx.SupplierCode, AsycudaDocumentSetEx.AsycudaDocumentSetId, 
                 AsycudaDocumentSetEx.CurrencyRate
FROM    AsycudaDocumentSetEntryData INNER JOIN
                 AsycudaDocumentSetEx INNER JOIN
                 [TODO-PODocSet] ON AsycudaDocumentSetEx.AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId INNER JOIN
                 EntryDataEx ON AsycudaDocumentSetEx.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId AND
                  AsycudaDocumentSetEntryData.EntryData_Id = EntryDataEx.EntryData_Id LEFT OUTER JOIN
                 AsycudaDocumentEntryData ON EntryDataEx.EntryData_Id = AsycudaDocumentEntryData.EntryData_Id inner join 
				 Suppliers on entrydataex.SupplierCode = Suppliers.SupplierCode
WHERE (AsycudaDocumentEntryData.Id IS NULL) AND (AsycudaDocumentSetEx.ImportedInvoices = AsycudaDocumentSetEx.TotalInvoices) and Suppliers.CountryCode is not null
GROUP BY AsycudaDocumentSetEx.ApplicationSettingsId, EntryDataEx.InvoiceTotal, EntryDataEx.Type, EntryDataEx.InvoiceDate, EntryDataEx.InvoiceNo, EntryDataEx.Currency, 
                 [TODO-PODocSet].Currency_Code, EntryDataEx.SupplierCode, EntryDataEx.ExpectedTotal, AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.CurrencyRate
GO
