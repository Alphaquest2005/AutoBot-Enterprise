USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ERRReport-SubmitWarehouseErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-ERRReport-SubmitWarehouseErrors]
AS
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Long Product Codes' AS Error, 'Length: ' + CAST(Length AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-LongProductCodes] where CNumber is not null
union
SELECT ApplicationSettingsId, CNumber, Reference as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Missing Supplimentary Unit' AS Error, 'TariffCode: ' + CAST(TariffCode AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-MissingSupplimentaryUnit] where CNumber is not null
union
SELECT ApplicationSettingsId, CNumber, Reference as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'Missing Item Code' AS Error, 'TariffCode: ' + CAST(TariffCode AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-NullItemNumber] where CNumber is not null
union
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description, 'WarehouseError' AS Error, 'Warehouse: ' + warehouseerror AS Info
FROM    dbo.[TODO-Error-WarehouseErrors] where CNumber is not null
GO
