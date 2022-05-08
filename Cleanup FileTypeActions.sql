
delete from FileTypeActions
where id not in
(SELECT FileTypeActions.Id
FROM    FileTypes INNER JOIN
                 FileTypeActions ON FileTypes.Id = FileTypeActions.FileTypeId)