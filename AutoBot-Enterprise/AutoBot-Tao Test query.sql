set statistics time, io on
select * from ObjectView --where id = 21079

set statistics time,io on
select EntryData_Id,
LineNumber,
Cost,
EntryDataId,
ItemNumber,
ItemDescription,
EntryDataDetailsKey from [AutoBot-EnterpriseDB].dbo.entrydatadetails --where EntryDataDetailsId = 955054