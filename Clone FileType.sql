
declare @EntryType nvarchar(50) = 'Sales', 
		@FormatType nvarchar(50) = 'CSV'
drop table #oldFileTypeInfo
select oldFileType.id, oldfiletype.FilePattern, oldfiletype.Type, oldFileInfo.EntryType, oldFileInfo.Format
into #oldFileTypeInfo
from [BudgetMarine-AutoBot].dbo.FileTypes oldFileType inner join [BudgetMarine-AutoBot].dbo.[FileTypes-FileImporterInfo] oldFileInfo on oldFileType.FileInfoId = oldFileInfo.Id where oldFileInfo.EntryType = @EntryType and oldFileInfo.Format = @FormatType 

select * from #oldFileTypeInfo

declare @oldCompany int = 2, @newCompany int = 7, 
@oldFileTypeId int = 125



--delete from FileTypes where ApplicationSettingsId = @newCompany 

INSERT INTO FileTypes
                 (ApplicationSettingsId, FilePattern, Type, AsycudaDocumentSetId, CreateDocumentSet, DocumentSpecific, DocumentCode, ReplyToMail, FileGroupId, MergeEmails, CopyEntryData, ParentFileTypeId, OverwriteFiles, 
                 HasFiles, Oldfiletypeid)
SELECT @newCompany as  ApplicationSettingsId, FilePattern, Type, AsycudaDocumentSetId, CreateDocumentSet, DocumentSpecific, DocumentCode, ReplyToMail, FileGroupId, MergeEmails, CopyEntryData, ParentFileTypeId, OverwriteFiles, 
                 HasFiles, id as Oldfiletypeid
FROM    [BudgetMarine-AutoBot].dbo.FileTypes AS FileTypes_1 
where ApplicationSettingsId = @oldCompany and FileTypes_1.Id = @oldFileTypeId

declare @newFileTypeId int = 189

--/////////////////////// make sure you put the old filetypeid in new filetype row
----------------------------------------------

---////insert email mappings too 

INSERT INTO EmailFileTypes
                 (FileTypeId, EmailMappingId, IsRequired)
SELECT pending.Id, FileTypeActions_1.EmailMappingId, FileTypeActions_1.IsRequired
FROM    EmailFileTypes AS FileTypeActions_1 INNER JOIN
                     (SELECT Id, OldFileTypeId
                      FROM     FileTypes
                      WHERE  (id = @newFileTypeId)) AS pending ON FileTypeActions_1.FileTypeId = pending.OldFileTypeId


-----------------------------------------------



INSERT INTO FileTypeActions
                 (FileTypeId, ActionId, AssessIM7, AssessEX)
SELECT pending.Id, ActionId, AssessIM7, AssessEX
FROM    [BudgetMarine-AutoBot].dbo.FileTypeActions AS FileTypeActions_1 inner join 
(SELECT FileTypes.Id, oldFiletypeid
FROM    FileTypes
WHERE (id = @newFileTypeId) ) as pending on FileTypeActions_1.FileTypeId = pending.oldFiletypeid


---------------------------------------------
INSERT INTO FileTypeMappings
                 (FileTypeId, OriginalName, DestinationName, DataType, Required)
SELECT pending.Id, FileTypeActions_1.OriginalName, FileTypeActions_1.DestinationName, FileTypeActions_1.DataType, FileTypeActions_1.Required
FROM    FileTypeMappings AS FileTypeActions_1 INNER JOIN
                     (SELECT Id, OldFileTypeId
                      FROM     FileTypes
                      WHERE  (id = @newFileTypeId)) AS pending ON FileTypeActions_1.FileTypeId = pending.OldFileTypeId

----------------------------------------------

INSERT INTO FileTypeContacts
                 (FileTypeId, ContactId)
SELECT pending.Id, FileTypeActions_1.ContactId
FROM    FileTypeContacts AS FileTypeActions_1 INNER JOIN
                     (SELECT Id, OldFileTypeId
                      FROM     FileTypes
                      WHERE  (id = @newFileTypeId)) AS pending ON FileTypeActions_1.FileTypeId = pending.OldFileTypeId


-------------------------------------------------

-----------------delete duplicate rows

--WITH cte AS (
--    SELECT 
--        FileTypeId, ActionId, AssessIM7, AssessEX, 
--        ROW_NUMBER() OVER (
--            PARTITION BY 
--                FileTypeId, ActionId, AssessIM7, AssessEX
--            ORDER BY 
--               FileTypeId, ActionId, AssessIM7, AssessEX
--        ) row_num
--     FROM 
--        FileTypeActions
--)
--DELETE FROM cte
--WHERE row_num > 1;


