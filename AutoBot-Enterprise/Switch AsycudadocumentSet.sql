
update AsycudaDocumentSetEntryData
set AsycudaDocumentSetId = 8
where id in 
(SELECT AsycudaDocumentSetEntryData.Id 
FROM    EntryData_Sales INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData_Sales.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (SystemDocumentSets.Id IS NULL))