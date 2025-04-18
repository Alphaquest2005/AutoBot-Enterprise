USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesPiSummary-Sales]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[ItemSalesPiSummary-Sales]
AS
SELECT DISTINCT 
                TOP (100) PERCENT AsycudaSalesAllocations.PreviousItem_Id, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity AS SalesQty, EntryDataDetails.QtyAllocated AS SalesAllocatedQty, CASE WHEN isnull(entrydatadetails.taxamount, 
                isnull(EntryData_Sales.Tax, 0)) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, PreviousDocumentItem.LineNumber AS pLineNumber, CASE WHEN isnull(entrydatadetails.taxamount, isnull(EntryData_Sales.Tax, 0)) 
                = 0 THEN DfqtyAllocated ELSE DPQtyallocated END AS pQtyAllocated, AsycudaDocumentBasicInfo.CNumber AS pCNumber, AsycudaDocumentBasicInfo.RegistrationDate AS pRegistrationDate, 
                AsycudaDocumentBasicInfo.AssessmentDate AS pAssessmentDate, ISNULL(AsycudaSalesAllocations.QtyAllocated, 0) AS QtyAllocated, EntryData.EntryDataDate, EntryData.ApplicationSettingsId, 'Sales' AS Type, 
                AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.Status, AsycudaSalesAllocations.xEntryItem_Id, EntryDataDetails.EntryDataDetailsId
FROM      EntryData INNER JOIN
                EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id LEFT OUTER JOIN
                AsycudaSalesAllocations INNER JOIN
                xcuda_Item AS PreviousDocumentItem ON AsycudaSalesAllocations.PreviousItem_Id = PreviousDocumentItem.Item_Id INNER JOIN
                AsycudaDocumentBasicInfo ON PreviousDocumentItem.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                EntryData_Sales ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id
WHERE   (EntryData_Sales.EntryData_Id IS NOT NULL and AsycudaSalesAllocations.Status is null)
GROUP BY AsycudaSalesAllocations.PreviousItem_Id, PreviousDocumentItem.LineNumber, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, 
                EntryData.ApplicationSettingsId, AsycudaSalesAllocations.QtyAllocated, EntryData.EntryDataDate, EntryDataDetails.EffectiveDate, AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.Status, EntryData_Sales.EntryData_Id, 
                EntryDataDetails.Quantity, EntryDataDetails.QtyAllocated, EntryDataDetails.TaxAmount, EntryData_Sales.Tax, PreviousDocumentItem.DFQtyAllocated, PreviousDocumentItem.DPQtyAllocated, EntryDataDetails.ItemNumber, 
                AsycudaSalesAllocations.xEntryItem_Id, EntryDataDetails.EntryDataDetailsId 
ORDER BY EntryData.EntryDataDate
GO
