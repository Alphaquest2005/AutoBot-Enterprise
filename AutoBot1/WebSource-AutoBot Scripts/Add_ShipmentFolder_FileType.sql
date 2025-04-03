-- Add new FileType for processing shipment folders based on Info.txt
SET IDENTITY_INSERT [dbo].[FileTypes] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1186)
BEGIN
    INSERT INTO [dbo].[FileTypes] (
        [Id], [ApplicationSettingsId], [Description], [FilePattern],
        [CreateDocumentSet], [DocumentSpecific], [DocumentCode], [ReplyToMail],
        [FileGroupId], [MergeEmails], [CopyEntryData], [ParentFileTypeId],
        [OverwriteFiles], [HasFiles], [OldFileTypeId], [ReplicateHeaderRow],
        [IsImportable], [MaxFileSizeInMB], [FileInfoId], [DocSetRefernece]
    ) VALUES (
        1186, 3, N'Shipment Folder (Info.txt + Files)', N'Info.txt', -- FilePattern identifies the type
        1, 0, N'NA', 0, -- CreateDocumentSet=true, DocumentSpecific=false
        NULL, 0, 0, NULL, -- FileGroupId=NULL, MergeEmails=false, CopyEntryData=false, Parent=NULL
        1, NULL, NULL, NULL, -- OverwriteFiles=true
        1, NULL, 39, N'ShipmentInput' -- IsImportable=true, FileInfoId=39 (Info, TXT), DocSetReference=ShipmentInput (folder name)
    );
    PRINT 'FileType ShipmentFolder added with ID 1186.';
END
ELSE
BEGIN
    PRINT 'FileType with ID 1186 already exists.';
END

SET IDENTITY_INSERT [dbo].[FileTypes] OFF;
GO