USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[UpdateEntryDataDetailsEx]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Alter date: <Alter Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateEntryDataDetailsEx]
	-- Add the parameters for the stored procedure here
		   @EntryDataDetailsId int,
           @EntryDataId varchar(50),
           @LineNumber int,
           @ItemNumber varchar(50),
           @Quantity decimal(15,4),
           @Units varchar(15),
           @ItemDescription varchar(max),
           @Cost decimal(15,4),
           @QtyAllocated float,
           @UnitWeight decimal(15,4),
           @DoNotAllocate bit,
           @TariffCode varchar(8),
           @CNumber varchar(max),
           @CLineNumber int,
           @Downloaded bit,
           @DutyFreePaid varchar(8),
           @EntryDataDate datetime,
           @AsycudaDocumentSetId int,
           @AsycudaDocumentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update EntryDataDetails
	Set ItemNumber = @ItemNumber,
		ItemDescription = @ItemDescription,
		Cost = @Cost,
		Quantity = @Quantity,
		DoNotAllocate = @DoNotAllocate,
		Units = @Units
	Where EntryDataDetailsId = @EntryDataDetailsId

END
GO
