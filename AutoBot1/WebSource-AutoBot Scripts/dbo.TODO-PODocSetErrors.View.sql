USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetErrors]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-PODocSetErrors]
AS
SELECT DISTINCT 
                 AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount,  AsycudaDocumentsetEx.ExpectedEntries, AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, 
                 AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.TotalFreight, AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines,
				 AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages, AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, AsycudaDocumentSetEx.FreightCurrencyRate
FROM    [TODO-SubmitInadequatePackages] WITH (NOLOCK) RIGHT OUTER JOIN
                 SystemDocumentSets WITH (NOLOCK) RIGHT OUTER JOIN
                 AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEx WITH (NOLOCK) INNER JOIN
                 Document_Type WITH (NOLOCK) ON AsycudaDocumentSetEx.Document_TypeId = Document_Type.Document_TypeId ON 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId ON SystemDocumentSets.Id = AsycudaDocumentSetEx.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails WITH (NOLOCK) ON AsycudaDocumentItemEntryDataDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id and AsycudaDocumentItemEntryDataDetails.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId ON 
                 [TODO-SubmitInadequatePackages].AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSet_Attachments WITH (NOLOCK) ON AsycudaDocumentSetEx.AsycudaDocumentSetId = AsycudaDocumentSet_Attachments.AsycudaDocumentSetId LEFT OUTER JOIN
                 Attachments WITH (NOLOCK) ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id
WHERE (AsycudaDocumentSetEx.TotalInvoices > 0) 
or ([TODO-SubmitInadequatePackages].RequiredPackages IS not NULL)
or (AsycudaDocumentSetEx.BLNumber IS  NULL)
or (AsycudaDocumentSetEx.BLNumber = '') 
               or (AsycudaDocumentSetEx.Currency_Code IS NULL)
			   or 
                 (AsycudaDocumentSetEx.Currency_Code ='') 
				 or (AsycudaDocumentSetEx.Country_of_origin_code IS NULL) 
				 or (ISNULL(AsycudaDocumentItemEntryDataDetails.ImportComplete, 0) <> 0) 
GROUP BY AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount, AsycudaDocumentItemEntryDataDetails.ImportComplete, 
                 AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalFreight, 
                 AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines, AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages
				 , AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, AsycudaDocumentSetEx.FreightCurrencyRate, AsycudaDocumentsetEx.ExpectedEntries
GO
