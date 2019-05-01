

delete from EntryPreviousItems where previousitem_id in (select PreviousItem_Id from xcuda_PreviousItem where ASYCUDA_Id in (select ASYCUDA_Id from xcuda_type
where Type_of_declaration + Declaration_gen_procedure_code <> 'IM7'))

delete from xcuda_Item where ASYCUDA_Id in (select ASYCUDA_Id from xcuda_type
where Type_of_declaration + Declaration_gen_procedure_code <> 'IM7')

delete from xcuda_ASYCUDA where ASYCUDA_Id in (select ASYCUDA_Id from xcuda_type
where Type_of_declaration + Declaration_gen_procedure_code <> 'IM7')

select ASYCUDA_Id from xcuda_type
where Type_of_declaration + Declaration_gen_procedure_code <> 'IM7'

delete from AsycudaDocumentSet

 where
 Declarant_Reference_Number <> 'Sales Data' and
 AsycudaDocumentSetId not in (
SELECT AsycudaDocumentSetId
FROM     xcuda_ASYCUDA_ExtendedProperties)