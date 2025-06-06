USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSet-Old]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[TODO-PODocSet-Old]
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
WHERE  --(AsycudaDocumentSetEx.TotalInvoices > 0)  AND([TODO-SubmitInadequatePackages].RequiredPackages IS NULL) AND (AsycudaDocumentSetEx.BLNumber IS NOT NULL) AND (AsycudaDocumentSetEx.BLNumber <> '')
--AND   (AsycudaDocumentSetEx.Currency_Code <> '') AND (AsycudaDocumentSetEx.Country_of_origin_code IS NOT NULL)AND (AsycudaDocumentSetEx.Currency_Code IS NOT NULL)
                /* AND (AsycudaDocumentSetEx.Manifest_Number IS NOT NULL) AND (AsycudaDocumentSetEx.Manifest_Number <> '') */
            --    AND
				(ISNULL(AsycudaDocumentItemEntryDataDetails.ImportComplete, 0) = 0) AND 
                 (NOT (AsycudaDocumentSetEx.AsycudaDocumentSetId IN
                     (SELECT xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
                      FROM     xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                                       [TODO-DocumentsToDelete] ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = [TODO-DocumentsToDelete].ASYCUDA_Id
                      GROUP BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId))) AND (SystemDocumentSets.Id IS NULL)
GROUP BY AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount, AsycudaDocumentItemEntryDataDetails.ImportComplete, 
                 AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalFreight, 
                 AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines, AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages
				 , AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, AsycudaDocumentSetEx.FreightCurrencyRate, AsycudaDocumentsetEx.ExpectedEntries
GO
