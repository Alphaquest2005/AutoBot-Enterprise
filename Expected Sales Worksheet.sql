SELECT [AsycudaSalesAllocations-ExpectedSales].DutyFreePaid, [AsycudaSalesAllocations-ExpectedSales].EntryDataDate, [AsycudaSalesAllocations-ExpectedSales].EntryDataId, 
                 [AsycudaSalesAllocations-ExpectedSales].ItemNumber, AsycudaSalesAndAdjustmentAllocationsEx.ItemDescription, [AsycudaSalesAllocations-ExpectedSales].Quantity, AsycudaSalesAndAdjustmentAllocationsEx.QtyAllocated, AsycudaSalesAndAdjustmentAllocationsEx.Status, AsycudaSalesAndAdjustmentAllocationsEx.xStatus, 
                 AsycudaSalesAndAdjustmentAllocationsEx.InvoiceNo, AsycudaSalesAndAdjustmentAllocationsEx.xReferenceNumber, AsycudaSalesAndAdjustmentAllocationsEx.xCNumber, 
                 AsycudaSalesAndAdjustmentAllocationsEx.xLineNumber , AsycudaSalesAndAdjustmentAllocationsEx.pReferenceNumber, AsycudaSalesAndAdjustmentAllocationsEx.pCNumber, 
                 AsycudaSalesAndAdjustmentAllocationsEx.pLineNumber
FROM    [AsycudaSalesAllocations-ExpectedSales] LEFT OUTER JOIN
                 AsycudaSalesAndAdjustmentAllocationsEx ON [AsycudaSalesAllocations-ExpectedSales].DutyFreePaid = case when AsycudaSalesAndAdjustmentAllocationsEx.DutyFreePaid = 'Duty Paid' then 1 else 0 end AND 
                 [AsycudaSalesAllocations-ExpectedSales].ItemNumber = AsycudaSalesAndAdjustmentAllocationsEx.ItemNumber AND 
                 Cast([AsycudaSalesAllocations-ExpectedSales].EntryDataDate as date) = cast(AsycudaSalesAndAdjustmentAllocationsEx.InvoiceDate as date) AND 
                 [AsycudaSalesAllocations-ExpectedSales].EntryDataId = AsycudaSalesAndAdjustmentAllocationsEx.InvoiceNo
where [AsycudaSalesAllocations-ExpectedSales].id not in ( select id from (select id, entrydataid, entrydatadate, itemnumber, row_number() over (partition by entrydataid, entrydatadate, itemnumber order by id) as row from [AsycudaSalesAllocations-ExpectedSales]) as t where row > 1)
		and  [AsycudaSalesAllocations-ExpectedSales].ItemNumber = '14003-0009CAR'--[AsycudaSalesAllocations-ExpectedSales].entrydataid = '144769' and

select * from [AsycudaSalesAllocations-ExpectedSales] where entrydataid = '144769' and ItemNumber = '14002-220'
select * from EntryDataDetailsEx where entrydataid = '144769' and ItemNumber = '14002-220'
select * from AsycudaSalesAndAdjustmentAllocationsEx where InvoiceNo = '144769' and ItemNumber = '14002-220'



select id from (select id, entrydataid, entrydatadate, itemnumber, row_number() over (partition by entrydataid, entrydatadate, itemnumber order by id) as row from [AsycudaSalesAllocations-ExpectedSales]) as t where row > 1

declare @pCnumber nvarchar(50) = '17227', @pLineNumber int = 45
select * from AsycudaItemPiQuantityData where  pCNumber = @pCnumber and pLineNumber = @pLineNumber

select * from AsycudaDocumentItem where  CNumber = @pCnumber and LineNumber = @pLineNumber

select ExpiryDate from Asycudadocument where CNumber = 7653

select * from AsycudaDocumentItem where PreviousInvoiceNumber like '%144664%'
