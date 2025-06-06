USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PreDiscrepancyErrors-B]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[TODO-PreDiscrepancyErrors-B]
as

SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, AdjustmentDetails.Units, 
                 AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, AdjustmentDetails.DoNotAllocate, AdjustmentDetails.TariffCode, AdjustmentDetails.Total, 
                 AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, AdjustmentDetails.ReceivedQty, CASE WHEN EffectiveDate IS NULL AND dbo.AdjustmentDetails.Status IS NULL 
                 THEN 'AutoMatch Failed' ELSE dbo.AdjustmentDetails.Status END AS Status, AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AdjustmentDetails.Downloaded, 
                 AdjustmentDetails.PreviousCNumber, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.Type, AdjustmentDetails.Comment, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Currency, 
                 AdjustmentDetails.IsReconciled, AdjustmentDetails.ApplicationSettingsId, AdjustmentDetails.FileTypeId, AdjustmentDetails.EmailId, AsycudaDocumentSet.Declarant_Reference_Number, AdjustmentDetails.InvoiceDate, 
                 AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, AsycudaDocumentItem.PiQuantity, AsycudaDocumentItem.ItemQuantity--,AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.QtyAllocated, AsycudaSalesAllocations.Status, AsycudaSalesAllocations.xStatus
FROM    SystemDocumentSets RIGHT OUTER JOIN
                 AdjustmentDetails INNER JOIN
                 AsycudaDocumentSetEntryData ON AdjustmentDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId AND 
                 AdjustmentDetails.AsycudaDocumentSetId <> AsycudaDocumentSet.AsycudaDocumentSetId LEFT OUTER JOIN
                 (select AsycudaDocumentItemEntryDataDetails.* from 
					AsycudaDocumentItemEntryDataDetails
					inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails.ASYCUDA_Id
					where 
					AsycudaDocumentCustomsProcedures.Discrepancy = 1
					--DocumentType not in ('IM7','EX9')
								) as AsycudaDocumentItemEntryDataDetails ON AdjustmentDetails.EntryData_Id = AsycudaDocumentItemEntryDataDetails.EntryData_Id AND 
                 AdjustmentDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId ON SystemDocumentSets.Id = AsycudaDocumentSet.AsycudaDocumentSetId LEFT OUTER JOIN
                [TODO-PreDiscrepancyErrors-AsycudaDocumentItem] as AsycudaDocumentItem INNER JOIN
                 AdjustmentOversAllocations ON AsycudaDocumentItem.Item_Id = AdjustmentOversAllocations.PreviousItem_Id ON AdjustmentDetails.EntryDataDetailsId = AdjustmentOversAllocations.EntryDataDetailsId
WHERE (AdjustmentDetails.Type = 'DIS') and AdjustmentDetails.status is null AND (SystemDocumentSets.Id IS NULL) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) and isnull(AdjustmentDetails.IsReconciled,0) <> 1 
and ((AdjustmentOversAllocations.AllocationId IS NULL) and (AdjustmentDetails.InvoiceQty - AdjustmentDetails.ReceivedQty < 0))
GO
