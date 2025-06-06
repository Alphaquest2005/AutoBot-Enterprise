USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[UpdateCancelledEntries]    Script Date: 3/27/2025 1:48:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
Create PROCEDURE [dbo].[UpdateCancelledEntries]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  UPDATE xcuda_ASYCUDA_ExtendedProperties
	SET         Cancelled = 1
	FROM    (SELECT AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code
						FROM     CancelledEntriesLst INNER JOIN
										AsycudaDocument ON
										CancelledEntriesLst.RegistrationNumber = AsycudaDocument.CNumber and CancelledEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND CancelledEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate--AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber
										) AS exp INNER JOIN
						xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
	END
GO
