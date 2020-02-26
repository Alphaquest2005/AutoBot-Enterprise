UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                Cancelled = 1
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.CNumber in () and AsycudaDocumentBasicInfo.ApplicationSettingsId = 2)
--WHERE        ( AsycudaDocumentBasicInfo.RegistrationDate <= '9/24/2018' and AsycudaDocumentBasicInfo.IsManuallyAssessed is null )


select * from AsycudaDocumentBasicInfo where AsycudaDocumentBasicInfo.RegistrationDate <= '9/24/2018' and IsManuallyAssessed is null 

UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                Cancelled = 1
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.CNumber in ('39') and AsycudaDocumentBasicInfo.ApplicationSettingsId = 2)