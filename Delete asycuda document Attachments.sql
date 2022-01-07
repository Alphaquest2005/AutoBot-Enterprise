
delete from AsycudaDocument_Attachments where id in 
(select a.attid from 
(SELECT AsycudaDocument.ReferenceNumber, AsycudaDocument_Attachments.Id, Attachments.FilePath, AsycudaDocument_Attachments.id as attid
FROM    AsycudaDocument INNER JOIN
                 AsycudaDocument_Attachments ON AsycudaDocument.ASYCUDA_Id = AsycudaDocument_Attachments.AsycudaDocumentId INNER JOIN
                 Attachments ON AsycudaDocument_Attachments.AttachmentId = Attachments.Id
WHERE (AsycudaDocument.ReferenceNumber like N'%December 2021%')) as a)