
delete from EntryDataDetails where EntryDataDetailsId in (select entrydatadetailsid from [AutoBot-EnterpriseDB].dbo.[TODO-DiscrepanciesToDoList])

select id from SystemDocumentSets

delete from AsycudaDocumentSet 
where asycudadocumentsetid > 213

delete from xcuda_ASYCUDA where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where AsycudaDocumentSetId is null)

delete from EntryData where EntryDataId not in (SELECT EntryDataId
FROM    AsycudaDocumentSetEntryData)