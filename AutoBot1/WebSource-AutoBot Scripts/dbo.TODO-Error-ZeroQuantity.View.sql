USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-ZeroQuantity]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create VIEW [dbo].[TODO-Error-ZeroQuantity]
AS
SELECT        AsycudaItemBasicInfo.Item_Id, 'Zero Quantity' AS Type, AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.Reference, 
                         AsycudaDocumentBasicInfo.ImportComplete
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         AsycudaItemBasicInfo ON AsycudaDocumentBasicInfo.ASYCUDA_Id = AsycudaItemBasicInfo.ASYCUDA_Id
WHERE        (AsycudaItemBasicInfo.ItemQuantity = 0) AND (AsycudaDocumentBasicInfo.ImportComplete = 1)
GO
