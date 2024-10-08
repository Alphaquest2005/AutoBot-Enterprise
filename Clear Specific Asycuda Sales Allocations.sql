


  drop table #alst
  select AllocationId, PreviousItem_Id, EntryDataDetailsId  
  into #alst
  from AsycudaSalesAndAdjustmentAllocationsEx where pCNumber = '34593'

  delete from AsycudaSalesAllocations where
  AllocationId in (select AllocationId from #alst)

  

update xcuda_Item
set DFQtyAllocated = 0, DPQtyAllocated = 0
where  Item_Id in (select PreviousItem_Id from #alst)--(select Item_Id from AsycudaItemBasicInfo where ItemNumber = 'EVC/100508' or ItemNumber = 'EVC/508') 


update EntryDataDetails
set QtyAllocated = 0
where  EntryDataDetailsId in  (select EntryDataDetailsId from #alst)--(select EntryDataDetailsId from EntryDataDetails where ItemNumber = 'EVC/100508' or ItemNumber = 'EVC/508')--(select EntryDataDetailsId from #alst)

update xcuda_PreviousItem
set QtyAllocated = 0
where  PreviousItem_Id in (select PreviousItem_Id from #alst)-- (select Item_Id from AsycudaItemBasicInfo where ItemNumber = 'EVC/100508' or ItemNumber = 'EVC/508')-- (select PreviousItem_Id from #alst)

update SubItems 
set QtyAllocated = 0

select * from AsycudaSalesAllocationsEx with(nolock) where ItemNumber = 'EVC/508'


(select * from EntryDataDetails where ItemNumber = 'EVC/508')