USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentsToXMLDataAdj]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-AdjustmentsToXMLDataAdj]
AS
SELECT TOP (100) PERCENT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                                                    CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
                                                    EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, 
                                                    AsycudaDocumentSet.Declarant_Reference_Number
                                   FROM     ApplicationSettings INNER JOIN
                                                    EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                                                    AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                                                    AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                                                    AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                                                    InventoryItems ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                                                    EntryDataDetails ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                                                    EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                                                    AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                                                    SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
                                   WHERE  (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND (ISNULL(EntryDataDetails.DoNotAllocate, 0) <> 1) AND (ISNULL(EntryDataDetails.IsReconciled, 0) <> 1) AND 
                                                    (AsycudaSalesAllocations.xStatus IS NULL)
                                   GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, dbo.InventoryItems.TariffCode,
													EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                                                    EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
                                   ORDER BY EntryDataDetails.EntryDataDetailsId
GO
