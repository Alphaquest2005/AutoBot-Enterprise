USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-StockDifferences-Asycuda]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[Reports-POSvsAsycuda-StockDifferences-Asycuda]
AS

SELECT        isnull(#OPS.InvoiceNo, 'Asycuda') as InvoiceNo, ISNULL(#OPS.ItemNumber, #AsycudaData.ItemNumber) AS ItemNumber,  ISNULL(#OPS.InventoryItemId, #AsycudaData.InventoryItemId) AS InventoryItemId, ISNULL(#OPS.ItemDescription, 
                         #AsycudaData.Description) AS Description, ISNULL(#OPS.Quantity, #AsycudaData.Quantity) AS Quantity,
						 #OPS.[OPS2zero-Adjustment],#OPS.Returns AS Returns, #OPS.Sales AS Sales,#OPS.QuantityOnHand as [OPS-QuantityOnHand], #OPS.Quantity AS [OPS-Quantity], 
                         ISNULL(#OPS.Cost, 0) AS OPSCost, #AsycudaData.Quantity AS [Asycuda-Quantity], #AsycudaData.PiQuantity AS [Asycuda-PiQuantity], ISNULL(#AsycudaData.Cost, 0) AS AsycudaCost, 
                         COALESCE (#OPS.Quantity, 0) - COALESCE (#AsycudaData.Quantity, 0) AS Diff, ISNULL(#OPS.Cost, #AsycudaData.Cost) AS Cost, 
                         (COALESCE (#OPS.Quantity, 0) - COALESCE (#AsycudaData.Quantity, 0)) * ISNULL(#OPS.Cost, #AsycudaData.Cost) AS TotalCost
--into  [#StockDifferences-Asycuda]
FROM            dbo.[Reports-POSvsAsycuda-AsycudaSummary-Asycuda] as #AsycudaData FULL OUTER JOIN
                dbo.[Reports-POSvsAsycuda-OPS] as #OPS ON #AsycudaData.InventoryItemId = #OPS.InventoryItemId
GO
