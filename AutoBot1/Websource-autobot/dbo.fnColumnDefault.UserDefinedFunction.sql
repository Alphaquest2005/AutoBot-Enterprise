USE [WebSource-AutoBot]
GO
/****** Object:  UserDefinedFunction [dbo].[fnColumnDefault]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnColumnDefault](@sTableName varchar(128), @sColumnName varchar(128))
RETURNS varchar(4000)
AS
BEGIN
	DECLARE @sDefaultValue varchar(4000)

	SELECT	@sDefaultValue = dbo.fnCleanDefaultValue(COLUMN_DEFAULT)
	FROM	INFORMATION_SCHEMA.COLUMNS
	WHERE	TABLE_NAME = @sTableName
	 AND 	COLUMN_NAME = @sColumnName

	RETURN 	@sDefaultValue

END
GO
