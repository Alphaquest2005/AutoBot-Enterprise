USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetAttachements-EntryData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-PODocSetAttachements-EntryData]
AS
SELECT AsycudaDocument_Attachments.AsycudaDocumentId, Attachments.DocumentCode, xcuda_Attached_documents.Item_Id, AsycudaDocument_Attachments.Id AS AttachementId, 
                 AsycudaDocumentEntryData.EntryData_Id, xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, Attachments.FilePath
FROM    xcuda_Item INNER JOIN
                 xcuda_Attached_documents ON xcuda_Item.Item_Id = xcuda_Attached_documents.Item_Id INNER JOIN
                 AsycudaDocument_Attachments ON xcuda_Item.ASYCUDA_Id = AsycudaDocument_Attachments.AsycudaDocumentId INNER JOIN
                 Attachments ON Attachments.Id = AsycudaDocument_Attachments.AttachmentId AND xcuda_Attached_documents.Attached_document_reference = Attachments.Reference INNER JOIN
                 AsycudaDocumentEntryData ON AsycudaDocument_Attachments.AsycudaDocumentId = AsycudaDocumentEntryData.AsycudaDocumentId INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 EntryData ON AsycudaDocumentEntryData.EntryData_Id = EntryData.EntryData_Id
WHERE (Attachments.DocumentCode IN ('IV05', 'BL10', 'IV04')) AND (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0) and replace(Attachments.FilePath,'.pdf','.csv') = EntryData.SourceFile
GROUP BY AsycudaDocument_Attachments.AsycudaDocumentId, Attachments.DocumentCode, xcuda_Attached_documents.Item_Id, AsycudaDocument_Attachments.Id, AsycudaDocumentEntryData.EntryData_Id, 
                 xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, Attachments.FilePath
GO
