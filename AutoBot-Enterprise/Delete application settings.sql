declare @appSettingId int = 5

delete from Emails where ApplicationSettingsId = @appSettingId 
delete from EntryData where ApplicationSettingsId = @appSettingId 
delete from InventoryItems where ApplicationSettingsId = @appSettingId
delete from SystemDocumentSets from SystemDocumentSets inner join asycudadocumentset on SystemDocumentSets.id = AsycudaDocumentSet.AsycudaDocumentSetId where ApplicationSettingsId = @appSettingId

delete from [OCR-Invoices]  where ApplicationSettingsId = @appSettingId
delete from FileTypes where ApplicationSettingsId = @appSettingId and ParentFileTypeId is not null
delete from FileTypes where ApplicationSettingsId = @appSettingId
 
delete from InfoMapping where ApplicationSettingsId = @appSettingId 
delete from EmailMapping where ApplicationSettingsId = @appSettingId 
delete from AsycudaDocumentSet  where ApplicationSettingsId = @appSettingId
delete from [ApplicationSettings-Declarants] where ApplicationSettingsId = @appSettingId 
delete from ApplicationSettings where ApplicationSettingsId = @appSettingId


--use [budgetmarine-AutoBot] exec sp_changedbowner 'sa'