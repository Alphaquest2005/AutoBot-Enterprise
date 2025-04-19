
declare @appSettingsId int =6

delete from entrydata where ApplicationSettingsId = @appSettingsId


delete from SystemDocumentSets where id in (select Asycudadocumentsetid from AsycudaDocumentSet where ApplicationSettingsId = @appSettingsId)

delete from AsycudaDocumentSet where ApplicationSettingsId = @appSettingsId
delete from InventoryItems where ApplicationSettingsId = @appSettingsId

delete from FileTypes where ApplicationSettingsId = @appSettingsId

delete from [Ocr-invoices] where ApplicationSettingsId = @appSettingsId

delete from ApplicationSettings where ApplicationSettingsId = @appSettingsId

--delete from Attachments where ApplicationSettingsId = @appSettingsId

delete from [History-Allocations] where ApplicationSettingsId = @appSettingsId