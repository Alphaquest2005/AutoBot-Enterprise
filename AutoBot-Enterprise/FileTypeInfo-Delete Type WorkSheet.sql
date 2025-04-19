update fileTypes
set Description = Type


update FileTypes
set FileInfoId = (select id from [FileTypes-FileImporterInfo] where EntryType = 'Info' and Format = 'Txt')
where Description = 'Info'