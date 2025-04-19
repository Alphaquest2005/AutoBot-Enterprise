
delete from entrydata where entrydataid in 
(SELECT EntryData.EntryDataId
FROM    EntryData LEFT OUTER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryDataId = AsycudaDocumentSetEntryData.EntryDataId
WHERE (AsycudaDocumentSetEntryData.AsycudaDocumentSetId IS NULL))