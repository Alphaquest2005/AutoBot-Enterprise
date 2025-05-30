USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetailsB]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetailsB]
 
AS
SELECT EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, xcuda_Item.Item_Id, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) AS [key], AsycudaDocumentBasicInfo.DocumentType, 
                 AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + AsycudaDocumentBasicInfo.National_customs_procedure AS CustomsProcedure, 
                 Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, AsycudaDocumentBasicInfo.ImportComplete, AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    EntryDataDetails INNER JOIN
                 xcuda_Item ON EntryDataDetails.ItemNumber = xcuda_Item.Free_text_2 AND REPLACE(xcuda_Item.Free_text_1, ' ', '') LIKE REPLACE(EntryDataDetails.EntryDataId, ' ', '') + '|%' AND 
                 REPLACE(EntryDataDetails.EntryDataId, ' ', '') + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) <> REPLACE(xcuda_Item.Free_text_1, ' ', '') INNER JOIN
                 AsycudaDocumentBasicInfo ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id AND EntryDataDetails.Quantity = Primary_Supplementary_Unit.Suppplementary_unit_quantity
GO
