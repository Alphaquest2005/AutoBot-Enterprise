
--delete from AsycudaDocument_Attachments where id in 
--(select a.attid from 
(SELECT AsycudaDocument.ReferenceNumber, AsycudaDocument_Attachments.Id, Attachments.FilePath, AsycudaDocument_Attachments.id as attid
FROM    AsycudaDocument INNER JOIN
                 AsycudaDocument_Attachments ON AsycudaDocument.ASYCUDA_Id = AsycudaDocument_Attachments.AsycudaDocumentId INNER JOIN
                 Attachments ON AsycudaDocument_Attachments.AttachmentId = Attachments.Id
WHERE (AsycudaDocument.ReferenceNumber like N'%DEC21CNT%')) as a)


SELECT AsycudaDocument.ReferenceNumber, EntryData.EntryDataId
FROM    AsycudaDocument INNER JOIN
                 AsycudaDocumentEntryData ON AsycudaDocument.ASYCUDA_Id = AsycudaDocumentEntryData.AsycudaDocumentId INNER JOIN
                 EntryData ON AsycudaDocumentEntryData.EntryData_Id = EntryData.EntryData_Id
WHERE (AsycudaDocument.ReferenceNumber LIKE N'%DEC21CNT%')


SELECT AsycudaDocumentSetEx.Declarant_Reference_Number, EntryData.EntryDataId
FROM    AsycudaDocumentSetEx INNER JOIN
                 AsycudaDocumentSetEntryData ON AsycudaDocumentSetEx.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId INNER JOIN
                 EntryData ON AsycudaDocumentSetEntryData.EntryData_Id = EntryData.EntryData_Id
WHERE (AsycudaDocumentSetEx.Declarant_Reference_Number LIKE N'%DEC21CNT%')


select  linenumber, ItemNumber, Commercial_Description, ItemQuantity, Total_CIF_itm from AsycudaDocumentItem where cnumber = '1214' order by cast(LineNumber as  int)

select  linenumber, ItemNumber, Commercial_Description, ItemQuantity, Total_CIF_itm from AsycudaDocumentItem where ReferenceNumber like '%DEC19%' and ItemNumber like 'OSC/3802140'and CustomsProcedure like '7400%' order by cast(LineNumber as  int)