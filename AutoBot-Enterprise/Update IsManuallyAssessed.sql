UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                IsManuallyAssessed = 1
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.Reference LIKE '%SalesR%')

select * 
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.Reference like '%SalesR%')

select * from AsycudaDocument where IsManuallyAssessed = 1

delete 
from xcuda_ASYCUDA
where xcuda_ASYCUDA.ASYCUDA_Id in (SELECT AsycudaDocumentBasicInfo.ASYCUDA_Id
FROM    AsycudaDocumentBasicInfo INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId
WHERE (AsycudaDocumentBasicInfo.DocumentType = 'im9') AND (AsycudaDocumentSet.Declarant_Reference_Number LIKE '%disc%')) 


