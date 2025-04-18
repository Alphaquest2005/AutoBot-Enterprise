USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOSalesFactor]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[ShipmentInvoicePOSalesFactor]
as

SELECT DISTINCT 
                  PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                 PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, 
                 INVDetails.Cost AS INVCost, PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) AS INVTotalCost,
				 (ISNULL(PODetails.Quantity, 1) / ISNULL(INVDetails.Quantity, 1)) as QtyFactor, (ISNULL(INVDetails.Cost, 1) / ISNULL(PODetails.Cost, 1)) as CostFactor
FROM    ShipmentInvoicePOItemMISINVDetails AS INVDetails FULL OUTER JOIN
                     ShipmentInvoicePOItemMISPODetails AS PODetails ON 
                 INVDetails.EntryData_Id = PODetails.EntryData_Id AND INVDetails.InvoiceId = PODetails.InvoiceId  AND 
                 INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId
WHERE /*(INVDetails.InvoiceNo = '004490' and PODetails.InvoiceNo  = '00973') and */
			(INVDetails.Quantity > 0 and PODetails.Quantity > 0) and
			round((ISNULL(PODetails.Quantity, 1) / ISNULL(INVDetails.Quantity, 1)),2) > 1 and ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * INVDetails.Quantity), 2) = ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) 
					and abs(round((ISNULL(PODetails.Quantity, 1) / ISNULL(INVDetails.Quantity, 1)),2) -  round((ISNULL(INVDetails.Cost, 1) / ISNULL(PODetails.Cost, 1)), 2)) = 0

GO
