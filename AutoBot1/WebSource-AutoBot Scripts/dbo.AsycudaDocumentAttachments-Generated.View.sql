USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentAttachments-Generated]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentAttachments-Generated]
AS
SELECT TOP (100) PERCENT AsycudaDocumentBasicInfo.ASYCUDA_Id, AsycudaDocumentBasicInfo.Reference AS ReferenceNumber, xcuda_Attached_documents.Attached_document_code AS DocumentCode, 
                 xcuda_Attached_documents.Attached_document_reference AS Reference, AsycudaDocumentBasicInfo.ApplicationSettingsId, AsycudaDocumentBasicInfo.AsycudaDocumentSetId
FROM    AsycudaDocumentBasicInfo INNER JOIN
                 xcuda_Attached_documents INNER JOIN
                 xcuda_Item ON xcuda_Attached_documents.Item_Id = xcuda_Item.Item_Id ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id
GO
