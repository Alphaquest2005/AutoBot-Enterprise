-- Copy FileTypeMappings from FileType 1152 (Original PO CSV Child) to FileType 1187 (New Shipment Folder CSV Child)
-- Ensure FileType 1187 exists before running this.

PRINT 'Copying FileTypeMappings from ID 1152 to ID 1187...';

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
    1187, -- Target FileTypeId (New Child)
    [OriginalName],
    [DestinationName],
    [DataType],
    [Required],
    [Comments],
    [ReplaceOnlyNulls],
    [ReplicateColumnValues]
FROM [dbo].[FileTypeMappings] AS source
WHERE source.[FileTypeId] = 1152 -- Source FileTypeId (Original Child)
  AND NOT EXISTS ( -- Prevent duplicates if script is run multiple times
    SELECT 1
    FROM [dbo].[FileTypeMappings] AS target
    WHERE target.[FileTypeId] = 1187
      AND target.[OriginalName] = source.[OriginalName]
      AND target.[DestinationName] = source.[DestinationName]
);

PRINT 'Finished copying FileTypeMappings from ID 1152 to ID 1187.';
GO