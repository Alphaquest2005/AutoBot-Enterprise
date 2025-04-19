delete from xcuda_Item
where ASYCUDA_Id in ( select ASYCUDA_Id from AsycudaDocument where ReferenceNumber like '%march 2021%' and ImportComplete = 0 )

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where ReferenceNumber like '%march 2021%' and ImportComplete = 0)


delete from xcuda_Item
where ASYCUDA_Id in ( select ASYCUDA_Id from AsycudaDocument where RegistrationDate < '9/24/2018' )

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where RegistrationDate < '9/24/2018')