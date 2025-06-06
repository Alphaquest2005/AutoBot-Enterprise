USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[RecreateMissingEntryPreviousItems]    Script Date: 3/27/2025 1:48:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
CREATE PROCEDURE [dbo].[RecreateMissingEntryPreviousItems]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO EntryPreviousItems
                         (PreviousItem_Id, Item_Id)
	select PreviousItem_Id,PreviousDocumentItemId from
	(SELECT Previous_item_number, Prev_reg_ser, Prev_reg_nbr, Prev_reg_year, Prev_reg_cuo, Suplementary_Quantity, Preveious_suplementary_quantity, Current_item_number, PreviousItem_Id, ASYCUDA_Id, QtyAllocated, 
                 PreviousDocumentItemId, RndCurrent_Value, ReferenceNumber, CNumber, RegistrationDate, AsycudaDocumentItemId, AssessmentDate, Prev_decl_HS_spec, SalesFactor, DocumentType, DutyFreePaid, ItemNumber, 
                 pLineNumber, ApplicationSettingsId, TotalDutyLiablity, DutyLiablity
		FROM    PreviousItemsEx) as t;

	WITH CTE(previousitem_id, 
		item_id, 
		DuplicateCount)
	AS (SELECT previousitem_id, 
		item_id, 
			   ROW_NUMBER() OVER(PARTITION BY previousitem_id, 
											  item_id
			   ORDER BY entrypreviousitemid) AS DuplicateCount
		FROM EntryPreviousItems)
	DELETE FROM CTE
	WHERE DuplicateCount > 1;


END
GO
