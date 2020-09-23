
delete from entrydata where ApplicationSettingsId <> 5


delete from SystemDocumentSets where id in (select Asycudadocumentsetid from AsycudaDocumentSet where ApplicationSettingsId <> 5)

delete from AsycudaDocumentSet where ApplicationSettingsId <> 5

delete from InventoryItems where ApplicationSettingsId <> 5

delete from FileTypes where ApplicationSettingsId <> 5

delete from ApplicationSettings where ApplicationSettingsId <> 5

delete from Attachments
delete from [History-Allocations]