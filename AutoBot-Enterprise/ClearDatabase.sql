delete from AsycudaSalesAllocations
DELETE FROM AsycudaDocumentSet
FROM    AsycudaDocumentSet LEFT OUTER JOIN
                 FileTypes ON AsycudaDocumentSet.AsycudaDocumentSetId = FileTypes.AsycudaDocumentSetId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (SystemDocumentSets.Id IS NULL) and filetypes.AsycudaDocumentSetId is null
delete from xcuda_Item
delete from xcuda_ASYCUDA
delete from [InventoryItems-NonStock]
delete from InventoryItems
delete from EntryData

