-- Add new Child FileType for the CSV generated within a Shipment Folder context
SET IDENTITY_INSERT [dbo].[FileTypes] ON;

-- Use ID 1187 (ensure this ID is available)
IF NOT EXISTS (SELECT 1 FROM [dbo].[FileTypes] WHERE Id = 1187)
BEGIN
    INSERT INTO [dbo].[FileTypes] (
        [Id], [ApplicationSettingsId], [Description], [FilePattern],
        [CreateDocumentSet], [DocumentSpecific], [DocumentCode], [ReplyToMail],
        [FileGroupId], [MergeEmails], [CopyEntryData], [ParentFileTypeId],
        [OverwriteFiles], [HasFiles], [OldFileTypeId], [ReplicateHeaderRow],
        [IsImportable], [MaxFileSizeInMB], [FileInfoId], [DocSetRefernece]
    ) VALUES (
        1187, 3, N'Shipment Folder PO CSV', N'.*-Fixed.csv', -- Description & Pattern similar to 1152
        1, 1, N'NA', 0, -- CreateDocumentSet=true, DocumentSpecific=true (like 1152)
        NULL, 0, 1, 1186, -- FileGroupId=NULL, MergeEmails=false, CopyEntryData=true, Parent=1186 (Shipment Folder)
        1, NULL, NULL, NULL, -- OverwriteFiles=true
        NULL, NULL, 6, N'ShipmentInput' -- IsImportable=NULL (processed via parent), FileInfoId=6 (PO, CSV), DocSetReference=ShipmentInput
    );
    PRINT 'Child FileType ShipmentFolder PO CSV added with ID 1187, Parent ID 1186.';
END
ELSE
BEGIN
    PRINT 'FileType with ID 1187 already exists.';
END

SET IDENTITY_INSERT [dbo].[FileTypes] OFF;
GO