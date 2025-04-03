-- Add FileTypeActions for the new ShipmentFolder FileType (ID 1186)
-- Sequence: ImportInfo -> Xlsx2csv -> SaveCsv -> AttachToDocSetByRef -> ExportPOEntries

-- Action IDs:
-- 122: ImportShipmentInfoFromTxt (New)
-- 13:  Xlsx2csv
-- 6:   SaveCsv (Corrected: Imports data from CSV)
-- 51:  AttachToDocSetByRef
-- 8:   ExportPOEntries

-- Assuming ApplicationSettingsId = 3 -- Verify this is correct for your target environment

-- Link Action 122 (ImportShipmentInfoFromTxt)
IF EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1186) AND EXISTS (SELECT 1 FROM [dbo].[Actions] WHERE Id = 122)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypeActions] WHERE FileTypeId = 1186 AND ActionId = 122)
    BEGIN
        INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1186, 122, NULL, NULL);
        PRINT 'Linked Action ImportShipmentInfoFromTxt (122) to FileType ShipmentFolder (1186)';
    END
    ELSE BEGIN PRINT 'Link already exists for FileType 1186 and Action 122.'; END
END
ELSE BEGIN PRINT 'Prerequisite FileType 1186 or Action 122 not found.'; END
GO

-- Link Action 13 (Xlsx2csv)
IF EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1186) AND EXISTS (SELECT 1 FROM [dbo].[Actions] WHERE Id = 13)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypeActions] WHERE FileTypeId = 1186 AND ActionId = 13)
    BEGIN
        INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1186, 13, NULL, NULL);
        PRINT 'Linked Action Xlsx2csv (13) to FileType ShipmentFolder (1186)';
    END
    ELSE BEGIN PRINT 'Link already exists for FileType 1186 and Action 13.'; END
END
ELSE BEGIN PRINT 'Prerequisite FileType 1186 or Action 13 not found.'; END
GO

-- Link Action 6 (SaveCsv)
IF EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1186) AND EXISTS (SELECT 1 FROM [dbo].[Actions] WHERE Id = 6)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypeActions] WHERE FileTypeId = 1186 AND ActionId = 6)
    BEGIN
        INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1186, 6, NULL, NULL);
        PRINT 'Linked Action SaveCsv (6) to FileType ShipmentFolder (1186)';
    END
    ELSE BEGIN PRINT 'Link already exists for FileType 1186 and Action 6.'; END
END
ELSE BEGIN PRINT 'Prerequisite FileType 1186 or Action 6 not found.'; END
GO

-- Link Action 51 (AttachToDocSetByRef)
IF EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1186) AND EXISTS (SELECT 1 FROM [dbo].[Actions] WHERE Id = 51)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypeActions] WHERE FileTypeId = 1186 AND ActionId = 51)
    BEGIN
        INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1186, 51, NULL, NULL);
        PRINT 'Linked Action AttachToDocSetByRef (51) to FileType ShipmentFolder (1186)';
    END
    ELSE BEGIN PRINT 'Link already exists for FileType 1186 and Action 51.'; END
END
ELSE BEGIN PRINT 'Prerequisite FileType 1186 or Action 51 not found.'; END
GO

-- Link Action 8 (ExportPOEntries)
IF EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1186) AND EXISTS (SELECT 1 FROM [dbo].[Actions] WHERE Id = 8)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypeActions] WHERE FileTypeId = 1186 AND ActionId = 8)
    BEGIN
        INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1186, 8, NULL, NULL);
        PRINT 'Linked Action ExportPOEntries (8) to FileType ShipmentFolder (1186)';
    END
    ELSE BEGIN PRINT 'Link already exists for FileType 1186 and Action 8.'; END
END
ELSE BEGIN PRINT 'Prerequisite FileType 1186 or Action 8 not found.'; END
GO