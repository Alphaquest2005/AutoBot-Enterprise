USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitAllXMLToCustoms]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[TODO-SubmitAllXMLToCustoms]
AS
SELECT ISNULL(CAST((row_number() OVER (ORDER BY AsycudaDocument.CNumber)) AS int), 0) as Id, AsycudaDocument.CNumber, AsycudaDocument.ASYCUDA_Id, AsycudaDocument.RegistrationDate, AsycudaDocument.Reference as ReferenceNumber, AsycudaDocument.AsycudaDocumentSetId, 
                 AsycudaDocument.DocumentType, AsycudaDocument.AssessmentDate, AsycudaDocument.ApplicationSettingsId, EntryDataDetailsEx.EmailId, AsycudaDocument.CustomsProcedure, Attachments.FilePath, 
                 AttachmentLog.Status, SystemDocumentSets.Id as SystemDocumentSetId, CASE WHEN IsPaid = 1 THEN 'Yes' ELSE 'No' END AS ToBePaid
FROM    AttachmentLog INNER JOIN
                 AsycudaDocumentSet_Attachments ON AttachmentLog.DocSetAttachment = AsycudaDocumentSet_Attachments.Id INNER JOIN
                 Attachments ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id RIGHT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails INNER JOIN
                 (SELECT EntryData.EmailId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryDataDetails.EntryDataDetailsId
FROM    EntryData INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id) as EntryDataDetailsEx ON AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = EntryDataDetailsEx.EntryDataDetailsId INNER JOIN
                 xcuda_Item ON AsycudaDocumentItemEntryDataDetails.Item_Id = xcuda_Item.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo as AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id ON Attachments.FilePath = AsycudaDocument.SourceFileName
				 left outer join SystemDocumentSets on EntryDataDetailsEx.AsycudaDocumentSetId = SystemDocumentSets.Id
				 
WHERE  (AsycudaDocument.ImportComplete = 1) AND (AsycudaDocument.SubmitToCustoms = 1) -- AND ((Attachments.FilePath IS NULL) or (AttachmentLog.Status <> N'Submit XML To Customs'))
GROUP BY EntryDataDetailsEx.EmailId, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.Reference, AsycudaDocument.AsycudaDocumentSetId, AsycudaDocument.DocumentType, 
                 AsycudaDocument.AssessmentDate, AsycudaDocument.ApplicationSettingsId, AsycudaDocument.Extended_customs_procedure, AsycudaDocument.ASYCUDA_Id, AttachmentLog.Status, Attachments.FilePath,AsycudaDocument.CustomsProcedure, SystemDocumentSets.Id
				 ,IsPaid
--IN ('9074-000', '4074-000', '3075-000', '7400-OPS', '4074-801','7400-OPP')
GO
