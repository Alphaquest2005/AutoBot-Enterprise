USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentDetails-EntryDataDetailsEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[AdjustmentDetails-EntryDataDetailsEx]
AS
SELECT EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, EntryDataDetails.EntryDataId, EntryDataDetails.LineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity, EntryDataDetails.Units, 
                 EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.QtyAllocated, EntryDataDetails.VolumeLiters, EntryDataDetails.UnitWeight, EntryDataDetails.DoNotAllocate, InventoryItems.TariffCode, 
                 xcuda_Registration.Number AS CNumber, xcuda_Item.LineNumber AS CLineNumber, CAST(CASE WHEN Item_Id IS NULL THEN 1 ELSE 0 END AS BIT) AS Downloaded, CASE WHEN isnull(EntryDataDetails.TaxAmount,0 ) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, CAST(EntryDataDetails.Cost * EntryDataDetails.Quantity AS FLOAT) AS Total, 
                 ISNULL(AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 0) AS AsycudaDocumentSetId, case when EntryDataDetails.InvoiceQty = 0 and EntryDataDetails.ReceivedQty = 0 and EntryDataDetails.Quantity > 0 then EntryDataDetails.Quantity else EntryDataDetails.InvoiceQty end as InvoiceQty, EntryDataDetails.ReceivedQty, EntryDataDetails.Status, 
                 EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.CNumber AS PreviousCNumber, EntryDataDetails.CLineNumber as PreviousCLineNumber, EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryDataDetails.IsReconciled, EntryDataDetails.LastCost, 
                 EntryDataDetails.FileLineNumber, InventorySources.Name, EntryDataDetails.InventoryItemId, EntryDataDetails.TaxAmount
FROM    AsycudaDocumentSetEntryData AS AsycudaDocumentSetEntryData_1 WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEntryData ON AsycudaDocumentSetEntryData_1.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId RIGHT OUTER JOIN
                 EntryDataDetails WITH (NOLOCK) ON AsycudaDocumentSetEntryData.EntryData_Id = EntryDataDetails.EntryData_Id AND 
                 AsycudaDocumentSetEntryData_1.EntryData_Id = EntryDataDetails.EntryData_Id LEFT OUTER JOIN
                 InventoryItems WITH (NOLOCK) INNER JOIN
                 InventoryItemSource WITH (NOLOCK) ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                 InventorySources WITH (NOLOCK) ON InventoryItemSource.InventorySourceId = InventorySources.Id ON EntryDataDetails.InventoryItemId = InventoryItems.Id LEFT OUTER JOIN
                 xcuda_Item WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = xcuda_Item.EntryDataDetailsId LEFT OUTER JOIN
                 xcuda_Registration WITH (NOLOCK) ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
GROUP BY EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryDataId, EntryDataDetails.LineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity, EntryDataDetails.Units, 
                 EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.QtyAllocated, EntryDataDetails.UnitWeight, InventoryItems.TariffCode, xcuda_Registration.Number, xcuda_Item.LineNumber, 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryDataDetails.Status, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.CNumber, 
                 EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryDataDetails.LastCost, EntryDataDetails.TaxAmount, EntryDataDetails.DoNotAllocate, xcuda_Item.Item_Id, EntryDataDetails.IsReconciled, 
                 InventorySources.Name, EntryDataDetails.EntryData_Id, EntryDataDetails.InventoryItemId, EntryDataDetails.FileLineNumber, EntryDataDetails.VolumeLiters, EntryDataDetails.CLineNumber
GO
