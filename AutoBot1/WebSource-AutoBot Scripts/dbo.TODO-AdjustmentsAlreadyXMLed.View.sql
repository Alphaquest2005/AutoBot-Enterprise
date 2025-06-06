USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentsAlreadyXMLed]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-AdjustmentsAlreadyXMLed]
AS

SELECT EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                 CAST(CASE WHEN dbo.InventoryItemsEx.TariffCode IS NULL THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
                 EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber AS PreviousCNumber, 
                 AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentBasicInfo.CNumber AS pCNumber, xcuda_Item.LineNumber as pLineNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.Reference as ReferenceNumber, 
                 AID.DocumentType, EntryData_Adjustments.Type, EntryDataDetails.EffectiveDate, EntryDataDetails.ItemDescription, EntryData.EmailId, EntryData.FileTypeId, 
                 EntryDataDetails.Quantity, EntryDataDetails.Cost, Emails.Subject, Emails.EmailDate, EntryDataDetails.LineNumber, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.Comment, 
                 EntryDataDetailsEx.DutyFreePaid
FROM    EntryDataDetailsEx INNER JOIN
                 ApplicationSettings INNER JOIN
                 EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItemsEx ON ApplicationSettings.ApplicationSettingsId = InventoryItemsEx.ApplicationSettingsId INNER JOIN
                 EntryDataDetails ON InventoryItemsEx.InventoryItemId = EntryDataDetails.InventoryItemId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id INNER JOIN
                 Emails ON EntryData.EmailId = Emails.EmailId ON EntryDataDetailsEx.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                 EntryDataDetailsEx.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId AND EntryDataDetailsEx.EntryData_Id = EntryData.EntryData_Id LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                 AsycudaDocumentBasicInfo INNER JOIN
				 xcuda_item inner join
                 --(select * from AsycudaDocumentItemEntryDataDetails where (AsycudaDocumentItemEntryDataDetails.DocumentType <> N'EX9') AND (AsycudaDocumentItemEntryDataDetails.DocumentType <> N'IM7') ) as AsycudaDocumentItemEntryDataDetails
				 (select ab.* from dbo.AsycudaDocumentItemEntryDataDetails as ab
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = ab.ASYCUDA_Id
				 where (AsycudaDocumentCustomsProcedures.Discrepancy = 1) ) as AID
				 
				 ON xcuda_Item.Item_Id= AID.Item_Id
				 on AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id
                  ON EntryDataDetails.EntryDataDetailsId = AID.EntryDataDetailsId AND EntryDataDetailsEx.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId
WHERE (AID.EntryDataDetailsId IS NOT NULL) AND (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND 
                 (EntryDataDetails.Status IS NULL)-- AND (AID.ImportComplete = 1)
GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId,  dbo.InventoryItemsEx.TariffCode, EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, 
                 AsycudaDocumentBasicInfo.Reference, AID.DocumentType, EntryDataDetails.EffectiveDate, EntryDataDetails.ItemDescription, EntryData.EmailId, EntryData.FileTypeId, 
                 EntryDataDetails.Quantity, EntryDataDetails.Cost, Emails.Subject, Emails.EmailDate, EntryDataDetails.LineNumber, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.Comment, 
                 EntryDataDetailsEx.DutyFreePaid, EntryDataDetails.EntryData_Id, xcuda_Item.LineNumber
GO
