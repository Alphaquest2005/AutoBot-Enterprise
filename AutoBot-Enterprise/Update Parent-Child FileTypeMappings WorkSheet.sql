SELECT ChildFileType.Id AS ChildFileTypeId, ParentFileType.Id AS ParentFileTypeId, ParentFileTypeMapping.DestinationName as ParentDestinationName, childFileTypeMapping.DestinationName AS ChildDestinationName
into #orignalSet
FROM     FileTypeMappings AS ParentFileTypeMapping INNER JOIN
                  FileTypes AS ParentFileType ON ParentFileTypeMapping.FileTypeId = ParentFileType.Id INNER JOIN
                  FileTypeMappings AS childFileTypeMapping INNER JOIN
                  FileTypes AS ChildFileType ON childFileTypeMapping.FileTypeId = ChildFileType.Id ON ParentFileType.Id = ChildFileType.ParentFileTypeId AND ParentFileTypeMapping.DestinationName = childFileTypeMapping.OriginalName
where ParentFileTypeMapping.DestinationName <> childFileTypeMapping.DestinationName

select * from #orignalSet

--- update parent destination name

update FileTypeMappings
set DestinationName = #orignalSet.ChildDestinationName
from FileTypeMappings inner join #orignalSet on FileTypeMappings.FileTypeId = #orignalSet.ParentFileTypeId and FileTypeMappings.DestinationName = #orignalSet.ParentDestinationName


---- add destination name as original name to childfiletype

insert into FileTypeMappings(FileTypeId, OriginalName, DestinationName, DataType, Required, Comments)
Select distinct #orignalSet.ChildFileTypeId, #orignalSet.ChildDestinationName, #orignalSet.ChildDestinationName, parentMappings.DataType, parentMappings.Required, parentMappings.Comments
from FileTypeMappings as childMappings right outer join #orignalSet on childMappings.FileTypeId = #orignalSet.ChildFileTypeId and childMappings.OriginalName = #orignalSet.ChildDestinationName
inner join FileTypeMappings as parentMappings on parentMappings.FileTypeId = #orignalSet.ParentFileTypeId and parentMappings.OriginalName = #orignalSet.ParentDestinationName
where childMappings.FileTypeId is null


SELECT ChildFileType.Id AS ChildFileTypeId, ParentFileType.Id AS ParentFileTypeId, ParentFileTypeMapping.DestinationName as ParentDestinationName, childFileTypeMapping.DestinationName AS ChildDestinationName
FROM     FileTypeMappings AS ParentFileTypeMapping INNER JOIN
                  FileTypes AS ParentFileType ON ParentFileTypeMapping.FileTypeId = ParentFileType.Id INNER JOIN
                  FileTypeMappings AS childFileTypeMapping INNER JOIN
                  FileTypes AS ChildFileType ON childFileTypeMapping.FileTypeId = ChildFileType.Id ON ParentFileType.Id = ChildFileType.ParentFileTypeId AND ParentFileTypeMapping.DestinationName = childFileTypeMapping.OriginalName
where ParentFileTypeMapping.DestinationName = childFileTypeMapping.DestinationName