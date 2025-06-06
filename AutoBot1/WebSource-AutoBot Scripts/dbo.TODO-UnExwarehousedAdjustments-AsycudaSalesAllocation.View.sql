USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UnExwarehousedAdjustments-AsycudaSalesAllocation]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-UnExwarehousedAdjustments-AsycudaSalesAllocation]
AS
SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItemsEx.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS Expr1, EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryDataDetails.Quantity, 
                 EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaSalesAllocations.xStatus, Emails.Subject, 
                 Emails.EmailDate, EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryDataDetails.LineNumber, 
                 CASE WHEN isnull(EntryDataDetails.TaxAmount, 0) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, EntryData.EmailId
FROM    ApplicationSettings INNER JOIN
                 EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItemsEx ON ApplicationSettings.ApplicationSettingsId = InventoryItemsEx.ApplicationSettingsId INNER JOIN
                 EntryDataDetails ON InventoryItemsEx.ItemNumber = EntryDataDetails.ItemNumber INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id INNER JOIN
                 AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
                 Emails ON EntryData.EmailId = Emails.EmailId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails ON EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL)
GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItemsEx.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaSalesAllocations.xStatus, Emails.Subject, Emails.EmailDate, EntryDataDetails.ItemDescription, 
                 EntryDataDetails.Cost, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryDataDetails.LineNumber, EntryDataDetails.TaxAmount, EntryData.EmailId, 
                 EntryDataDetails.Quantity
GO
