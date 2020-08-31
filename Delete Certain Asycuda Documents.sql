delete from xcuda_Item
where ASYCUDA_Id in ( select ASYCUDA_Id from AsycudaDocument where ReferenceNumber like '%SUCR375559%' and ImportComplete = 1 )

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where ReferenceNumber like '%SUCR375559%' and ImportComplete = 1)