USE [AutoBot-EnterpriseDB]
GO

/****** Object:  View [dbo].[AsycudaDocumentSetFreightCIFVerification]    Script Date: 11/25/2021 3:27:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[AsycudaDocumentSetFreightCIFVerification]
AS
SELECT TOP (100) PERCENT xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, SUM(xcuda_Gs_external_freight.Amount_foreign_currency) AS TotalFreight, AsycudaDocumentTotalCIF.TotalCIF, 
                 AsycudaDocumentTotalCIF.ReferenceNumber, AsycudaDocumentTotalCIF.ImportComplete, AsycudaDocumentTotalCIF.RegistrationDate, AsycudaDocumentTotalCIF.CNumber, 
                 AsycudaDocumentTotalCIF.DocumentType
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 AsycudaDocumentTotalCIF ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentTotalCIF.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Gs_external_freight ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Gs_external_freight.Valuation_Id
WHERE (xcuda_Gs_external_freight.Amount_foreign_currency IS NOT NULL) and isnull(xcuda_ASYCUDA_ExtendedProperties.Cancelled,0 ) = 0
GROUP BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, AsycudaDocumentTotalCIF.TotalCIF, AsycudaDocumentTotalCIF.ReferenceNumber, AsycudaDocumentTotalCIF.RegistrationDate, 
                 AsycudaDocumentTotalCIF.CNumber, AsycudaDocumentTotalCIF.DocumentType, AsycudaDocumentTotalCIF.ImportComplete
GO


select * from [AsycudaDocumentSetFreightCIFVerification] where CNumber = '8910'

select * from [AsycudaDocumentSetFreightCIFVerification] where ReferenceNumber like 'SUCR444759-BM%' order by CNumber