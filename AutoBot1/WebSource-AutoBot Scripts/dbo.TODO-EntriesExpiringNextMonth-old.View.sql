USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-EntriesExpiringNextMonth-old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[TODO-EntriesExpiringNextMonth-old]
AS
SELECT dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id, dbo.AsycudaDocumentBasicInfo.ExpiryDate, 'Expired Entry' AS Type, dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.CNumber, dbo.AsycudaDocumentBasicInfo.RegistrationDate, 
                 dbo.AsycudaDocumentBasicInfo.Reference, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    dbo.AsycudaDocumentBasicInfo INNER JOIN
                 dbo.AsycudaDocumentItem ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.AsycudaDocumentItem.AsycudaDocumentId
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (dbo.AsycudaDocumentBasicInfo.ImportComplete = 1) AND (ISNULL(dbo.AsycudaDocumentBasicInfo.Cancelled, 0) <> 1) AND (ISNULL(dbo.AsycudaDocumentItem.PiQuantity, 0) 
                 <> dbo.AsycudaDocumentItem.ItemQuantity) and (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse') and (dbo.AsycudaDocumentBasicInfo.ExpiryDate <= DATEADD(Month, 2, GETDATE()))
GROUP BY dbo.AsycudaDocumentBasicInfo.ExpiryDate, dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.CNumber, dbo.AsycudaDocumentBasicInfo.RegistrationDate, 
                 dbo.AsycudaDocumentBasicInfo.Reference, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId,  dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id
GO
