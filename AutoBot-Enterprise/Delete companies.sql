


delete from inventoryitems where ApplicationSettingsId <> 2
delete from dbo.SystemDocumentSets where id in ( select AsycudaDocumentSetId from dbo.AsycudaDocumentSet where ApplicationSettingsId <> 2)
delete from dbo.AsycudaDocumentSet where ApplicationSettingsId <> 2
delete from ApplicationSettings where ApplicationSettingsId <> 2
