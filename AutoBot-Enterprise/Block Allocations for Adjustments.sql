update EntryDataDetails 
set DoNotAllocate = Null, [Status] = Null
where [Status] = 'Stock Issues'



update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM      EntryDataDetails INNER JOIN
                         AsycudaSalesAllocationsEx ON EntryDataDetails.ItemNumber = AsycudaSalesAllocationsEx.ItemNumber INNER JOIN
                         EntryData_Sales ON EntryDataDetails.EntryDataId = EntryData_Sales.EntryDataId INNER JOIN
                         EntryData ON EntryData_Sales.EntryDataId = EntryData.EntryDataId
                         
WHERE        (EntryDataDetails.EntryDataId <> 'Asycuda') and (EntryDataDetails.[Status] is null)  and AsycudaSalesAllocationsEx.Status is not null


update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM      EntryDataDetails INNER JOIN
                         
                         AdjustmentDetails ON EntryDataDetails.ItemNumber = AdjustmentDetails.ItemNumber 
                         
WHERE        (EntryDataDetails.EntryDataId <> 'Asycuda') and (EntryDataDetails.[Status] is null)


update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM      EntryDataDetails INNER JOIN
                         
                         AdjustmentSales ON EntryDataDetails.ItemNumber = AdjustmentSales.ItemNumber 
                         
WHERE        (EntryDataDetails.EntryDataId <> 'Asycuda') and (EntryDataDetails.[Status] is null)


select * from EntryDataDetailsEx where status = 'stock issues'