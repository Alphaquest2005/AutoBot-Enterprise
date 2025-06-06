USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetFreight]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaDocumentSetFreight]
AS
SELECT TOP (100) PERCENT xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, sum(xcuda_Gs_external_freight.Amount_foreign_currency) AS TotalFreight
FROM     xcuda_ASYCUDA_ExtendedProperties LEFT OUTER JOIN
                 xcuda_Gs_external_freight ON xcuda_ASYCUDA_ExtendedProperties.Asycuda_id = xcuda_Gs_external_freight.Valuation_Id
where xcuda_Gs_external_freight.Amount_foreign_currency is not null
GROUP BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
GO
