USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[AsycudaItemTaxesByRow]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[AsycudaItemTaxesByRow]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @cols NVARCHAR(MAX), @query NVARCHAR(MAX);
	SET @cols = STUFF(
					 (
						 SELECT DISTINCT
								','+QUOTENAME(c.Duty_tax_code)
						 FROM [dbo].[AsycudaItemTaxes] c FOR XML PATH(''), TYPE
					 ).value('.', 'nvarchar(max)'), 1, 1, '');
	SET @query = 'SELECT Item_Id, '+@cols+'from (SELECT Item_Id,Duty_tax_code,Amount
		FROM [dbo].[AsycudaItemTaxes]
		)x pivot (max(amount) for Duty_tax_code in ('+@cols+')) p';
	--select @query
	EXECUTE (@query);      
END
GO
