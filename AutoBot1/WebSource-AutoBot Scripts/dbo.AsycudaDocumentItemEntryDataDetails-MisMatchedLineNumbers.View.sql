USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetails-MisMatchedLineNumbers]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetails-MisMatchedLineNumbers]
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId, dbo.xcuda_Item.Item_Id, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50)))) 
                 AS [key], dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, dbo.AsycudaDocumentBasicInfo.ImportComplete
FROM    dbo.EntryDataDetails WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item WITH (NOLOCK) ON dbo.EntryDataDetails.ItemNumber = dbo.xcuda_Item.Free_text_2 AND 
                 dbo.xcuda_Item.Free_text_1  like dbo.EntryDataDetails.EntryDataId + '|%' and dbo.EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50)))) <> dbo.xcuda_Item.Free_text_1 INNER JOIN
                 dbo.AsycudaDocumentBasicInfo WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.Primary_Supplementary_Unit WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id and EntryDataDetails.Quantity = dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity
GO
