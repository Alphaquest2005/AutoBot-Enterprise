
UPDATE xcuda_ASYCUDA_ExtendedProperties
SET         EffectiveExpiryDate = exp.Expiration
FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code, 
                                  CAST(ExpiredEntriesLst.Expiration AS datetime) AS Expiration
                 FROM     ExpiredEntriesLst INNER JOIN
                                  AsycudaDocument ON ExpiredEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND ExpiredEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                                  ExpiredEntriesLst.RegistrationNumber = AsycudaDocument.CNumber AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber) AS exp INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id

select * from [TODO-Error-ExpiredEntries] where ApplicationSettingsId = 2 order by RegistrationDate 


update xcuda_ASYCUDA_ExtendedProperties
set EffectiveExpiryDate = exp.Expiration
from
(SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code, cast(ExpiredEntriesLst.Expiration as datetime) as Expiration
FROM    ExpiredEntriesLst INNER JOIN
                 AsycudaDocument ON ExpiredEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND ExpiredEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate AND 
                 ExpiredEntriesLst.RegistrationNumber = AsycudaDocument.CNumber AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber) as exp

				 UPDATE xcuda_ASYCUDA_ExtendedProperties
SET         EffectiveExpiryDate = DATEADD(DAY,cast(Delay as int),xcuda_Registration.Date)
FROM    xcuda_Registration INNER JOIN
                 xcuda_Warehouse ON xcuda_Registration.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_Registration.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id


select  xcuda_Registration.Date,       EffectiveExpiryDate = DATEADD(DAY,cast(Delay as int),xcuda_Registration.Date)
FROM    xcuda_Registration INNER JOIN
                 xcuda_Warehouse ON xcuda_Registration.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_Registration.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id