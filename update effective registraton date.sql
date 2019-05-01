UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                EffectiveRegistrationDate = '12/1/2016'
FROM            xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                         xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
WHERE        (xcuda_Registration.Date = '3/11/17')
