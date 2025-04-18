USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetAttachements-Basic]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[TODO-PODocSetAttachements-Basic]
AS
SELECT AsycudaDocument_Attachments.AsycudaDocumentId, Attachments.DocumentCode, xcuda_Attached_documents.Item_Id, AsycudaDocument_Attachments.Id AS AttachementId
FROM    xcuda_Item INNER JOIN
                 xcuda_Attached_documents ON xcuda_Item.Item_Id = xcuda_Attached_documents.Item_Id INNER JOIN
                 AsycudaDocument_Attachments ON xcuda_Item.ASYCUDA_Id = AsycudaDocument_Attachments.AsycudaDocumentId INNER JOIN
                 Attachments ON Attachments.Id = AsycudaDocument_Attachments.AttachmentId AND xcuda_Attached_documents.Attached_document_reference = Attachments.Reference
WHERE (Attachments.DocumentCode IN ('IV05', 'BL10', 'IV04'))
GROUP BY AsycudaDocument_Attachments.AsycudaDocumentId, Attachments.DocumentCode, xcuda_Attached_documents.Item_Id, AsycudaDocument_Attachments.Id
GO
