USE [WebSource-AutoBot];
GO

PRINT 'Reordering actions for FileTypeId 109...';

-- Ensure the target actions exist (redundant if previous script ran, but safe)
IF NOT EXISTS (SELECT 1 FROM dbo.Actions WHERE Id = 15)
    PRINT 'WARNING: Action Id 15 (SaveInfo) not found!';
IF NOT EXISTS (SELECT 1 FROM dbo.Actions WHERE Id = 121)
    PRINT 'WARNING: Action Id 121 (SyncConsigneeInDB) not found!';
-- Add checks for other action IDs if necessary

BEGIN TRANSACTION;

BEGIN TRY
    -- Delete existing actions for FileTypeId 109
    DELETE FROM dbo.FileTypeActions WHERE FileTypeId = 109;
    PRINT 'Deleted existing actions for FileTypeId 109.';

    -- Re-insert in the desired order
    -- 1. SaveInfo (15)
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 15, NULL, NULL);
    -- 2. SyncConsigneeInDB (121)
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 121, NULL, NULL);
    -- 3. Other actions (order based on original script, specific order among these might not matter)
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 40, NULL, NULL); -- SubmitMissingInvoices
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 42, NULL, NULL); -- SubmitUnclassifiedItems
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 47, NULL, NULL); -- SubmitIncompleteSuppliers
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 41, NULL, NULL); -- SubmitIncompleteEntryData
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 44, NULL, NULL); -- SubmitInadequatePackages
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 36, NULL, NULL); -- CreateC71
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 49, NULL, NULL); -- AssessC71
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 38, NULL, NULL); -- DownLoadC71
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 34, NULL, NULL); -- ImportC71
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 39, NULL, NULL); -- DownLoadLicense
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 35, NULL, NULL); -- ImportLicense
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 37, NULL, NULL); -- CreateLicense
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 50, NULL, NULL); -- AssessLicense
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 7, NULL, NULL);  -- RecreatePOEntries
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 51, NULL, NULL); -- AttachToDocSetByRef
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 106, NULL, NULL);-- SubmitEntryCIF
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 8, NULL, NULL);  -- ExportPOEntries
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (109, 10, NULL, NULL); -- AssessPOEntries

    PRINT 'Re-inserted actions for FileTypeId 109 in the desired order.';

    COMMIT TRANSACTION;
    PRINT 'Transaction committed.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    PRINT 'An error occurred. Transaction rolled back.';
    -- Re-throw the error for visibility
    THROW; 
END CATCH
GO

PRINT 'Action reordering script finished.';
GO
