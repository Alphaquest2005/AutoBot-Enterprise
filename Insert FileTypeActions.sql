declare @oldCompany int = 2, @newCompany int = 6, 
 @EntryType nvarchar(50) = 'Sales', 
		@FormatType nvarchar(50) = 'CSV'
drop table #oldFileTypeInfo
select oldFileType.id, oldfiletype.FilePattern, oldfiletype.Type, oldFileInfo.EntryType, oldFileInfo.Format
into #oldFileTypeInfo
from [BudgetMarine-AutoBot].dbo.FileTypes oldFileType inner join [BudgetMarine-AutoBot].dbo.[FileTypes-FileImporterInfo] oldFileInfo on oldFileType.FileInfoId = oldFileInfo.Id where oldFileInfo.EntryType = @EntryType and oldFileInfo.Format = @FormatType AND (oldFileType.ApplicationSettingsId = @oldCompany)

select * from #oldFileTypeInfo




drop table #newFileTypeInfo
SELECT oldFileType.Id, oldFileType.FilePattern, oldFileType.Type, oldFileInfo.EntryType, oldFileInfo.Format
INTO       [#newFileTypeInfo]
FROM    FileTypes AS oldFileType INNER JOIN
                 [FileTypes-FileImporterInfo] AS oldFileInfo ON oldFileType.FileInfoId = oldFileInfo.Id
WHERE (oldFileInfo.EntryType = @EntryType) AND (oldFileInfo.Format = @FormatType) AND (oldFileType.ApplicationSettingsId = @newCompany)
select * from #newFileTypeInfo


declare @oldFileTypeId int = 125, @newFileTypeId int = 127

drop table #oldFileTypeActions
select * into #oldFileTypeActions from [BudgetMarine-AutoBot].dbo.[FileTypeActions] where FileTypeId = @oldFileTypeId

select * from #oldFileTypeActions

INSERT INTO FileTypeActions
                 (FileTypeId, ActionId, AssessIM7, AssessEX)
SELECT @newFileTypeId, ActionId, AssessIM7, AssessEX
FROM    #oldFileTypeActions 


select * from FileTypeActions