delete entrydata
where entrydatadate >= '4/1/2019'


delete from xcuda_Item
where ASYCUDA_Id in (
SELECT xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
                 AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId
WHERE (CAST(xcuda_Registration.Date AS Date) >= '4/1/2019') AND (AsycudaDocumentSet.ApplicationSettingsId = 2))

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (
SELECT xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
                 AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId
WHERE (CAST(xcuda_Registration.Date AS Date) >= '4/1/2019') AND (AsycudaDocumentSet.ApplicationSettingsId = 2))