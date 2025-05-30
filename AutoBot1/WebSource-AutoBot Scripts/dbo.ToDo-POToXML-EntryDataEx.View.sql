USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ToDo-POToXML-EntryDataEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[ToDo-POToXML-EntryDataEx]
AS

SELECT EntryData.EntryData_Id, EntryData.EntryDataId AS InvoiceNo, EntryData_PurchaseOrders.SupplierInvoiceNo, EntryData.InvoiceTotal, ISNULL(Totals.Total, 0) + ISNULL(EntryData.TotalInternalFreight, 0) + ISNULL(EntryData.TotalInsurance, 0) 
                 + ISNULL(EntryData.TotalOtherCost, 0) - ISNULL(EntryData.TotalDeduction, 0) + ISNULL(EntryData.TotalFreight, 0) AS ExpectedTotal , EntryData.ApplicationSettingsId
FROM    EntryData WITH (NOLOCK) INNER JOIN
                 EntryData_PurchaseOrders WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id
				 inner join [ToDo-POToXML-EntryDataExTotals] as Totals on Totals.EntryData_Id = EntryData.EntryData_Id

GROUP BY EntryData.EntryData_Id, EntryData.EntryDataId, EntryData.InvoiceTotal, EntryData.ApplicationSettingsId, EntryData_PurchaseOrders.SupplierInvoiceNo, Totals.Total,  
	EntryData.TotalInternalFreight, TotalInsurance, TotalOtherCost, TotalDeduction, TotalFreight


GO
