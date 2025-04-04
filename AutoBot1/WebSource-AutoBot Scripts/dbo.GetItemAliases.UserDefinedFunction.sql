USE [WebSource-AutoBot]
GO
/****** Object:  UserDefinedFunction [dbo].[GetItemAliases]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetItemAliases]
(
	-- Add the parameters for the function here
	@Id varchar(50)
)
RETURNS varchar(50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @res varchar(50)

	-- Add the r-SQL statements to compute the return value here
	SELECT  @res = COALESCE(@res + ' ', '') + AliasName
		FROM   InventoryItemAliasEX where ItemNumber = @id

	-- Return the result of the function
	RETURN @res

END
GO
