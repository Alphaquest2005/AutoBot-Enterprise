USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemMISMatches]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE view [dbo].[ShipmentInvoicePOItemMISMatches]
as

SELECT distinct  ISNULL(CAST(row_number() over (order by isnull(PODetailsId, 0), isnull(INVDetailsId,0)) AS int), 0)  as Id, PODetailsId, INVDetailsId, POId, INVId, PONumber, InvoiceNo, POItemCode, INVItemCode, PODescription, INVDescription, POCost, INVCost, POQuantity, INVQuantity, INVSalesFactor, POTotalCost, INVTotalCost
FROM    ShipmentInvoicePOItemMISData
GROUP BY id, PODetailsId, INVDetailsId, POId, INVId, PONumber, InvoiceNo, POItemCode, INVItemCode, PODescription, INVDescription, POCost, INVCost, POQuantity, INVQuantity, INVSalesFactor, POTotalCost, INVTotalCost

GO
