--DELETE FROM EntryData
--FROM    EntryData INNER JOIN
--                 EntryData_Adjustments ON EntryData.EntryDataId = EntryData_Adjustments.EntryDataId

delete from AsycudaSalesAllocations
where AllocationId in (select AllocationId from AdjustmentShortAllocations)

update EntryDataDetails
set Status = null, EffectiveDate = null
from EntryDataDetails inner join AdjustmentDetails on EntryDataDetails.EntryDataDetailsId = AdjustmentDetails.EntryDataDetailsId


