delete from xcuda_Item
where ASYCUDA_Id in ( select ASYCUDA_Id from AsycudaDocument where ReferenceNumber like '%Imports%' and ImportComplete = 0 )

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where ReferenceNumber like '%Imports%' and ImportComplete = 0)