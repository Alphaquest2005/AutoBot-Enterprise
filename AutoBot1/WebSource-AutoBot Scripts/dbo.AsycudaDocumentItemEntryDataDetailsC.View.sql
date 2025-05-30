USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetailsC]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetailsC]
 
AS
--- don't consider quantity because of lumped items
--- this ignore the linenumber incase its wrong 
SELECT        AsycudaDocumentBasicInfo.AsycudaDocumentSetId, AsycudaDocumentBasicInfo.ApplicationSettingsId, EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, xcuda_Item.Item_Id, EntryDataDetails.ItemNumber, 
                         EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) AS [key], AsycudaDocumentBasicInfo.DocumentType, 
                         AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + AsycudaDocumentBasicInfo.National_customs_procedure AS CustomsProcedure, Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, 
                         AsycudaDocumentBasicInfo.ImportComplete, xcuda_Item.ASYCUDA_Id, EntryDataTypes.Type AS EntryDataType, xcuda_HScode.Commodity_code AS Tariffcode, AsycudaDocumentBasicInfo.CNumber, xcuda_Item.LineNumber, CustomsOperation
FROM            CustomsProcedureEntryDataTypes INNER JOIN
                         EntryDataTypes WITH (NOLOCK) ON CustomsProcedureEntryDataTypes.EntryDataType = EntryDataTypes.Type RIGHT OUTER JOIN
                         EntryDataDetails WITH (NOLOCK) INNER JOIN
                         xcuda_Item WITH (NOLOCK) ON xcuda_Item.PreviousInvoiceKey <> EntryDataDetails.EntryDataDetailsKey AND xcuda_Item.PreviousInvoiceItemNumber = EntryDataDetails.ItemNumber AND 
                         xcuda_Item.PreviousInvoiceNumber = EntryDataDetails.EntryDataId INNER JOIN
                         AsycudaDocumentBasicInfo WITH (NOLOCK) ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                         Primary_Supplementary_Unit WITH (NOLOCK) ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id INNER JOIN
                         xcuda_HScode WITH (NOLOCK) ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id ON CustomsProcedureEntryDataTypes.CustomsProcedure = AsycudaDocumentBasicInfo.CustomsProcedure AND 
                         EntryDataTypes.EntryData_Id = EntryDataDetails.EntryData_Id
WHERE        ((CHARINDEX(xcuda_Item.PreviousInvoiceItemNumber, EntryDataDetails.ItemNumber) > 0) or CHARINDEX(xcuda_HScode.Precision_4, EntryDataDetails.ItemNumber)> 0)  and EntryDataDetails.Quantity = Primary_Supplementary_Unit.Suppplementary_unit_quantity  and  isnull(AsycudaDocumentBasicInfo.Cancelled, 0) = 0


--SELECT EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, xcuda_Item.Item_Id, EntryDataDetails.ItemNumber, 
--                 EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) AS [key], AsycudaDocumentBasicInfo.DocumentType, 
--                 AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + AsycudaDocumentBasicInfo.National_customs_procedure AS CustomsProcedure, 
--                 Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, AsycudaDocumentBasicInfo.ImportComplete, AsycudaDocumentBasicInfo.ApplicationSettingsId
--FROM    EntryDataDetails INNER JOIN  xcuda_Item ON
				
--				 EntryDataDetails.ItemNumber = xcuda_Item.PreviousInvoiceItemNumber 	AND
--				   EntryDataDetails.EntryDataId =  xcuda_Item.PreviousInvoiceNumber
--				 AND REPLACE(EntryDataDetails.EntryDataId, ' ', '') + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) <> REPLACE(xcuda_Item.PreviousInvoiceKey, ' ', '')
--				 INNER JOIN
--                 AsycudaDocumentBasicInfo ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
--                 Primary_Supplementary_Unit ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id
--				 --AND EntryDataDetails.Quantity = Primary_Supplementary_Unit.Suppplementary_unit_quantity
GO
