UPDATE FileTypes
SET         DocSetRefernece = AsycudaDocumentSet.Declarant_Reference_Number
FROM    AsycudaDocumentSet INNER JOIN
                 FileTypes ON AsycudaDocumentSet.AsycudaDocumentSetId = FileTypes.AsycudaDocumentSetId