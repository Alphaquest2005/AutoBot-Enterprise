USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71ToCreate-PODocSet]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-C71ToCreate-PODocSet]
AS
SELECT AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount, AsycudaDocumentSetEx.ExpectedEntries, 
                 AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.TotalFreight, 
                 AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines, AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages, 
                 AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, AsycudaDocumentSetEx.FreightCurrencyRate
FROM    AsycudaDocumentEntryData RIGHT OUTER JOIN
                 AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEx WITH (NOLOCK) INNER JOIN
                 Document_Type WITH (NOLOCK) ON AsycudaDocumentSetEx.Document_TypeId = Document_Type.Document_TypeId ON 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId ON 
                 AsycudaDocumentEntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 AsycudaDocumentBasicInfo ON AsycudaDocumentEntryData.AsycudaDocumentId = AsycudaDocumentBasicInfo.ASYCUDA_Id AND 
                 AsycudaDocumentSetEx.AsycudaDocumentSetId = AsycudaDocumentBasicInfo.AsycudaDocumentSetId LEFT OUTER JOIN
                 SystemDocumentSets WITH (NOLOCK) ON AsycudaDocumentSetEx.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (SystemDocumentSets.Id IS NULL) AND (ISNULL(AsycudaDocumentBasicInfo.ImportComplete, 0) = 0)
GROUP BY AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount, AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, 
                 AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalFreight, AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines, 
                 AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages, AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, 
                 AsycudaDocumentSetEx.FreightCurrencyRate, AsycudaDocumentSetEx.ExpectedEntries
GO
