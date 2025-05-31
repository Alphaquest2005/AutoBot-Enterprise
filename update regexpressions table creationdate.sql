-- Add missing columns to OCR-RegularExpressions table
ALTER TABLE [OCR-RegularExpressions] 
ADD [CreatedDate] DATETIME NULL,
    [LastUpdated] DATETIME NULL,
    [Description] NVARCHAR(500) NULL;




-- Add default constraints for new records
ALTER TABLE [OCR-RegularExpressions] 
ADD CONSTRAINT DF_RegularExpressions_CreatedDate DEFAULT GETDATE() FOR [CreatedDate];

ALTER TABLE [OCR-RegularExpressions] 
ADD CONSTRAINT DF_RegularExpressions_LastUpdated DEFAULT GETDATE() FOR [LastUpdated];


-- Set default values for existing records
UPDATE [OCR-RegularExpressions] 
SET [CreatedDate] = GETDATE(),
    [LastUpdated] = GETDATE(),
    [Description] = 'Existing pattern'
WHERE [CreatedDate] IS NULL;



-- Make CreatedDate and LastUpdated NOT NULL with defaults
ALTER TABLE [OCR-RegularExpressions] 
ALTER COLUMN [CreatedDate] DATETIME NULL;

ALTER TABLE [OCR-RegularExpressions] 
ALTER COLUMN [LastUpdated] DATETIME NULL;