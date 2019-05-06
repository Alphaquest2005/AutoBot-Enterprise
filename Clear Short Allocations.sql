delete from AsycudaSalesAllocations
where AllocationId in (select AllocationId from AdjustmentShortAllocations)



