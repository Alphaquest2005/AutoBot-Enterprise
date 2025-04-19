USE [BudgetMarine-AutoBot]
GO

/****** Object:  View [dbo].[Kim-]    Script Date: 12/13/2022 2:38:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[Kim-]
AS
SELECT        dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.PiQuantity, 
                         dbo.AsycudaDocumentItem.Total_CIF_itm / dbo.AsycudaDocumentItem.ItemQuantity AS Cost, dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity AS [Remaining Qty], 
                         (dbo.AsycudaDocumentItem.Total_CIF_itm / dbo.AsycudaDocumentItem.ItemQuantity) * (dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity) AS Value, dbo.AsycudaDocumentItem.ReferenceNumber, 
                         dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.LineNumber
FROM            dbo.CustomsOperations INNER JOIN
                         dbo.Customs_Procedure ON dbo.CustomsOperations.Id = dbo.Customs_Procedure.CustomsOperationId INNER JOIN
                         dbo.AsycudaDocumentItem ON dbo.Customs_Procedure.CustomsProcedure = dbo.AsycudaDocumentItem.CustomsProcedure
WHERE        (dbo.AsycudaDocumentItem.IsAssessed = 1) AND (dbo.CustomsOperations.Name = N'Warehouse')
GROUP BY dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.PiQuantity, 
                         dbo.AsycudaDocumentItem.Total_CIF_itm / dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity, 
                         (dbo.AsycudaDocumentItem.Total_CIF_itm / dbo.AsycudaDocumentItem.ItemQuantity) * (dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity), dbo.AsycudaDocumentItem.ReferenceNumber, 
                         dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.LineNumber
HAVING        (dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity > 0)
GO



ALTER VIEW [dbo].[Kim-ItemValues]
AS
SELECT        dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.PiQuantity, 
                         dbo.AsycudaDocumentItem.Total_CIF_itm / dbo.AsycudaDocumentItem.ItemQuantity AS Cost, dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity AS [Remaining Qty], 
                         (dbo.AsycudaDocumentItem.Total_CIF_itm / dbo.AsycudaDocumentItem.ItemQuantity) * (dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity) AS Value
FROM            dbo.CustomsOperations INNER JOIN
                         dbo.Customs_Procedure ON dbo.CustomsOperations.Id = dbo.Customs_Procedure.CustomsOperationId INNER JOIN
                         dbo.AsycudaDocumentItem ON dbo.Customs_Procedure.CustomsProcedure = dbo.AsycudaDocumentItem.CustomsProcedure
WHERE        (dbo.AsycudaDocumentItem.IsAssessed = 1) AND (dbo.CustomsOperations.Name = N'Warehouse') AND (dbo.AsycudaDocumentItem.ItemQuantity - dbo.AsycudaDocumentItem.PiQuantity > 0)
GO


