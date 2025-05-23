USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO_ErrorUnknownAsycudaDocumentItemEntryDataDetails]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO_ErrorUnknownAsycudaDocumentItemEntryDataDetails]
 
AS
SELECT   dbo.xcuda_Item.Item_Id, dbo.xcuda_Item.PreviousInvoiceItemNumber,dbo.xcuda_Item.PreviousInvoiceLineNumber ,dbo.xcuda_Item.PreviousInvoiceNumber + '|' + RTRIM(LTRIM(CAST(dbo.xcuda_Item.PreviousInvoiceLineNumber AS varchar(50)))) 
                 AS [key], dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + dbo.AsycudaDocumentBasicInfo.National_customs_procedure as CustomsProcedure  , dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, dbo.AsycudaDocumentBasicInfo.ImportComplete, xcuda_item.ASYCUDA_Id,
				 dbo.AsycudaDocumentBasicInfo.AssessmentDate 
FROM    dbo.EntryDataDetails  right outer JOIN
                 dbo.xcuda_Item  ON 
				 replace(dbo.xcuda_Item.PreviousInvoiceNumber,' ', '') + '|' + replace(dbo.xcuda_Item.PreviousInvoiceLineNumber,' ', '') =  replace(dbo.EntryDataDetails.EntryDataId,' ', '') + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50))))  and EntryDataDetails.ItemNumber =  xcuda_Item.PreviousInvoiceItemNumber  INNER JOIN
                 dbo.AsycudaDocumentBasicInfo  ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.Primary_Supplementary_Unit ON dbo.xcuda_Item.Item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id
where EntryDataDetails.EntryDataDetailsId is null and xcuda_Item.PreviousInvoiceItemNumber is not null
--where entrydataid like '%122089%' 
GO
