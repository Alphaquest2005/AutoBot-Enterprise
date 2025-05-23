USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[UpdateExpiryDates]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateExpiryDates]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

  UPDATE xcuda_ASYCUDA_ExtendedProperties
	SET         EffectiveExpiryDate = exp.Expiration
	FROM    (SELECT        AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code, 
                         MAX(CAST(ExpiredEntriesLst.Expiration AS datetime)) AS Expiration
			FROM            ExpiredEntriesLst INNER JOIN
									 AsycudaDocument ON ExpiredEntriesLst.RegistrationNumber = AsycudaDocument.CNumber AND ExpiredEntriesLst.Office = AsycudaDocument.Customs_clearance_office_code AND 
									 ExpiredEntriesLst.RegistrationDate = AsycudaDocument.RegistrationDate
			GROUP BY AsycudaDocument.ASYCUDA_Id, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.Customs_clearance_office_code--AND ExpiredEntriesLst.DeclarantReference = AsycudaDocument.ReferenceNumber
										) AS exp INNER JOIN
						xcuda_ASYCUDA_ExtendedProperties ON exp.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
	END
GO
