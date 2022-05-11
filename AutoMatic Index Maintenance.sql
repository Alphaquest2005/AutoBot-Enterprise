-- Automated index tuning script, will maintain NONCLUSTERED indexes based on missing index DMVs.
-- Set options below and run in the database to be tuned to generate the script (@printOnly = 1)
-- or to actually create and drop the indexes (@printOnly = 0).  
 
-- This is intended to be used for databases with large numbers of ad-hoc queries or databases that
-- cannot be actively maintained by a DBA.  Use this at your own risk, and you should at least start
-- by using this with @printOnly set to 1 and manually running the script it generates.
 
-- The values provided here for the parameters are just provided for example purposes, you should try 
-- different values to see what works best in your database, the ideal values will depend on your workload.
-- Keep in mind that this will not touch CLUSTERED indexes, and there may be further improvement
-- to be found by changing the clustered index on the table.
 
DECLARE @minUserSeeksOrScans INT = 10       -- Minimum user seeks + user scans to qualify for index to be created
DECLARE @minAvgTotalUserCost FLOAT = 2.5    -- Minimum average total user cost to qualify for index to be created
DECLARE @minAvgUserImpact FLOAT = 50      -- Minimum average user impact value to qualify for index to be created
DECLARE @maxIndexesTotal INT = 10           -- Maximum total number of NONCLUSTERED indexes per table
DECLARE @maxIndexCreates INT = 4            -- Maximum number of new indexes per table to create
DECLARE @maxIndexDrops INT = 10              -- Maximum number of existing indexes per table that may be removed if over @maxIndexesTotal
DECLARE @printOnly BIT = 1                  -- Print the index commands only, do not actually create or drop them
 
-- Table variables for holding index information
DECLARE @newIndexes TABLE (schemaName SYSNAME, tableName SYSNAME, benefitRank INT, 
                            createStatement NVARCHAR(MAX), tableObjectId INT)
DECLARE @oldIndexes TABLE (tableObjectId INT, leastUsedRank INT, dropStatement NVARCHAR(MAX))
 
SET NOCOUNT ON
 
PRINT '-------------------------------------------'
PRINT '-- Automatic index processing starting.  --'
 
IF @printOnly = 0
BEGIN
    PRINT '--  LIVE MODE: INDEXES WILL BE MODIFIED! --'
END
 
PRINT '-------------------------------------------'
PRINT ''
PRINT ''
 
 
-- Load the index suggestions and rank them by expected benefit
INSERT INTO @newIndexes (schemaName, tableName, benefitRank, createStatement, tableObjectId)
SELECT S.name AS schemaName, T.name AS tableName, 
    ROW_NUMBER() OVER (PARTITION BY S.name, T.name
                        ORDER BY MIGS.avg_total_user_cost * MIGS.avg_user_impact * (MIGS.user_seeks + MIGS.user_scans) DESC
                    ) AS benefitRank,
    'CREATE INDEX ' + QUOTENAME('IX_' + T.name + '_'
        + ISNULL(REPLACE(REPLACE(REPLACE(MID.equality_columns, '], [', '_'), ']', ''), '[', ''), '') 
        + CASE WHEN equality_columns IS NOT NULL AND inequality_columns IS NOT NULL THEN '_' ELSE '' END
        + ISNULL(REPLACE(REPLACE(REPLACE(MID.inequality_columns, '], [', '_'), ']', ''), '[', ''), ''))
        + ' ON ' + QUOTENAME(S.name) + '.' + QUOTENAME(T.name) 
        + ' (' + ISNULL(equality_columns, '')
        + CASE WHEN equality_columns IS NOT NULL AND inequality_columns IS NOT NULL THEN ', ' ELSE '' END
        + ISNULL(inequality_columns, '') + ')' AS createStatement,
        T.object_id AS tableObjectId
FROM sys.dm_db_missing_index_group_stats MIGS
    INNER JOIN sys.dm_db_missing_index_groups MIG ON MIG.index_group_handle = MIGS.group_handle
    INNER JOIN sys.dm_db_missing_index_details MID ON MID.index_handle = MIG.index_handle
    INNER JOIN sys.objects T ON T.object_id = MID.object_id  
    INNER JOIN sys.schemas S ON S.schema_id = T.schema_id
WHERE MIGS.user_seeks + MIGS.user_scans > @minUserSeeksOrScans
    AND MIGS.avg_total_user_cost > @minAvgTotalUserCost
    AND MIGS.avg_user_impact > @minAvgUserImpact
    AND MID.database_id = DB_ID()
 
-- Load the existing NONCLUSTERED indexes for all tables that had suggestions and rank them by least used
INSERT INTO @oldIndexes (tableObjectId, leastUsedRank, dropStatement)
SELECT T.object_id AS tableObjectId, 
    ROW_NUMBER() OVER (PARTITION BY S.name, T.name
                        ORDER BY ISNULL(user_seeks + user_scans + user_lookups, 0), 
                            ISNULL(last_user_seek, last_user_scan)
                    ) AS leastUsedRank,
    'DROP INDEX ' + QUOTENAME(I.name) + ' ON ' + QUOTENAME(S.name) + '.' + QUOTENAME(T.name) AS dropStatement
FROM sys.indexes I
    INNER JOIN sys.objects T ON T.object_id = I.object_id
    INNER JOIN sys.schemas S ON S.schema_id = T.schema_id
    LEFT OUTER JOIN sys.dm_db_index_usage_stats IUS ON IUS.object_id = I.object_id AND IUS.index_id = I.index_id AND IUS.database_id = DB_ID()
WHERE I.type_desc = 'NONCLUSTERED'
    AND IUS.object_id IN (SELECT tableObjectId FROM @newIndexes)
 
-- Now loop through the tables, create up to @maxIndexCreates indexes, drop up to @maxIndexDrops indexes, but leave @maxIndexesTotal or fewer
DECLARE tableCur CURSOR FOR
    SELECT MAX(schemaName), MAX(tableName), tableObjectId
    FROM @newIndexes
    GROUP BY tableObjectId
    ORDER BY MAX(schemaName), MAX(tableName)
     
OPEN tableCur
 
-- Variables used within the loop
DECLARE @schemaName SYSNAME
DECLARE @tableName SYSNAME
DECLARE @tableObjectId INT
DECLARE @existingIndexCount INT
DECLARE @numIndexesToCreate INT
DECLARE @statementToRun NVARCHAR(MAX)
 
FETCH NEXT FROM tableCur INTO @schemaName, @tableName, @tableObjectId
 
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '-------------------------------------------------------------------------------------------------------'
    PRINT '-- Starting index processing for table: ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName)
 
    -- Get number of existing NONCLUSTERED indexes on this table
    SELECT @existingIndexCount = ISNULL((SELECT MAX(leastUsedRank) FROM @oldIndexes WHERE tableObjectId = @tableObjectId), 0)
     
    PRINT '--      ' + CAST(@existingIndexCount AS VARCHAR(100)) + ' existing NONCLUSTERED indexes found.'
     
    -- Create up to @maxIndexCreates indexes, unless that would leave more than @maxIndexesTotal indexes after others are dropped.
    -- The actual number that will run will be the lesser of @maxIndexCreates and (@maxIndexesTotal - @existingIndexCount + @maxIndexDrops
    SELECT @numIndexesToCreate = CASE WHEN @maxIndexesTotal - @existingIndexCount + @maxIndexDrops < @maxIndexCreates
                                        THEN @maxIndexesTotal - @existingIndexCount + @maxIndexDrops
                                    ELSE @maxIndexCreates
                                END
     
    -- Loop through the CREATE INDEX statements to print/run
    DECLARE createIndexCur CURSOR FOR
        SELECT createStatement
        FROM @newIndexes
        WHERE tableObjectId = @tableObjectId
            AND benefitRank <= @numIndexesToCreate
             
    OPEN createIndexCur
     
    FETCH NEXT FROM createIndexCur INTO @statementToRun
     
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Print the CREATE INDEX statement
        PRINT @statementToRun
         
        IF @printOnly = 0
        BEGIN
            -- Create the index
            EXEC (@statementToRun)
        END
     
        FETCH NEXT FROM createIndexCur INTO @statementToRun
    END
     
    CLOSE createIndexCur
    DEALLOCATE createIndexCur
     
    -- Now drop up to @maxIndexDrops NONCLUSTERED indexes, if over the total.
    -- This is the lesser of @maxIndexDrops and how far above @maxIndexesTotal we now are for this table.
    -- Loop through the DROP INDEX statements to print/run
    DECLARE dropIndexCur CURSOR FOR
        SELECT dropStatement
        FROM @oldIndexes
        WHERE tableObjectId = @tableObjectId
            AND leastUsedRank <= @existingIndexCount + @numIndexesToCreate - @maxIndexesTotal
            AND leastUsedRank <= @maxIndexDrops
             
    OPEN dropIndexCur
     
    FETCH NEXT FROM dropIndexCur INTO @statementToRun
     
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Print the DROP INDEX statement
        PRINT @statementToRun
         
        IF @printOnly = 0
        BEGIN
            -- Drop the index
            EXEC (@statementToRun)
        END
         
        FETCH NEXT FROM dropIndexCur INTO @statementToRun
    END
     
    CLOSE dropIndexCur
    DEALLOCATE dropIndexCur
 
    PRINT '-- Finished processing for table: ' + QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName)
    PRINT '-------------------------------------------------------------------------------------------------------'
    PRINT ''
 
    FETCH NEXT FROM tableCur INTO @schemaName, @tableName, @tableObjectId
END
 
CLOSE tableCur
DEALLOCATE tableCur
 
PRINT ''
PRINT ''
PRINT '-------------------------------------------'
PRINT '-- Automatic index processing completed. --'
PRINT '-------------------------------------------'