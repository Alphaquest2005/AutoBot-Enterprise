
--test with CHAIN/10G-28, WPA/30062,INT/YBA063GL,BM/SHG16B, INT/YBC103/1,CMC/11811 - expried,BM/FGFB,TOH/MTS009_8S,REF/RF0A3FDSTAH
declare @itemnumber nvarchar(50) = 'SEH/R1-S1', @cnumber nvarchar(50) = '11062' , @linenumber int = 9

declare @itemId int = (select item_id from AsycudaItemBasicInfo where CNumber = @cnumber and LineNumber = @linenumber)
select 'asycudadocument'
select * from asycudadocument where cnumber = @cnumber

select 'AsycudaDocumentItem'
select * from AsycudaDocumentItem where Item_Id = @itemId
select 'AsycudaItemPiQuantityData'
select * from AsycudaItemPiQuantityData where Item_Id = @itemId
select 'ItemSalesAsycudaPiSummary'
select * from ItemSalesAsycudaPiSummary where PreviousItem_Id = @itemId
select '[ItemSalesAsycudaPiSummary-Asycuda]'
select * from [ItemSalesAsycudaPiSummary-Asycuda] where PreviousItem_Id = @itemId
select '[ItemSalesAsycudaPiSummary-Sales]'
select * from [ItemSalesAsycudaPiSummary-Sales] where PreviousItem_Id = @itemId
select 'AsycudaSalesAndAdjustmentAllocationsEx'
select * from AsycudaSalesAndAdjustmentAllocationsEx where PreviousItem_Id = @itemId
select '[DataCheck-Unexwarehoused Per month-year]'
select * from [DataCheck-Unexwarehoused Per month-year] where pCNumber =  @cnumber and pLineNumber = @linenumber									
select 'Total Sales sum qtyallocated'
select sum(QtyAllocated) from AsycudaSalesAndAdjustmentAllocationsEx where ItemNumber = @itemnumber
select 'universal Qty Allocated vs PIquantity'
select sum(DFQtyAllocated + DPQtyAllocated), sum(PiQuantity) from AsycudaDocumentItem where ItemNumber in (select itemnumber from InventoryItemAliasEx where ItemNumber = @itemnumber ) or ItemNumber = @itemnumber
select 'universal Qty Allocated vs PIquantity'
select * from [EX9AsycudaAllocations-Sales] where pCNumber = @cnumber and plinenumber  = @linenumber


select * from asycudadocumentitem where itemnumber = '8BM/MK-BWFLG-LG'