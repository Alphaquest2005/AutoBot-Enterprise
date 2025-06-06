USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AllAdjustmentsAlreadyXMLed-Old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[TODO-AllAdjustmentsAlreadyXMLed-Old]
AS
SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItemsEx.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, 
                 EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, 
                 AsycudaDocumentItem.CNumber AS pCNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.ReferenceNumber, AsycudaDocumentItemEntryDataDetails.DocumentType, Emails.Subject,
				 emails.EmailId
FROM    SystemDocumentSets INNER JOIN
                 ApplicationSettings INNER JOIN
                 EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItemsEx ON ApplicationSettings.ApplicationSettingsId = InventoryItemsEx.ApplicationSettingsId INNER JOIN
                 EntryDataDetails ON InventoryItemsEx.ItemNumber = EntryDataDetails.ItemNumber INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id ON 
                 SystemDocumentSets.Id = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 Emails ON EntryData.EmailId = Emails.EmailUniqueId LEFT OUTER JOIN
                 AsycudaDocumentItem INNER JOIN
                 AsycudaDocumentItemEntryDataDetails ON AsycudaDocumentItem.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id ON 
                 EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NOT NULL) AND (AsycudaDocumentItemEntryDataDetails.CustomsProcedure not in ('7400-000','4000-000')) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NOT NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND (AsycudaDocumentItemEntryDataDetails.ImportComplete = 1)
GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, dbo.InventoryItemsEx.TariffCode,
				EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, 
                 AsycudaDocumentItem.ReferenceNumber, AsycudaDocumentItemEntryDataDetails.DocumentType, Emails.Subject, emails.EmailId
GO
