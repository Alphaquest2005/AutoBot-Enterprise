USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryDataEx-Basic]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO












CREATE VIEW [dbo].[EntryDataEx-Basic]
AS

SELECT EntryData.EntryData_Id, EntryData.EntryDataDate AS InvoiceDate, COALESCE ((CASE WHEN entrydata_purchaseorders.ponumber IS NOT NULL THEN 'PO' ELSE NULL END), 
                 (CASE WHEN EntryData_Sales.EntryData_Id IS NOT NULL THEN 'Sales' ELSE NULL END), CASE WHEN EntryData_OpeningStock.EntryData_Id IS NOT NULL THEN 'OPS' ELSE NULL END,
                 CASE WHEN EntryData_Adjustments.EntryData_Id IS NOT NULL THEN entrydata_adjustments.Type ELSE NULL END) AS Type,  EntryData.EntryDataId AS InvoiceNo,  entrydata_purchaseorders.SupplierInvoiceNo, Coalesce(EntryData_Sales.Tax, EntryData_Adjustments.Tax) as Tax,
                 EntryData.InvoiceTotal, EntryData.ImportedLines, EntryData.Currency, EntryData.ApplicationSettingsId, CASE WHEN Coalesce(EntryData_Sales.Tax, EntryData_Adjustments.Tax) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, 
                 EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode,
                 ISNULL(EntryData.TotalInternalFreight, 0) AS TotalInternalFreight, ISNULL(EntryData.TotalInsurance, 0) AS TotalInternalInsurance, 
                 ISNULL(EntryData.TotalOtherCost, 0) AS TotalOtherCost, ISNULL(EntryData.TotalDeduction, 0) AS TotalDeductions, ISNULL(EntryData.TotalFreight, 0) AS TotalFreight, 
                 EntryData.SourceFile
FROM    EntryData WITH (NOLOCK) LEFT OUTER JOIN
                 EntryData_Adjustments WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id  LEFT OUTER JOIN
                 EntryData_Sales WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id LEFT OUTER JOIN
                 EntryData_OpeningStock WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_OpeningStock.EntryData_Id LEFT OUTER JOIN
                 EntryData_PurchaseOrders WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id
--GROUP BY EntryData.EntryData_Id, EntryData.EntryDataDate, EntryData.EntryDataId, EntryData.InvoiceTotal, EntryData.ImportedLines,  EntryData.Currency, 
--                 EntryData.ApplicationSettingsId, EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode,  
--                  EntryData.SourceFile, entrydata_purchaseorders.ponumber, EntryData_Sales.EntryData_Id, EntryData_OpeningStock.EntryData_Id ,
--				 EntryData_Adjustments.EntryData_Id, entrydata_adjustments.Type,  EntryData_Sales.Tax, EntryData_Adjustments.Tax,  entrydata_purchaseorders.SupplierInvoiceNo,
--				 EntryData.TotalInternalFreight,EntryData.TotalInsurance, EntryData.TotalOtherCost, EntryData.TotalDeduction, EntryData.TotalFreight



GO
