DELETE FROM AsycudaDocumentset_Attachments
WHERE (AsycudaDocumentset_Attachments.AsycudaDocumentSetId = 1437 and AttachmentId >= 2121)

DELETE FROM AsycudaDocumentset_Attachments
WHERE (AsycudaDocumentset_Attachments.AsycudaDocumentSetId = 422)

DELETE FROM AsycudaDocument_Attachments
FROM    AsycudaDocument_Attachments INNER JOIN
                 AsycudaDocument ON AsycudaDocument_Attachments.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id
WHERE (AsycudaDocument.AsycudaDocumentSetId = 422)


DELETE FROM xcuda_Attached_documents
FROM    xcuda_Item INNER JOIN
                 xcuda_Attached_documents ON xcuda_Item.Item_Id = xcuda_Attached_documents.Item_Id INNER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id

WHERE (AsycudaDocument.AsycudaDocumentSetId = 422)



select * from AsycudaDocumentSet

SELECT Attachments.Reference, Attachments.DocumentCode, Attachments.FilePath, AsycudaDocument.ReferenceNumber, AsycudaDocument_Attachments.Id, AsycudaDocument_Attachments.AttachmentId, 
                 AsycudaDocument_Attachments.AsycudaDocumentId
FROM    AsycudaDocument_Attachments INNER JOIN
                 AsycudaDocument ON AsycudaDocument_Attachments.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 Attachments ON AsycudaDocument_Attachments.AttachmentId = Attachments.Id
				 WHERE (AsycudaDocument.AsycudaDocumentSetId = '1437')
				-- WHERE (AsycudaDocument.ASYCUDA_Id = '7949')
--WHERE (AsycudaDocument.ReferenceNumber = '30-15824-F12')
ORDER BY AsycudaDocument.ReferenceNumber

SELECT Attachments.Reference, Attachments.DocumentCode, Attachments.FilePath, AsycudaDocumentSet_Attachments.AttachmentId, AsycudaDocumentSet_Attachments.AsycudaDocumentSetId, 
                 AsycudaDocumentSet.Declarant_Reference_Number, FileTypes.Type
FROM    AsycudaDocumentSet_Attachments INNER JOIN
                 Attachments ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id INNER JOIN
                 AsycudaDocumentSet ON AsycudaDocumentSet_Attachments.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 FileTypes ON AsycudaDocumentSet_Attachments.FileTypeId = FileTypes.Id
WHERE (AsycudaDocumentSet.AsycudaDocumentSetId = '1446')
ORDER BY AsycudaDocumentSet.Declarant_Reference_Number


SELECT Attachments.Reference, Attachments.DocumentCode, Attachments.FilePath, AsycudaDocument.ReferenceNumber, AsycudaDocument_Attachments.Id, AsycudaDocument_Attachments.AttachmentId, 
                 AsycudaDocument_Attachments.AsycudaDocumentId, xcuda_Item.LineNumber, xcuda_Item.Free_text_1
FROM    xcuda_Item INNER JOIN
                 xcuda_Attached_documents ON xcuda_Item.Item_Id = xcuda_Attached_documents.Item_Id INNER JOIN
                 xcuda_Attachments ON xcuda_Attached_documents.Attached_documents_Id = xcuda_Attachments.Attached_documents_Id INNER JOIN
                 AsycudaDocument_Attachments INNER JOIN
                 AsycudaDocument ON AsycudaDocument_Attachments.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 Attachments ON AsycudaDocument_Attachments.AttachmentId = Attachments.Id ON xcuda_Attachments.AttachmentId = Attachments.Id AND xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
WHERE (AsycudaDocument.ASYCUDA_Id = '8069')
ORDER BY DocumentCode

select asycuda_id from xcuda_ASYCUDA_ExtendedProperties where AsycudaDocumentSetid = '1431'