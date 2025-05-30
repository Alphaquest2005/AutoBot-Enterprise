USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitMissingInvoicePDFs-Old]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-SubmitMissingInvoicePDFs-Old]
AS
SELECT EntryDataEx.EntryDataId as InvoiceNo, EntryDataEx.SourceFile, dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.AsycudaDocumentSet.ApplicationSettingsId, dbo.AsycudaDocumentSet.Declarant_Reference_Number, 
                 EntryDataEx.EmailId
FROM   EntryData as EntryDataEx INNER JOIN
                 dbo.AsycudaDocumentSetEntryData ON EntryDataEx.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
				  AsycudaDocumentSet on dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId inner join
                 dbo.[TODO-PODocSet-Criteria] ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.[TODO-PODocSet-Criteria].AsycudaDocumentSetId LEFT OUTER JOIN
                 dbo.[TODO-PODocSetAttachements] ON dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId = dbo.[TODO-PODocSetAttachements].AsycudaDocumentSetId AND 
                 dbo.AsycudaDocumentSetEntryData.EntryData_Id = dbo.[TODO-PODocSetAttachements].EntryData_Id
WHERE (dbo.[TODO-PODocSetAttachements].EntryData_Id IS NULL)
GROUP BY EntryDataex.EntryDataId, EntryDataEx.SourceFile, dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.AsycudaDocumentSet.ApplicationSettingsId, dbo.AsycudaDocumentSet.Declarant_Reference_Number, 
                 EntryDataEx.EmailId
GO
