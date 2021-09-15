declare @appid int = 3

delete from asycudasalesallocations 
where PreviousItem_Id in 
(select Item_Id from xcuda_Item
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where ApplicationSettingsId = @appid and ImportComplete = 0 ))

delete from xcuda_Item
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where ApplicationSettingsId = @appid and ImportComplete = 0)--

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (select ASYCUDA_Id from AsycudaDocument where ApplicationSettingsId = @appid and ImportComplete = 0)--


DELETE FROM AsycudaDocumentSet
FROM    AsycudaDocumentSet LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (AsycudaDocumentSet.ApplicationSettingsId = @appid) AND (SystemDocumentSets.Id IS NULL)