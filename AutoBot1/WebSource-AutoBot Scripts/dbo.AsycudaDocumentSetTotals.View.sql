USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetTotals]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaDocumentSetTotals]
AS
SELECT TOP (100) PERCENT xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, SUM(xcuda_Gs_insurance.Amount_foreign_currency) AS Insurance, SUM(xcuda_Gs_other_cost.Amount_foreign_currency) AS OtherCost, 
                 SUM(xcuda_Gs_internal_freight.Amount_foreign_currency) AS InternalFreight, SUM(xcuda_Gs_deduction.Amount_foreign_currency) AS Deductions
FROM    xcuda_Gs_other_cost INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_Gs_other_cost.Valuation_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 xcuda_Gs_internal_freight ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Gs_internal_freight.Valuation_Id INNER JOIN
                 xcuda_Gs_deduction ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Gs_deduction.Valuation_Id INNER JOIN
                 xcuda_Gs_insurance ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Gs_insurance.Valuation_Id
GROUP BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
GO
