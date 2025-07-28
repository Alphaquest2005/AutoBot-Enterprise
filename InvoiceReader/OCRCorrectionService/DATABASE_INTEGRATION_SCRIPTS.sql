-- ====================================================================
-- OCR TEMPLATE TABLE MAPPING DATABASE INTEGRATION SCRIPTS
-- ====================================================================
-- Purpose: Connect FileTypeManager.EntryTypes to OCR template system
-- Connects: FileTypes.FileImporterInfo.EntryType → OCR_TemplateTableMapping
-- Links: Template_Specifications.md EntityTypes to actual FileTypeId values
-- ====================================================================

-- **STEP 1: ANALYZE CURRENT OCR-INVOICES TABLE STRUCTURE**
-- First, let's see what FileTypeId values we have in the OCR-Invoices table
SELECT DISTINCT 
    oi.[FileTypeId],
    oi.[Name] as InvoiceName,
    COUNT(*) as TemplateCount
FROM [WebSource-AutoBot-Original].[dbo].[OCR-Invoices] oi
WHERE oi.[IsActive] = 1
GROUP BY oi.[FileTypeId], oi.[Name]
ORDER BY oi.[FileTypeId];

-- **STEP 2: ANALYZE FILETYPES TABLE TO SEE ENTRYTYPE MAPPINGS**
-- Check what EntryType values exist in FileTypes.FileImporterInfo
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    ft.[Type] as FileTypeName,
    fii.[EntryType],
    fii.[Format],
    COUNT(*) as UsageCount
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE ft.[ApplicationSettingsId] = 1 -- Adjust as needed
    AND fii.[EntryType] IS NOT NULL
GROUP BY ft.[Id], ft.[Type], fii.[EntryType], fii.[Format]
ORDER BY ft.[Id];

-- **STEP 3: CREATE OR UPDATE OCR_TEMPLATETABLEMAPPING WITH FILETYPE CONNECTIONS**
-- Ensure OCR_TemplateTableMapping table exists with proper structure
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OCR_TemplateTableMapping')
BEGIN
    CREATE TABLE [dbo].[OCR_TemplateTableMapping] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [FileTypeId] INT NOT NULL,
        [DocumentType] NVARCHAR(50) NOT NULL, -- Maps to FileTypeManager.EntryTypes
        [PrimaryEntityType] NVARCHAR(50) NOT NULL, -- Primary EntityType for this document type
        [SecondaryEntityTypes] NVARCHAR(500) NULL, -- Comma-separated additional EntityTypes
        [RequiredFields] NVARCHAR(1000) NULL, -- Comma-separated required fields for this document type
        [ValidationRules] NVARCHAR(MAX) NULL, -- JSON or XML with validation rules
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [UpdatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [ApplicationSettingsId] INT NOT NULL DEFAULT 1,
        
        -- Foreign key constraints
        CONSTRAINT [FK_OCR_TemplateTableMapping_FileTypeId] 
            FOREIGN KEY ([FileTypeId]) REFERENCES [CoreEntities].[dbo].[FileTypes]([Id]),
        
        -- Unique constraint to prevent duplicates
        CONSTRAINT [UK_OCR_TemplateTableMapping_FileType_DocType] 
            UNIQUE ([FileTypeId], [DocumentType], [ApplicationSettingsId])
    );
    
    -- Create indexes for performance
    CREATE NONCLUSTERED INDEX [IX_OCR_TemplateTableMapping_FileTypeId] 
        ON [dbo].[OCR_TemplateTableMapping] ([FileTypeId]);
    CREATE NONCLUSTERED INDEX [IX_OCR_TemplateTableMapping_DocumentType] 
        ON [dbo].[OCR_TemplateTableMapping] ([DocumentType]);
    CREATE NONCLUSTERED INDEX [IX_OCR_TemplateTableMapping_Active] 
        ON [dbo].[OCR_TemplateTableMapping] ([IsActive]) WHERE [IsActive] = 1;
END;

-- **STEP 4: INSERT DOCUMENT TYPE MAPPINGS BASED ON TEMPLATE_SPECIFICATIONS.MD**
-- Map FileTypeManager.EntryTypes to EntityTypes from Template_Specifications.md

-- Clear existing mappings (optional - comment out if you want to preserve)
-- DELETE FROM [dbo].[OCR_TemplateTableMapping] WHERE [ApplicationSettingsId] = 1;

-- **INVOICE DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'Inv' as DocumentType,
    'Invoice' as PrimaryEntityType,
    'InvoiceDetails,EntryData,EntryDataDetails' as SecondaryEntityTypes,
    'InvoiceNo,InvoiceTotal,SupplierCode' as RequiredFields,
    '{"entityTypes":["Invoice","InvoiceDetails"],"dataTypes":{"InvoiceTotal":"Numeric","InvoiceDate":"Date"},"businessRules":{"InvoiceTotal":{"min":0}}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'INV'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'Inv'
    );

-- **SHIPMENT INVOICE DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'ShipmentInvoice' as DocumentType,
    'Invoice' as PrimaryEntityType,
    'InvoiceDetails,EntryData,EntryDataDetails' as SecondaryEntityTypes,
    'InvoiceNo,InvoiceTotal,SupplierCode' as RequiredFields,
    '{"entityTypes":["Invoice","InvoiceDetails"],"dataTypes":{"InvoiceTotal":"Numeric","InvoiceDate":"Date"},"businessRules":{"InvoiceTotal":{"min":0}}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'Shipment Invoice'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'ShipmentInvoice'
    );

-- **BILL OF LADING DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'BL' as DocumentType,
    'ShipmentBL' as PrimaryEntityType,
    'ShipmentBLDetails' as SecondaryEntityTypes,
    'BLNumber,WeightKG' as RequiredFields,
    '{"entityTypes":["ShipmentBL","ShipmentBLDetails"],"dataTypes":{"WeightKG":"Numeric","VolumeM3":"Numeric"},"businessRules":{"WeightKG":{"min":0}}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'BL'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'BL'
    );

-- **PURCHASE ORDER DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'Po' as DocumentType,
    'PurchaseOrders' as PrimaryEntityType,
    'PurchaseOrderDetails' as SecondaryEntityTypes,
    'PONumber,LineNumber,Quantity' as RequiredFields,
    '{"entityTypes":["PurchaseOrders","PurchaseOrderDetails"],"dataTypes":{"Quantity":"Numeric","UnitPrice":"Numeric"},"businessRules":{"Quantity":{"min":0}}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'PO'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'Po'
    );

-- **FREIGHT DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'Freight' as DocumentType,
    'ShipmentFreight' as PrimaryEntityType,
    'ShipmentFreightDetails' as SecondaryEntityTypes,
    'FreightInvoiceNo,FreightTotal,CarrierCode' as RequiredFields,
    '{"entityTypes":["ShipmentFreight","ShipmentFreightDetails"],"dataTypes":{"FreightTotal":"Numeric"},"businessRules":{"FreightTotal":{"min":0}}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'Freight'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'Freight'
    );

-- **MANIFEST DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'Manifest' as DocumentType,
    'ShipmentManifest' as PrimaryEntityType,
    NULL as SecondaryEntityTypes,
    'ManifestNumber' as RequiredFields,
    '{"entityTypes":["ShipmentManifest"],"dataTypes":{"ManifestNumber":"String"},"businessRules":{}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'Manifest'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'Manifest'
    );

-- **SIMPLIFIED DECLARATION DOCUMENTS MAPPING**
INSERT INTO [dbo].[OCR_TemplateTableMapping] 
    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId])
SELECT DISTINCT 
    ft.[Id] as FileTypeId,
    'SimplifiedDeclaration' as DocumentType,
    'SimplifiedDeclaration' as PrimaryEntityType,
    'ExtraInfo' as SecondaryEntityTypes,
    'DeclarationNumber' as RequiredFields,
    '{"entityTypes":["SimplifiedDeclaration","ExtraInfo"],"dataTypes":{"DeclarationNumber":"String"},"businessRules":{}}' as ValidationRules,
    1 as ApplicationSettingsId
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE fii.[EntryType] = 'Simplified Declaration'
    AND ft.[ApplicationSettingsId] = 1
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] ottm 
        WHERE ottm.[FileTypeId] = ft.[Id] AND ottm.[DocumentType] = 'SimplifiedDeclaration'
    );

-- **STEP 5: CREATE VIEW FOR EASY ACCESS TO TEMPLATE MAPPINGS**
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_OCR_TemplateTableMapping')
    DROP VIEW [dbo].[vw_OCR_TemplateTableMapping];
GO

CREATE VIEW [dbo].[vw_OCR_TemplateTableMapping] AS
SELECT 
    ottm.[Id],
    ottm.[FileTypeId],
    ft.[Type] as FileTypeName,
    fii.[EntryType] as FileImporterEntryType,
    fii.[Format] as FileFormat,
    ottm.[DocumentType] as EntryTypeConstant,
    ottm.[PrimaryEntityType],
    ottm.[SecondaryEntityTypes],
    ottm.[RequiredFields],
    ottm.[ValidationRules],
    ottm.[IsActive],
    ottm.[CreatedDate],
    ottm.[UpdatedDate],
    ottm.[ApplicationSettingsId],
    -- Count of OCR-Invoices using this FileTypeId
    (SELECT COUNT(*) FROM [WebSource-AutoBot-Original].[dbo].[OCR-Invoices] oi 
     WHERE oi.[FileTypeId] = ottm.[FileTypeId] AND oi.[IsActive] = 1) as TemplateCount
FROM [dbo].[OCR_TemplateTableMapping] ottm
    INNER JOIN [CoreEntities].[dbo].[FileTypes] ft ON ottm.[FileTypeId] = ft.[Id]
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
WHERE ottm.[IsActive] = 1;
GO

-- **STEP 6: VERIFICATION QUERIES**
-- Verify the mappings were created correctly
SELECT 'Verification: OCR Template Table Mappings Created' as QueryPurpose;
SELECT * FROM [dbo].[vw_OCR_TemplateTableMapping] ORDER BY [FileTypeId];

-- Check for any FileTypes that don't have template mappings
SELECT 'FileTypes without Template Mappings' as QueryPurpose;
SELECT 
    ft.[Id] as FileTypeId,
    ft.[Type] as FileTypeName,
    fii.[EntryType],
    fii.[Format]
FROM [CoreEntities].[dbo].[FileTypes] ft
    INNER JOIN [CoreEntities].[dbo].[FileImporterInfos] fii ON ft.[FileImporterInfosId] = fii.[Id]
    LEFT JOIN [dbo].[OCR_TemplateTableMapping] ottm ON ft.[Id] = ottm.[FileTypeId] AND ottm.[IsActive] = 1
WHERE ft.[ApplicationSettingsId] = 1 
    AND fii.[EntryType] IS NOT NULL
    AND ottm.[Id] IS NULL
ORDER BY ft.[Id];

-- Show template usage statistics
SELECT 'Template Usage Statistics' as QueryPurpose;
SELECT 
    ottm.*,
    COUNT(oi.[Id]) as ActiveTemplateCount
FROM [dbo].[OCR_TemplateTableMapping] ottm
    LEFT JOIN [WebSource-AutoBot-Original].[dbo].[OCR-Invoices] oi ON ottm.[FileTypeId] = oi.[FileTypeId] AND oi.[IsActive] = 1
WHERE ottm.[IsActive] = 1
GROUP BY ottm.[Id], ottm.[FileTypeId], ottm.[DocumentType], ottm.[PrimaryEntityType], 
         ottm.[SecondaryEntityTypes], ottm.[RequiredFields], ottm.[ValidationRules], 
         ottm.[IsActive], ottm.[CreatedDate], ottm.[UpdatedDate], ottm.[ApplicationSettingsId]
ORDER BY ottm.[FileTypeId];

-- ====================================================================
-- USAGE NOTES:
-- 1. Run these scripts in your SQL Server Management Studio
-- 2. Adjust ApplicationSettingsId values as needed for your environment
-- 3. Review the verification queries to ensure mappings are correct
-- 4. The ValidationRules column contains JSON for future extensibility
-- 5. Use vw_OCR_TemplateTableMapping view for easy access in code
-- ====================================================================