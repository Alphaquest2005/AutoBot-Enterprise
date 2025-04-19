-- Copy FileTypeMappings from FileType 1151 (assuming it has the correct PO mappings) to FileType 1186 (Shipment Folder)
-- Ensure FileType 1186 exists before running this.

PRINT 'Copying FileTypeMappings from ID 1151 to ID 1186...';

INSERT INTO [dbo].[FileTypeMappings] (
    [FileTypeId],
    [OriginalName],
    [DestinationName],
    [DataType],
    [Required],
    [Comments],
    [ReplaceOnlyNulls],
    [ReplicateColumnValues]
)
SELECT
    1186, -- Target FileTypeId
    [OriginalName],
    [DestinationName],
    [DataType],
    [Required],
    [Comments],
    [ReplaceOnlyNulls],
    [ReplicateColumnValues]
FROM [dbo].[FileTypeMappings] AS source
WHERE source.[FileTypeId] = 1151
  AND NOT EXISTS ( -- Prevent duplicates if script is run multiple times
    SELECT 1
    FROM [dbo].[FileTypeMappings] AS target
    WHERE target.[FileTypeId] = 1186
      AND target.[OriginalName] = source.[OriginalName]
      AND target.[DestinationName] = source.[DestinationName]
);

PRINT 'Finished copying FileTypeMappings from ID 1151 to ID 1186.';
GO