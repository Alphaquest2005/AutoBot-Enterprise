delete from xcuda_ASYCUDA
where ASYCUDA_Id in
(
select ASYCUDA_Id from (SELECT ASYCUDA_Id, CNumber, RegistrationDate, Customs_clearance_office_code, DocumentType, ApplicationSettingsId, ROW_NUMBER() over (partition by cnumber, registrationdate, customs_clearance_office_code order by asycuda_id desc) as rownum
FROM    AsycudaDocument where CNumber is not null) t
where t.rownum > 1)


select * from AsycudaDocument where CNumber = '32356'

select * from xcuda_Registration where Number = '32356'