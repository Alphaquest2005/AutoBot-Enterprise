USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetails-backup -because i reimported items with changed linenumbers DO NOT DELETE]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetails-backup -because i reimported items with changed linenumbers DO NOT DELETE]
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId, dbo.xcuda_Item.Item_Id, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50)))) 
                 AS [key], dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, dbo.AsycudaDocumentBasicInfo.ImportComplete
FROM    dbo.EntryDataDetails WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item WITH (NOLOCK) ON dbo.EntryDataDetails.ItemNumber = dbo.xcuda_Item.Free_text_2 AND 
                 dbo.EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50)))) = dbo.xcuda_Item.Free_text_1 INNER JOIN
                 dbo.AsycudaDocumentBasicInfo WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.Primary_Supplementary_Unit WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id
GO
