USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesPiSummary-Adjustments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[ItemSalesPiSummary-Adjustments]
AS
SELECT DISTINCT 
                 TOP (100) PERCENT AsycudaSalesAllocations.PreviousItem_Id, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity AS SalesQty, EntryDataDetails.QtyAllocated AS SalesAllocatedQty, 
                coalesce(
		CASE when AdjustmentComments.Comments is not null then AdjustmentComments.DutyFreePaid else null end,
		case when isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Adjustments.Tax, 0)) <> 0
			THEN 'Duty Paid'
		ELSE 'Duty Free'
		END) AS DutyFreePaid, 
                 PreviousDocumentItem.LineNumber AS pLineNumber, CASE WHEN coalesce(
		CASE when AdjustmentComments.Comments is not null then AdjustmentComments.DutyFreePaid else null end,
		case when isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Adjustments.Tax, 0)) <> 0
			THEN 'Duty Paid'
		ELSE 'Duty Free'
		END) 
                 = 'Duty Free' THEN DfqtyAllocated ELSE DPQtyallocated END AS pQtyAllocated, AsycudaDocumentBasicInfo.CNumber AS pCNumber, AsycudaDocumentBasicInfo.RegistrationDate AS pRegistrationDate, 
                 AsycudaDocumentBasicInfo.AssessmentDate AS pAssessmentDate, ISNULL(AsycudaSalesAllocations.QtyAllocated, 0) AS QtyAllocated,
				 coalesce(entrydatadate,EntryDataDetails.effectivedate) AS EntryDataDate,--CASE WHEN entrydata_adjustments.entrydata_id IS NULL  THEN entrydatadate ELSE EntryDataDetails.effectivedate END AS EntryDataDate,
				 EntryData.ApplicationSettingsId, CASE WHEN EntryData_Adjustments.entrydata_id IS NOT NULL THEN EntryData_Adjustments.Type ELSE '' END AS Type, AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.Status
		--		, coalesce(
		--CASE when AdjustmentComments.Comments is not null then AdjustmentComments.DutyFreePaid else null end,
		--case when isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Adjustments.Tax, 0)) <> 0
		--	THEN 'Duty Paid'
		--ELSE 'Duty Free'
		--END) as DFP
		, xEntryItem_Id, EntryDataDetails.EntryDataDetailsId
FROM    AsycudaSalesAllocations INNER JOIN
                 xcuda_Item AS PreviousDocumentItem ON AsycudaSalesAllocations.PreviousItem_Id = PreviousDocumentItem.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo ON PreviousDocumentItem.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
				 left outer join AdjustmentComments on lower(EntryDataDetails.Comment) = lower(AdjustmentComments.Comments)
WHERE (AsycudaSalesAllocations.Status IS NULL)  AND (EntryData_Adjustments.EntryData_Id IS NOT NULL)
GROUP BY AsycudaSalesAllocations.PreviousItem_Id, PreviousDocumentItem.LineNumber, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, 
                 EntryData.ApplicationSettingsId, AsycudaSalesAllocations.QtyAllocated, EntryData_Adjustments.EntryData_Id, EntryData.EntryDataDate, EntryDataDetails.EffectiveDate, AsycudaSalesAllocations.AllocationId, 
                 AsycudaSalesAllocations.Status, EntryData_Adjustments.Type, EntryDataDetails.Quantity, EntryDataDetails.QtyAllocated, EntryData_Adjustments.Tax, EntryDataDetails.TaxAmount, 
                 PreviousDocumentItem.DFQtyAllocated, PreviousDocumentItem.DPQtyAllocated, EntryDataDetails.ItemNumber,AdjustmentComments.Comments, AdjustmentComments.DutyFreePaid, xEntryItem_Id, EntryDataDetails.EntryDataDetailsId
ORDER BY EntryDataDate
GO
