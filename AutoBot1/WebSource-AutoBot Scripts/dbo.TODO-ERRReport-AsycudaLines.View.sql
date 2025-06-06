USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ERRReport-AsycudaLines]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-ERRReport-AsycudaLines]
AS
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Long Product Codes' AS Error, 'Length: ' + CAST(Length AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-LongProductCodes]
union
SELECT ApplicationSettingsId, CNumber, Reference as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Missing Supplimentary Unit' AS Error, 'TariffCode: ' + CAST(TariffCode AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-MissingSupplimentaryUnit]
union
SELECT ApplicationSettingsId, CNumber, Reference as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Missing Item Code' AS Error, 'TariffCode: ' + CAST(TariffCode AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-NullItemNumber]
union
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'WarehouseError' AS Error, 'Warehouse: ' + warehouseerror AS Info
FROM    dbo.[TODO-Error-WarehouseErrors]
union
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Unlinked PreviousItems' AS Error, 'PreviousItemId: ' + CAST(PreviousItem_id as nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-UnlinkedPreviousItems]
union
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'ExpiredLines' AS Error, 'ExpiryDate: ' + CAST(ExpiryDate as nvarchar(10)) AS Info
FROM    dbo.[TODO-Error-ExpiredLines]

GO
