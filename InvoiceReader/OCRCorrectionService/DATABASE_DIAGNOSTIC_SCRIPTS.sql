-- ====================================================================
-- DATABASE DIAGNOSTIC SCRIPTS
-- ====================================================================
-- Purpose: Identify correct database/schema names and table structures
-- ====================================================================

-- **STEP 1: FIND DATABASES WITH FILETYPES TABLES**
SELECT 
    'Database Discovery' as QueryPurpose,
    'FileTypes table locations' as Description;

-- Check current database for FileTypes table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FileTypes')
BEGIN
    SELECT 'Current Database' as Location, DB_NAME() as DatabaseName, 'FileTypes found' as Status;
END
ELSE
BEGIN
    SELECT 'Current Database' as Location, DB_NAME() as DatabaseName, 'FileTypes NOT found' as Status;
END

-- **STEP 2: EXAMINE ACTUAL OCR_TEMPLATETABLEMAPPING STRUCTURE**
SELECT 
    'OCR_TemplateTableMapping Structure Analysis' as QueryPurpose;

-- Check if our table was created and examine its structure
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'OCR_TemplateTableMapping')
BEGIN
    SELECT 
        c.COLUMN_NAME,
        c.DATA_TYPE,
        c.IS_NULLABLE,
        c.CHARACTER_MAXIMUM_LENGTH,
        c.COLUMN_DEFAULT
    FROM INFORMATION_SCHEMA.COLUMNS c
    WHERE c.TABLE_NAME = 'OCR_TemplateTableMapping'
    ORDER BY c.ORDINAL_POSITION;
END
ELSE
BEGIN
    SELECT 'OCR_TemplateTableMapping table does not exist' as Status;
END

-- **STEP 3: FIND FILETYPES TABLE IN VARIOUS DATABASES**
-- Try common database names
DECLARE @sql NVARCHAR(MAX);
DECLARE @databases TABLE (DatabaseName NVARCHAR(128));

-- Insert common database names to check
INSERT INTO @databases VALUES 
    ('CoreEntities'),
    ('WaterNut'),
    ('AutoBot'),
    ('WebSource-AutoBot-Original'),
    (DB_NAME()); -- Current database

-- Check each database for FileTypes table
DECLARE @dbName NVARCHAR(128);
DECLARE db_cursor CURSOR FOR SELECT DatabaseName FROM @databases;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @dbName;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        SET @sql = 'IF EXISTS (SELECT name FROM [' + @dbName + '].sys.tables WHERE name = ''FileTypes'')
                    SELECT ''' + @dbName + ''' as DatabaseName, ''FileTypes found'' as TableStatus, COUNT(*) as RowCount
                    FROM [' + @dbName + '].dbo.FileTypes';
        EXEC sp_executesql @sql;
    END TRY
    BEGIN CATCH
        SELECT @dbName as DatabaseName, 'Database not accessible or does not exist' as TableStatus, 0 as RowCount;
    END CATCH
    
    FETCH NEXT FROM db_cursor INTO @dbName;
END

CLOSE db_cursor;
DEALLOCATE db_cursor;

-- **STEP 4: EXAMINE EXISTING OCR-INVOICES TABLE STRUCTURE**
SELECT 
    'OCR-Invoices Table Analysis' as QueryPurpose;

-- Check WebSource-AutoBot-Original database for OCR-Invoices
BEGIN TRY
    IF EXISTS (SELECT name FROM [WebSource-AutoBot-Original].sys.tables WHERE name = 'OCR-Invoices')
    BEGIN
        SELECT 
            'OCR-Invoices Column Structure' as Analysis;
        
        SELECT 
            c.COLUMN_NAME,
            c.DATA_TYPE,
            c.IS_NULLABLE,
            c.CHARACTER_MAXIMUM_LENGTH
        FROM [WebSource-AutoBot-Original].INFORMATION_SCHEMA.COLUMNS c
        WHERE c.TABLE_NAME = 'OCR-Invoices'
        ORDER BY c.ORDINAL_POSITION;
        
        -- Sample data to understand FileTypeId values
        SELECT TOP 10
            'Sample OCR-Invoices Data' as Analysis,
            [Id], [Name], [FileTypeId], [ApplicationSettingsId], [IsActive]
        FROM [WebSource-AutoBot-Original].[dbo].[OCR-Invoices]
        WHERE [IsActive] = 1;
    END
    ELSE
    BEGIN
        SELECT 'OCR-Invoices table not found in WebSource-AutoBot-Original' as Status;
    END
END TRY
BEGIN CATCH
    SELECT 'Cannot access WebSource-AutoBot-Original database' as Status;
END CATCH

-- **STEP 5: LIST ALL TABLES WITH 'FILE' IN THE NAME**
SELECT 
    'Tables with FILE in name' as QueryPurpose;

SELECT 
    TABLE_CATALOG,
    TABLE_SCHEMA,
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE '%FILE%'
ORDER BY TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME;

-- **STEP 6: FIND TABLES WITH FILETYPEID COLUMN**
SELECT 
    'Tables with FileTypeId column' as QueryPurpose;

SELECT DISTINCT
    c.TABLE_CATALOG,
    c.TABLE_SCHEMA,
    c.TABLE_NAME,
    c.COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.COLUMN_NAME = 'FileTypeId'
ORDER BY c.TABLE_CATALOG, c.TABLE_SCHEMA, c.TABLE_NAME;

-- ====================================================================
-- INSTRUCTIONS:
-- 1. Run this diagnostic script first
-- 2. Review the results to identify:
--    - Correct database name for FileTypes table
--    - Actual column names in existing tables
--    - Proper schema references
-- 3. Use results to create corrected integration script
-- ====================================================================