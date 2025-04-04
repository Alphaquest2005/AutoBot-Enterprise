USE [WebSource-AutoBot];
GO

PRINT 'Starting comprehensive fix script...';

-- Ensure ApplicationSettingsId = 3 exists (used in InfoMapping) - adjust if needed
IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationSettings WHERE ApplicationSettingsId = 3)
BEGIN
    PRINT 'WARNING: ApplicationSettingsId = 3 not found. InfoMapping for ConsigneeName might fail or use incorrect settings.';
    -- Consider adding an INSERT statement here if appropriate, or handle the error.
END
GO

-- 1. Ensure Actions exist
PRINT 'Checking/Creating Actions...';
SET IDENTITY_INSERT [dbo].[Actions] ON;
IF NOT EXISTS (SELECT 1 FROM dbo.Actions WHERE Id = 15 AND Name = N'SaveInfo')
BEGIN
    -- If SaveInfo is missing entirely, this is unexpected. Add it.
    INSERT INTO [dbo].[Actions] ([Id], [Name], [TestMode], [IsDataSpecific]) 
    VALUES (15, N'SaveInfo', 0, 1);
    PRINT 'Action SaveInfo (Id 15) created.';
END
IF NOT EXISTS (SELECT 1 FROM dbo.Actions WHERE Id = 121)
BEGIN
    INSERT INTO [dbo].[Actions] ([Id], [Name], [TestMode], [IsDataSpecific]) 
    VALUES (121, N'SyncConsigneeInDB', 0, 1); -- Assuming IsDataSpecific = 1 based on previous analysis
    PRINT 'Action SyncConsigneeInDB (Id 121) created.';
END
ELSE
BEGIN
    -- Ensure existing action has correct properties (optional, but good practice)
    UPDATE dbo.Actions 
    SET Name = N'SyncConsigneeInDB', TestMode = 0, IsDataSpecific = 1 
    WHERE Id = 121 AND (Name != N'SyncConsigneeInDB' OR TestMode != 0 OR IsDataSpecific != 1);
    PRINT 'Action SyncConsigneeInDB (Id 121) verified/updated.';
END
SET IDENTITY_INSERT [dbo].[Actions] OFF;
GO

-- 2. Ensure FileTypeActions links exist for FileTypeId = 109
PRINT 'Checking/Creating FileTypeActions links for FileTypeId 109...';
IF NOT EXISTS (SELECT 1 FROM dbo.FileTypeActions WHERE FileTypeId = 109 AND ActionId = 15)
BEGIN
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX])
    VALUES (109, 15, NULL, NULL);
    PRINT 'FileTypeAction link created for FileTypeId 109 -> SaveInfo (ActionId 15).';
END
ELSE
BEGIN
    PRINT 'FileTypeAction link exists for FileTypeId 109 -> SaveInfo (ActionId 15).';
END

IF NOT EXISTS (SELECT 1 FROM dbo.FileTypeActions WHERE FileTypeId = 109 AND ActionId = 121)
BEGIN
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX])
    VALUES (109, 121, NULL, NULL);
    PRINT 'FileTypeAction link created for FileTypeId 109 -> SyncConsigneeInDB (ActionId 121).';
END
ELSE
BEGIN
    PRINT 'FileTypeAction link exists for FileTypeId 109 -> SyncConsigneeInDB (ActionId 121).';
END
GO

-- 3. Ensure InfoMapping for Consignee Name exists
PRINT 'Checking/Creating InfoMapping for Consignee Name (Id 1046)...';
SET IDENTITY_INSERT [dbo].[InfoMapping] ON;
IF NOT EXISTS (SELECT 1 FROM dbo.InfoMapping WHERE Id = 1046)
BEGIN
    INSERT INTO [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) 
    VALUES (1046, N'Consignee Name', N'ConsigneeName', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId'); -- Assuming AppSettingsId=3
    PRINT 'InfoMapping for Consignee Name (Id 1046) created.';
END
ELSE
BEGIN
    -- Ensure existing mapping has correct properties
    UPDATE dbo.InfoMapping 
    SET [Key] = N'Consignee Name', 
        [Field] = N'ConsigneeName', 
        [EntityType] = N'AsycudaDocumentSet', 
        [ApplicationSettingsId] = 3, -- Assuming AppSettingsId=3
        [EntityKeyField] = N'AsycudaDocumentSetId'
    WHERE Id = 1046;
    PRINT 'InfoMapping for Consignee Name (Id 1046) verified/updated.';
END
SET IDENTITY_INSERT [dbo].[InfoMapping] OFF;
GO

-- 4. Ensure EmailInfoMappings link exists with UpdateDatabase = 1
PRINT 'Checking/Updating EmailInfoMappings link (EmailMappingId 22 -> InfoMappingId 1046)...';
-- Assuming EmailMappingId = 22 is correct based on previous scripts. Adjust if necessary.
IF NOT EXISTS (SELECT 1 FROM dbo.EmailMapping WHERE Id = 22)
BEGIN
    PRINT 'WARNING: EmailMappingId = 22 not found. Cannot create/update EmailInfoMappings link.';
END
ELSE
BEGIN
    MERGE INTO dbo.EmailInfoMappings AS Target
    USING (SELECT EmailMappingId = 22, InfoMappingId = 1046, UpdateDatabase = 1) AS Source
    ON Target.EmailMappingId = Source.EmailMappingId AND Target.InfoMappingId = Source.InfoMappingId
    WHEN MATCHED AND Target.UpdateDatabase != Source.UpdateDatabase THEN
        UPDATE SET Target.UpdateDatabase = Source.UpdateDatabase
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (EmailMappingId, InfoMappingId, UpdateDatabase)
        VALUES (Source.EmailMappingId, Source.InfoMappingId, Source.UpdateDatabase);
    PRINT 'EmailInfoMappings link for Consignee Name ensured with UpdateDatabase = 1.';
END
GO

-- 5. Ensure InfoMappingRegEx for Consignee Name exists and is correct
PRINT 'Checking/Updating InfoMappingRegEx for Consignee Name (InfoMappingId 1046)...';
-- Use MERGE to handle create or update based on InfoMappingId
MERGE INTO dbo.InfoMappingRegEx AS Target
USING (SELECT 
            InfoMappingId = 1046, 
            KeyRegX = N'.*', -- Use non-restrictive KeyRegX
            FieldRx = N'^\s*Consignee Name\s*[:# ]\s*(?<Value>.+)', -- Use simpler FieldRx
            LineRegx = N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))' -- Keep original LineRegx
       ) AS Source
ON Target.InfoMappingId = Source.InfoMappingId -- Match based on the mapping it applies to
WHEN MATCHED AND (Target.KeyRegX != Source.KeyRegX OR Target.FieldRx != Source.FieldRx) THEN 
    UPDATE SET 
        Target.KeyRegX = Source.KeyRegX,
        Target.FieldRx = Source.FieldRx
        -- Optionally update LineRegx if needed: Target.LineRegx = Source.LineRegx 
WHEN NOT MATCHED BY TARGET THEN
    INSERT (InfoMappingId, KeyRegX, FieldRx, LineRegx) -- Add other columns if they have defaults or are needed
    VALUES (Source.InfoMappingId, Source.KeyRegX, Source.FieldRx, Source.LineRegx);
PRINT 'InfoMappingRegEx for Consignee Name ensured with correct regex.';
GO

PRINT 'Comprehensive fix script finished.';
GO
