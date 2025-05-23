USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[FindBrokenObjects]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FindBrokenObjects]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @stmt nvarchar(max) = ''
DECLARE @vw_schema  NVARCHAR(255)
DECLARE @vw_name varchar(255)

IF OBJECT_ID('tempdb..#badViews') IS NOT NULL DROP TABLE #badViews
IF OBJECT_ID('tempdb..#nulldata') IS NOT NULL DROP TABLE #nulldata

CREATE TABLE #badViews 
(    
    [schema]  NVARCHAR(255),
    name VARCHAR(255),
    error NVARCHAR(MAX) 
)

CREATE TABLE #nullData
(  
    null_data varchar(1)
)


DECLARE tbl_cursor CURSOR LOCAL FORWARD_ONLY READ_ONLY
    FOR SELECT name, SCHEMA_NAME(schema_id) AS [schema]
        FROM sys.objects 
        WHERE type='v'

OPEN tbl_cursor
FETCH NEXT FROM tbl_cursor
INTO @vw_name, @vw_schema



WHILE @@FETCH_STATUS = 0
BEGIN
    SET @stmt = 'SELECT TOP 1 * FROM [' + @vw_schema + N'].[' + @vw_name + ']'
    BEGIN TRY
        INSERT INTO #nullData EXECUTE sp_executesql @stmt
    END TRY 

    BEGIN CATCH
        IF ERROR_NUMBER() != 213 BEGIN
            INSERT INTO #badViews (name, [schema], error) values (@vw_name, @vw_schema, ERROR_MESSAGE())     
        END
    END CATCH


    FETCH NEXT FROM tbl_cursor 
    INTO @vw_name, @vw_schema
END

CLOSE tbl_cursor -- free the memory
DEALLOCATE tbl_cursor

SELECT * FROM #badViews order by name

DROP TABLE #badViews
DROP TABLE #nullData
END
GO
