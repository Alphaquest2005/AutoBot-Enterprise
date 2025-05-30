USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitEntryCIF-EntryDataEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[TODO-SubmitEntryCIF-EntryDataEx]
AS
SELECT EntryData.EntryDataDate AS InvoiceDate, COALESCE ((CASE WHEN entrydata_purchaseorders.ponumber IS NOT NULL THEN 'PO' ELSE NULL END), (CASE WHEN EntryData_Sales.EntryData_Id IS NOT NULL 
                 THEN 'Sales' ELSE NULL END), CASE WHEN EntryData_OpeningStock.EntryData_Id IS NOT NULL THEN 'OPS' ELSE NULL END, CASE WHEN EntryData_Adjustments.EntryData_Id IS NOT NULL 
                 THEN entrydata_adjustments.Type ELSE NULL END) AS Type, EntryData.EntryDataId AS InvoiceNo, EntryData_PurchaseOrders.SupplierInvoiceNo, EntryData.InvoiceTotal, EntryData.ImportedLines, 
                 EntryData.ApplicationSettingsId, EntryData.EntryData_Id, AsycudaDocumentSetEntryData.AsycudaDocumentSetId
FROM    EntryData WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 EntryData_Adjustments WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 EntryData_Sales WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id LEFT OUTER JOIN
                 EntryData_OpeningStock WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_OpeningStock.EntryData_Id LEFT OUTER JOIN
                 EntryData_PurchaseOrders WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id
GROUP BY EntryData.EntryDataDate, EntryData.EntryDataId, EntryData.InvoiceTotal, EntryData.ImportedLines, EntryData.ApplicationSettingsId, EntryData_PurchaseOrders.SupplierInvoiceNo, EntryData.EntryData_Id, 
                 EntryData_PurchaseOrders.PONumber, EntryData_Sales.EntryData_Id, EntryData_OpeningStock.EntryData_Id, EntryData_Adjustments.EntryData_Id, EntryData_Adjustments.Type, 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId
GO
