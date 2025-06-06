USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesPiSummary-ItemNumber]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[ItemSalesPiSummary-ItemNumber]
AS
SELECT TOP (100) PERCENT AsycudaSalesAllocations.PreviousItem_Id, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity AS SalesQty, EntryDataDetails.QtyAllocated AS SalesAllocatedQty, 
                 CASE WHEN isnull(entrydatadetails.taxamount, COALESCE (EntryData_Sales.Tax, isnull(EntryData_Adjustments.Tax, 0))) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, 
                 PreviousDocumentItem.LineNumber AS pLineNumber, CASE WHEN isnull(entrydatadetails.taxamount, isnull(EntryData_Sales.Tax, isnull(EntryData_Adjustments.Tax, 0))) 
                 = 0 THEN DfqtyAllocated ELSE DPQtyallocated END AS pQtyAllocated, AsycudaDocumentBasicInfo.CNumber AS pCNumber, AsycudaDocumentBasicInfo.RegistrationDate AS pRegistrationDate, 
                 AsycudaDocumentBasicInfo.AssessmentDate AS pAssessmentDate, ISNULL(AsycudaSalesAllocations.QtyAllocated, 0) AS QtyAllocated, CASE WHEN entrydata_adjustments.entrydata_id IS NULL 
                 THEN entrydatadate ELSE EntryDataDetails.effectivedate END AS EntryDataDate, EntryData.ApplicationSettingsId, CASE WHEN EntryData_sales.entrydata_id IS NOT NULL 
                 THEN 'Sales' WHEN EntryData_Adjustments.entrydata_id IS NOT NULL THEN EntryData_Adjustments.Type ELSE '' END AS Type, AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.Status
FROM    AsycudaSalesAllocations INNER JOIN
                 xcuda_Item AS PreviousDocumentItem ON AsycudaSalesAllocations.PreviousItem_Id = PreviousDocumentItem.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo ON PreviousDocumentItem.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId INNER JOIN
                 InventoryItemAliasEx ON EntryDataDetails.ItemNumber = InventoryItemAliasEx.ItemNumber AND EntryData.ApplicationSettingsId = InventoryItemAliasEx.ApplicationSettingsId LEFT OUTER JOIN
                 EntryData_Sales ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id LEFT OUTER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
WHERE (AsycudaSalesAllocations.Status IS NULL) 
GROUP BY AsycudaSalesAllocations.PreviousItem_Id, PreviousDocumentItem.LineNumber, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, 
                 EntryData.ApplicationSettingsId, EntryDataDetails.ItemNumber, AsycudaSalesAllocations.QtyAllocated, EntryData_Adjustments.EntryData_Id, EntryData.EntryDataDate, EntryDataDetails.EffectiveDate, 
                 AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.Status, EntryData_Sales.EntryData_Id, EntryData_Adjustments.Type, EntryDataDetails.Quantity, EntryDataDetails.QtyAllocated, 
                 EntryData_Adjustments.Tax, EntryDataDetails.TaxAmount, EntryData_Sales.Tax, PreviousDocumentItem.DFQtyAllocated, PreviousDocumentItem.DPQtyAllocated
ORDER BY EntryDataDate
GO
