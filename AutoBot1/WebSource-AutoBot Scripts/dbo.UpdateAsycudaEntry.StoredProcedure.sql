USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[UpdateAsycudaEntry]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Alter date: <Alter Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateAsycudaEntry]
	-- Add the parameters for the stored procedure here
	@Item_Id int,
	@DFQtyAllocated Float,
	@DPQtyAllocated Float
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update xcuda_Item
	Set DFQtyAllocated = @DFQtyAllocated, DPQtyAllocated = @DPQtyAllocated
	where Item_Id = @Item_Id

END
GO
