USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[BondReport-CNumber]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[BondReport-CNumber]
AS
SELECT dbo.AsycudaItemBasicInfo.CNumber, AsycudaItemBasicInfo.RegistrationDate, SUM(dbo.AsycudaItemBasicInfo.ItemQuantity) AS ItemQuantity, SUM(dbo.[AscyudaItemPiQuantity-Basic].PiQuantity) 
                 AS PiQuantity, SUM(dbo.AsycudaItemBasicInfo.ItemQuantity - ISNULL(dbo.[AscyudaItemPiQuantity-Basic].PiQuantity, 0)) AS Balance, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    dbo.AsycudaItemBasicInfo INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.AsycudaItemBasicInfo.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.[AscyudaItemPiQuantity-Basic] ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.[AscyudaItemPiQuantity-Basic].Item_Id
WHERE (dbo.AsycudaItemBasicInfo.CNumber IS NOT NULL) AND (dbo.AsycudaDocumentBasicInfo.DocumentType = N'IM7') AND (ISNULL(dbo.AsycudaDocumentBasicInfo.Cancelled, 0) <> 1)
GROUP BY AsycudaItemBasicInfo.CNumber, AsycudaItemBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
GO
