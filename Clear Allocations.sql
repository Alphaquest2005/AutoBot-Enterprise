
declare @appSettingsId int = 6

DELETE FROM AdjustmentOversAllocations
FROM            AdjustmentOversAllocations LEFT OUTER JOIN
                         EntryData INNER JOIN
                         EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId ON AdjustmentOversAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId 
WHERE        ApplicationSettingsId = @appSettingsId

DELETE FROM AdjustmentOversAllocations
FROM            AsycudaItemBasicInfo INNER JOIN
                         AdjustmentOversAllocations ON AsycudaItemBasicInfo.Item_Id = AdjustmentOversAllocations.PreviousItem_Id
WHERE        ApplicationSettingsId = @appSettingsId

DELETE FROM AsycudaSalesAllocations
FROM            EntryData INNER JOIN
                         EntryDataDetails ON EntryData.EntryDataId = EntryDataDetails.EntryDataId RIGHT OUTER JOIN
                         AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId
WHERE        ApplicationSettingsId = @appSettingsId


DELETE FROM AsycudaSalesAllocations
FROM            AsycudaItemBasicInfo inner JOIN
                         AsycudaSalesAllocations ON AsycudaItemBasicInfo.Item_Id = AsycudaSalesAllocations.PreviousItem_Id
WHERE        ApplicationSettingsId = @appSettingsId



UPDATE xcuda_Item
SET         DFQtyAllocated = 0, DPQtyAllocated = 0
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id
WHERE (AsycudaDocumentSet.ApplicationSettingsId = @appSettingsId)


UPDATE EntryDataDetails
SET         QtyAllocated = 0, Status = NULL, EffectiveDate = NULL
FROM    EntryDataDetails INNER JOIN
                 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId
WHERE (EntryData.ApplicationSettingsId = @appSettingsId)

UPDATE xcuda_PreviousItem
SET         QtyAllocated = 0
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 xcuda_PreviousItem ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_PreviousItem.ASYCUDA_Id
WHERE (AsycudaDocumentSet.ApplicationSettingsId = @appSettingsId)

UPDATE SubItems
SET         QtyAllocated = 0
FROM    xcuda_Item INNER JOIN
                 SubItems ON xcuda_Item.Item_Id = SubItems.Item_Id INNER JOIN
                 AsycudaDocumentSet INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentSet.AsycudaDocumentSetId = xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ON 
                 xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE (AsycudaDocumentSet.ApplicationSettingsId = @appSettingsId)

SELECT AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.EntryDataDetailsId, AsycudaSalesAllocations.PreviousItem_Id, AsycudaSalesAllocations.Status, AsycudaSalesAllocations.QtyAllocated, 
                 AsycudaSalesAllocations.EntryTimeStamp, AsycudaSalesAllocations.EANumber, AsycudaSalesAllocations.SANumber, AsycudaSalesAllocations.xEntryItem_Id, EntryData.ApplicationSettingsId
FROM    AsycudaSalesAllocations WITH (nolock) INNER JOIN
                 EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId
WHERE (EntryData.ApplicationSettingsId = @appSettingsId)