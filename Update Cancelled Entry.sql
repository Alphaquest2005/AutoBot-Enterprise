UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                Cancelled = 1
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.CNumber in (57972,57975,57976,57977,57978,57979,57980,57981,57982,57983,57984
) and AsycudaDocumentBasicInfo.ApplicationSettingsId = 6)
--WHERE        ( AsycudaDocumentBasicInfo.RegistrationDate <= '9/24/2018' and AsycudaDocumentBasicInfo.IsManuallyAssessed is null )


select * from AsycudaDocumentBasicInfo where AsycudaDocumentBasicInfo.RegistrationDate <= '9/24/2018' and IsManuallyAssessed is null 


select AsycudaDocumentBasicInfo.Cancelled , AsycudaDocumentBasicInfo.*             
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.CNumber in ('36383') and  AsycudaDocumentBasicInfo.ApplicationSettingsId = 2)


UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                Cancelled = 1
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.CNumber in ('49877') and  AsycudaDocumentBasicInfo.ApplicationSettingsId = 7)


------------------------ reset cancelled entries
UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                Cancelled = null

select * from AsycudaDocumentBasicInfo where Cancelled = 1


SELECT CancelledEntriesLst.*
						FROM     CancelledEntriesLst left outer join
										AsycudaDocument ON
										CancelledEntriesLst.RegistrationNumber = AsycudaDocument.CNumber and CancelledEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND CancelledEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate--AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber
					where AsycudaDocument.CNumber is null

select * from AsycudaDocument where CNumber = '43109'