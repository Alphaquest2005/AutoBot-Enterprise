USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PreDiscrepancyErrors-A]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE view [dbo].[TODO-PreDiscrepancyErrors-A]
as
SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, AdjustmentDetails.Units, 
                 AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, AdjustmentDetails.DoNotAllocate, AdjustmentDetails.TariffCode, AdjustmentDetails.Total, 
                 AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, AdjustmentDetails.ReceivedQty, CASE WHEN EffectiveDate IS NULL AND dbo.AdjustmentDetails.Status IS NULL 
                 THEN 'AutoMatch Failed' WHEN (AsycudaDocumentItem.PiQuantity + AsycudaSalesAllocations.QtyAllocated > AsycudaDocumentItem.ItemQuantity) 
                 THEN 'PiQuantity + QtyAllocated > ItemQuantity' ELSE isnull(dbo.AdjustmentDetails.Status, ErrAllocation.Status) END AS Status, AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.CNumber, 
                 AdjustmentDetails.CLineNumber, AdjustmentDetails.Downloaded, AdjustmentDetails.PreviousCNumber, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.Type, AdjustmentDetails.Comment, 
                 AdjustmentDetails.EffectiveDate, AdjustmentDetails.Currency, AdjustmentDetails.IsReconciled, AdjustmentDetails.ApplicationSettingsId, AdjustmentDetails.FileTypeId, AdjustmentDetails.EmailId, 
                 AsycudaDocumentSet.Declarant_Reference_Number, AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, AsycudaDocumentItem.PiQuantity, AsycudaDocumentItem.ItemQuantity
                 --, AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.QtyAllocated AS AllocationQty, AsycudaSalesAllocations.Status AS allocationStatus, AsycudaSalesAllocations.xStatus, ErrAllocation.Status AS Expr1
FROM    (SELECT AsycudaDocumentItemEntryDataDetails_1.ApplicationSettingsId, AsycudaDocumentItemEntryDataDetails_1.AsycudaDocumentSetId, AsycudaDocumentItemEntryDataDetails_1.EntryDataDetailsId, 
                                  AsycudaDocumentItemEntryDataDetails_1.EntryData_Id, AsycudaDocumentItemEntryDataDetails_1.Item_Id, AsycudaDocumentItemEntryDataDetails_1.ItemNumber, 
                                  AsycudaDocumentItemEntryDataDetails_1.[key], AsycudaDocumentItemEntryDataDetails_1.DocumentType, AsycudaDocumentItemEntryDataDetails_1.CustomsProcedure, 
                                  AsycudaDocumentItemEntryDataDetails_1.Quantity, AsycudaDocumentItemEntryDataDetails_1.ImportComplete, AsycudaDocumentItemEntryDataDetails_1.Asycuda_id, 
                                  AsycudaDocumentItemEntryDataDetails_1.EntryDataType
                 FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1 INNER JOIN
                                  AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails_1.Asycuda_id
                 WHERE  (AsycudaDocumentCustomsProcedures.Discrepancy = 1)) AS AsycudaDocumentItemEntryDataDetails RIGHT OUTER JOIN
                 AdjustmentDetails INNER JOIN
                 AsycudaDocumentSetEntryData ON AdjustmentDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId AND 
                 AdjustmentDetails.AsycudaDocumentSetId <> AsycudaDocumentSet.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaSalesAllocations AS ErrAllocation ON AdjustmentDetails.EntryDataDetailsId = ErrAllocation.EntryDataDetailsId ON AsycudaDocumentItemEntryDataDetails.EntryData_Id = AdjustmentDetails.EntryData_Id AND 
                 AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = AdjustmentDetails.EntryDataDetailsId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                 [TODO-PreDiscrepancyErrors-AsycudaDocumentItem] AS AsycudaDocumentItem INNER JOIN
                 AsycudaSalesAllocations AS AsycudaSalesAllocations ON AsycudaDocumentItem.Item_Id = AsycudaSalesAllocations.PreviousItem_Id ON 
                 AdjustmentDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId
WHERE (AdjustmentDetails.Type = 'DIS') AND (SystemDocumentSets.Id IS NULL) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (ISNULL(AdjustmentDetails.IsReconciled, 0) <> 1) AND 
                 (AsycudaSalesAllocations.AllocationId IS NULL) AND (AdjustmentDetails.InvoiceQty > 0) OR
                 (AdjustmentDetails.Type = 'DIS') AND (SystemDocumentSets.Id IS NULL) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (ISNULL(AdjustmentDetails.IsReconciled, 0) <> 1) AND 
                 (AsycudaSalesAllocations.AllocationId IS NOT NULL) AND (AsycudaDocumentItem.PiQuantity + AsycudaSalesAllocations.QtyAllocated > AsycudaDocumentItem.ItemQuantity)
GO
