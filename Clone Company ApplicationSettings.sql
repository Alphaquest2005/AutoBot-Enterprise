
declare @oldCompany int = 2, @newCompany int = 3



--delete from FileTypes where ApplicationSettingsId = @newCompany 

--INSERT INTO FileTypes
--                 (ApplicationSettingsId, FilePattern, Type, AsycudaDocumentSetId, CreateDocumentSet, DocumentSpecific, DocumentCode, ReplyToMail, FileGroupId, MergeEmails, CopyEntryData, ParentFileTypeId, OverwriteFiles, 
--                 HasFiles, Oldfiletypeid)
--SELECT @newCompany as  ApplicationSettingsId, FilePattern, Type, AsycudaDocumentSetId, CreateDocumentSet, DocumentSpecific, DocumentCode, ReplyToMail, FileGroupId, MergeEmails, CopyEntryData, ParentFileTypeId, OverwriteFiles, 
--                 HasFiles, id as Oldfiletypeid
--FROM    FileTypes AS FileTypes_1 
--where ApplicationSettingsId = @oldCompany

------------------------------------------------

--INSERT INTO EmailFileTypes
--                 (FileTypeId, EmailMappingId, IsRequired)
--SELECT pending.Id, FileTypeActions_1.EmailMappingId, FileTypeActions_1.IsRequired
--FROM    EmailFileTypes AS FileTypeActions_1 INNER JOIN
--                     (SELECT Id, OldFileTypeId
--                      FROM     FileTypes
--                      WHERE  (ApplicationSettingsId = @newCompany)) AS pending ON FileTypeActions_1.FileTypeId = pending.OldFileTypeId


-------------------------------------------------

--INSERT INTO FileTypeActions
--                 (FileTypeId, ActionId, AssessIM7, AssessEX)
--SELECT pending.Id, ActionId, AssessIM7, AssessEX
--FROM    FileTypeActions AS FileTypeActions_1 inner join 
--(SELECT FileTypes.Id, oldFiletypeid
--FROM    FileTypes
--WHERE (FileTypes.ApplicationSettingsId = @newCompany) ) as pending on FileTypeActions_1.FileTypeId = pending.oldFiletypeid


-----------------------------------------------
--INSERT INTO FileTypeMappings
--                 (FileTypeId, OriginalName, DestinationName, DataType, Required)
--SELECT pending.Id, FileTypeActions_1.OriginalName, FileTypeActions_1.DestinationName, FileTypeActions_1.DataType, FileTypeActions_1.Required
--FROM    FileTypeMappings AS FileTypeActions_1 INNER JOIN
--                     (SELECT Id, OldFileTypeId
--                      FROM     FileTypes
--                      WHERE  (ApplicationSettingsId = @newCompany)) AS pending ON FileTypeActions_1.FileTypeId = pending.OldFileTypeId

------------------------------------------------

--INSERT INTO FileTypeContacts
--                 (FileTypeId, ContactId)
--SELECT pending.Id, FileTypeActions_1.ContactId
--FROM    FileTypeContacts AS FileTypeActions_1 INNER JOIN
--                     (SELECT Id, OldFileTypeId
--                      FROM     FileTypes
--                      WHERE  (ApplicationSettingsId = @newCompany)) AS pending ON FileTypeActions_1.FileTypeId = pending.OldFileTypeId

-------------------------------------------------
--Export Template
-------------------------------------------------


--SELECT FileTypes.Id, newFileTypes.Id AS newFiletypeId
--FROM    FileTypes INNER JOIN
--                 FileTypes AS newFileTypes ON FileTypes.FilePattern = newFileTypes.FilePattern
--WHERE (FileTypes.ApplicationSettingsId = 2) AND (newFileTypes.ApplicationSettingsId = 3)

-----------------------

--delete from FileTypes where ApplicationSettingsId = 3

----------------------

--DELETE FROM FileTypeActions
--FROM    FileTypeActions INNER JOIN
--                 FileTypes ON FileTypeActions.FileTypeId = FileTypes.Id
--WHERE (FileTypes.ApplicationSettingsId = 3)

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