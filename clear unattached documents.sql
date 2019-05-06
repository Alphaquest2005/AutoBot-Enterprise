delete from xcuda_ASYCUDA where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where AsycudaDocumentSetId is null)

delete from EntryData where EntryDataId not in (SELECT EntryDataId
FROM    AsycudaDocumentSetEntryData)