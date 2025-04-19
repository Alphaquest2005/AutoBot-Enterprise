-- Purpose: Refine RegEx pattern 2329 to prevent overlap with pattern 80 for Shein Invoice Details (Template ID 5)
-- Specifically, prevent pattern 2329 from matching lines immediately preceding a "Sold by:" line,
-- as those are handled by pattern 80.

PRINT 'Updating RegEx ID 2329...';

UPDATE dbo.[OCR-RegularExpressions]
SET RegEx = '(?i)(?<quantity>\d+)\s+of:\s+(?<description>[^$\r\n]+)\s+\$(?<price>\d{1,3}(?:,\d{3})*\.\d{2})(?!\s*\r?\n\s*Sold\s+by:)'
WHERE Id = 2329;

IF @@ROWCOUNT = 0
BEGIN
    PRINT 'WARNING: RegEx ID 2329 not found. No update performed.';
END
ELSE
BEGIN
    PRINT 'RegEx ID 2329 updated successfully.';
END
GO

PRINT 'Verification: Displaying updated RegEx for IDs 80 and 2329:';
SELECT Id, RegEx 
FROM dbo.[OCR-RegularExpressions] 
WHERE Id IN (80, 2329);
GO