USE [WebSource-AutoBot]
GO

-- Check if ActionId 121 already exists to prevent errors on re-run
IF NOT EXISTS (SELECT 1 FROM dbo.Actions WHERE Id = 121)
BEGIN
    -- Add the new Action 'SyncConsigneeInDB'
    SET IDENTITY_INSERT [dbo].[Actions] ON;
    
    INSERT INTO [dbo].[Actions] ([Id], [Name], [TestMode], [IsDataSpecific]) 
    VALUES (121, N'SyncConsigneeInDB', 0, 1); -- Assuming IsDataSpecific = 1
    
    SET IDENTITY_INSERT [dbo].[Actions] OFF;
    PRINT 'Action SyncConsigneeInDB added with Id 121.';
END
ELSE
BEGIN
    PRINT 'Action SyncConsigneeInDB (Id 121) already exists.';
END
GO

-- Check if the FileTypeAction link already exists to prevent duplicates
IF NOT EXISTS (SELECT 1 FROM dbo.FileTypeActions WHERE FileTypeId = 109 AND ActionId = 121)
BEGIN
    -- Add the FileTypeAction link for FileTypeId 109 to run SyncConsigneeInDB
    -- This assumes the application executes actions based on the order they are retrieved,
    -- likely based on the FileTypeActions.Id (IDENTITY column).
    -- Inserting it now should place it after existing actions for FileTypeId 109.
    INSERT INTO [dbo].[FileTypeActions] ([FileTypeId], [ActionId], [AssessIM7], [AssessEX])
    VALUES (109, 121, NULL, NULL);
    
    PRINT 'FileTypeAction added for FileTypeId 109 and ActionId 121.';
END
ELSE
BEGIN
    PRINT 'FileTypeAction for FileTypeId 109 and ActionId 121 already exists.';
END
GO

PRINT 'Script execution finished.';
