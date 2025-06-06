USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryDataEx-Old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










CREATE VIEW [dbo].[EntryDataEx-Old]
AS

SELECT TOP (100) PERCENT EntryData.EntryData_Id, EntryData.EntryDataDate AS InvoiceDate, COALESCE ((CASE WHEN entrydata_purchaseorders.ponumber IS NOT NULL THEN 'PO' ELSE NULL END), 
                 (CASE WHEN EntryData_Sales.EntryData_Id IS NOT NULL THEN 'Sales' ELSE NULL END), CASE WHEN EntryData_OpeningStock.EntryData_Id IS NOT NULL THEN 'OPS' ELSE NULL END,
                 CASE WHEN EntryData_Adjustments.EntryData_Id IS NOT NULL THEN entrydata_adjustments.Type ELSE NULL END) AS Type, EntryDataExTotals.Total AS ImportedTotal, EntryData.EntryDataId AS InvoiceNo,  entrydata_purchaseorders.SupplierInvoiceNo, Coalesce(EntryData_Sales.Tax, EntryData_Adjustments.Tax) as Tax,
                 EntryData.InvoiceTotal, EntryData.ImportedLines, EntryDataExTotals.TotalLines, EntryData.Currency, EntryData.ApplicationSettingsId, CASE WHEN isnull(EntryDataExTotals.Tax, Coalesce(EntryData_Sales.Tax, EntryData_Adjustments.Tax)) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, 
                 EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode, ISNULL(EntryDataExTotals.Total, 0) + ISNULL(EntryData.TotalInternalFreight, 0) + ISNULL(EntryData.TotalInsurance, 0) 
                 + ISNULL(EntryData.TotalOtherCost, 0) - ISNULL(EntryData.TotalDeduction, 0) + ISNULL(EntryData.TotalFreight, 0) AS ExpectedTotal, EntryDataExTotals.ClassifiedLines, 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId, ISNULL(EntryData.TotalInternalFreight, 0) AS TotalInternalFreight, ISNULL(EntryData.TotalInsurance, 0) AS TotalInternalInsurance, 
                 ISNULL(EntryData.TotalOtherCost, 0) AS TotalOtherCost, ISNULL(EntryData.TotalDeduction, 0) AS TotalDeductions, ISNULL(EntryData.TotalFreight, 0) AS TotalFreight, ISNULL(EntryDataExTotals.Total, 0) AS Totals, ISNULL(EntryDataExTotals.Packages, 0) AS Packages,
                 EntryData.SourceFile
FROM    EntryData WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 EntryDataExTotals WITH (NOLOCK) ON EntryData.EntryData_Id = EntryDataExTotals.EntryData_Id LEFT OUTER JOIN
                 EntryData_Sales WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id LEFT OUTER JOIN
                 EntryData_OpeningStock WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_OpeningStock.EntryData_Id LEFT OUTER JOIN
                 EntryData_PurchaseOrders WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id
GROUP BY EntryData.EntryData_Id, EntryData.EntryDataDate, EntryDataExTotals.Total, EntryData.EntryDataId, EntryData.InvoiceTotal, EntryData.ImportedLines, EntryDataExTotals.TotalLines, EntryData.Currency, 
                 EntryData.ApplicationSettingsId, EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode, EntryDataExTotals.ClassifiedLines, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                  EntryData.SourceFile, entrydata_purchaseorders.ponumber, EntryData_Sales.EntryData_Id, EntryData_OpeningStock.EntryData_Id ,
				 EntryData_Adjustments.EntryData_Id, entrydata_adjustments.Type, EntryDataExTotals.Tax, EntryData_Sales.Tax, EntryData_Adjustments.Tax, EntryDataExTotals.Packages, entrydata_purchaseorders.SupplierInvoiceNo,
				 EntryData.TotalInternalFreight,EntryData.TotalInsurance, EntryData.TotalOtherCost, EntryData.TotalDeduction, EntryData.TotalFreight
ORDER BY entrydata_id desc

--SELECT TOP (100) PERCENT dbo.EntryData.EntryData_Id, dbo.EntryData.EntryDataDate AS InvoiceDate, COALESCE ((CASE WHEN entrydata_purchaseorders.ponumber IS NOT NULL THEN 'PO' ELSE NULL END), 
--                 (CASE WHEN EntryData_Sales.EntryData_Id IS NOT NULL THEN 'Sales' ELSE NULL END), CASE WHEN EntryData_OpeningStock.EntryData_Id IS NOT NULL THEN 'OPS' ELSE NULL END, 
--                 CASE WHEN EntryData_Adjustments.EntryData_Id IS NOT NULL THEN entrydata_adjustments.Type ELSE NULL END) AS Type, dbo.EntryDataExTotals.Total AS ImportedTotal, dbo.EntryData.EntryDataId AS InvoiceNo, 
--                 dbo.EntryData.InvoiceTotal, dbo.EntryData.ImportedLines, dbo.EntryDataExTotals.TotalLines, dbo.EntryData.Currency, dbo.EntryData.ApplicationSettingsId, 
--                 CASE WHEN Tax = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, dbo.EntryData.EmailId, dbo.EntryData.FileTypeId, dbo.EntryData.SupplierCode, ISNULL(dbo.EntryDataExTotals.Total, 0) 
--                 + ISNULL(dbo.EntryData.TotalInternalFreight, 0) + ISNULL(dbo.EntryData.TotalInsurance, 0) + ISNULL(dbo.EntryData.TotalOtherCost, 0) - ISNULL(dbo.EntryData.TotalDeduction, 0) + ISNULL(dbo.EntryData.TotalFreight, 0) 
--                 AS ExpectedTotal, dbo.EntryDataExTotals.ClassifiedLines, dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, ISNULL(dbo.EntryData.TotalInternalFreight, 0) AS TotalInternalFreight, 
--                 ISNULL(dbo.EntryData.TotalInsurance, 0) AS TotalInternalInsurance, ISNULL(dbo.EntryData.TotalOtherCost, 0) AS TotalOtherCost, ISNULL(dbo.EntryData.TotalDeduction, 0) AS TotalDeductions, 
--                 ISNULL(dbo.EntryData.TotalFreight, 0) AS TotalFreight, ISNULL(dbo.EntryDataExTotals.Total, 0) AS Totals, SourceFile
--FROM    dbo.EntryData WITH (NOLOCK) INNER JOIN
--                 dbo.AsycudaDocumentSetEntryData ON dbo.EntryData.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
--                 dbo.EntryData_Adjustments ON dbo.EntryData.EntryData_Id = dbo.EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
--                 dbo.EntryDataExTotals WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryDataExTotals.EntryData_Id LEFT OUTER JOIN
--                 dbo.EntryData_Sales WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryData_Sales.EntryData_Id LEFT OUTER JOIN
--                 dbo.EntryData_OpeningStock WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryData_OpeningStock.EntryData_Id LEFT OUTER JOIN
--                 dbo.EntryData_PurchaseOrders WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryData_PurchaseOrders.EntryData_Id
--ORDER BY InvoiceNo
GO
