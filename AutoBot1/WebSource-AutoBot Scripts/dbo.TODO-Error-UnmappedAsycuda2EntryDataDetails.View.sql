USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-UnmappedAsycuda2EntryDataDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[TODO-Error-UnmappedAsycuda2EntryDataDetails]
as
SELECT AsycudaDocumentItem.Item_Id, AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.ItemQuantity, AsycudaDocumentItem.Commercial_Description, AsycudaDocumentItem.DPQtyAllocated, 
                 AsycudaDocumentItem.DFQtyAllocated, AsycudaDocumentItem.ImportComplete, AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.Extended_customs_procedure, 
                 AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.LineNumber, AsycudaDocumentItem.ReferenceNumber, AsycudaDocumentItem.PiQuantity, 
                 AsycudaDocumentItem.PreviousInvoiceNumber, AsycudaDocumentItem.PreviousInvoiceLineNumber, AsycudaDocumentItem.PreviousInvoiceItemNumber, AsycudaDocumentItem.ApplicationSettingsId, 
                 xcuda_Item.Free_text_1, xcuda_Item.Free_text_2
FROM    AsycudaDocumentBasicInfo INNER JOIN
                 AsycudaDocumentItem ON AsycudaDocumentBasicInfo.ASYCUDA_Id = AsycudaDocumentItem.AsycudaDocumentId INNER JOIN
                 Customs_Procedure ON AsycudaDocumentBasicInfo.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                 xcuda_Item ON AsycudaDocumentItem.Item_Id = xcuda_Item.Item_Id LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails ON AsycudaDocumentItem.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id
WHERE (ISNULL(AsycudaDocumentItem.Cancelled, 0) <> 1) AND (Customs_Procedure.CustomsOperationId = 2) AND (AsycudaDocumentItemEntryDataDetails.Item_Id IS NULL)
GO
