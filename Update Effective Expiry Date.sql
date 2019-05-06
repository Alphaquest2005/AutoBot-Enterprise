UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                EffectiveExpiryDate = CONVERT(DATETIME, '2020-10-15', 102)
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.CNumber = '37628')