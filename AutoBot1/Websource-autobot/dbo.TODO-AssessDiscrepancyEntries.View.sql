USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AssessDiscrepancyEntries]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-AssessDiscrepancyEntries]
AS
SELECT DISTINCT 
                 ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
                 EntryData.EntryDataDate AS InvoiceDate, AsycudaDocumentSet.Declarant_Reference_Number
FROM    ApplicationSettings INNER JOIN
                 EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id INNER JOIN
                 EntryDataDetails ON EntryData_Adjustments.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Item INNER JOIN
                     (SELECT ApplicationSettingsId, AsycudaDocumentSetId, EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, CustomsProcedure, Quantity, ImportComplete, Asycuda_id, 
                                       EntryDataType
                      FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
                      WHERE  (ImportComplete = 0)) AS ToBeExecutions ON xcuda_Item.Item_Id = ToBeExecutions.Item_Id ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON 
                 EntryDataDetails.EntryDataDetailsId = ToBeExecutions.EntryDataDetailsId LEFT OUTER JOIN
                     (SELECT ApplicationSettingsId, AsycudaDocumentSetId, EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, CustomsProcedure, Quantity, ImportComplete, Asycuda_id, 
                                       EntryDataType
                      FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
                      WHERE  (ImportComplete = 1)) AS Executions ON EntryDataDetails.EntryDataDetailsId = Executions.EntryDataDetailsId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (Executions.EntryDataDetailsId IS NULL) AND (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0) AND (EntryData_Adjustments.Type IS NOT NULL) AND (SystemDocumentSets.Id IS NULL)
GROUP BY ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryData_Adjustments.Type, EntryData.EntryDataId, EntryData.EntryDataDate, 
                 AsycudaDocumentSet.Declarant_Reference_Number, SystemDocumentSets.Id
GO
