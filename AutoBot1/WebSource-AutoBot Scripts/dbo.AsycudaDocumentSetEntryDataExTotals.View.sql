USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetEntryDataExTotals]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[AsycudaDocumentSetEntryDataExTotals]
AS

SELECT AsycudaDocumentSetId, SUM(Total) AS Total, SUM(AllocatedTotal) AS AllocatedTotal, SUM(TotalLines) AS TotalLines, SUM(Tax) AS Tax, SUM(ClassifiedLines) AS ClassifiedLines, SUM(LicenseLines) AS LicenseLines, 
                 SUM(QtyLicensesRequired) AS QtyLicensesRequired, SUM(ImportedInvoices) AS ImportedInvoices
FROM    AsycudaDocumentSetEntryDataExTotalDetails
GROUP BY AsycudaDocumentSetId
--SELECT AsycudaDocumentSetEntryData.AsycudaDocumentSetId, SUM(CAST(isnull(EntryDataDetails.Quantity, 0) * isnull(EntryDataDetails.Cost, 0) AS float)) AS Total, SUM(CAST(EntryDataDetails.QtyAllocated * EntryDataDetails.Cost AS float)) 
--                 AS AllocatedTotal, COUNT(DISTINCT EntryDataDetails.EntryDataDetailsId) AS TotalLines, SUM(EntryDataDetails.TaxAmount) AS Tax, COUNT(InventoryItems.TariffCode) AS ClassifiedLines, 
--                 SUM(CAST(ISNULL(TariffCodes.LicenseRequired, 0) AS int)) AS LicenseLines, COUNT(DISTINCT (CASE WHEN dbo.TariffCodes.LicenseRequired = 1 THEN dbo.TariffCodes.TariffCode ELSE NULL END)) 
--                 AS QtyLicensesRequired, COUNT(DISTINCT dbo.AsycudaDocumentSetEntryData.EntryData_Id) AS ImportedInvoices
--FROM    EntryDataDetails WITH (NOLOCK) INNER JOIN
--                 EntryData WITH (NOLOCK) ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
--                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id and EntryDataDetails.EntryData_Id =  AsycudaDocumentSetEntryData.EntryData_Id left OUTER JOIN
--                 InventoryItemSource INNER JOIN
--                 InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id RIGHT OUTER JOIN
--                 InventoryItems ON InventoryItemSource.InventoryId = InventoryItems.Id LEFT OUTER JOIN
--                 TariffCodes ON InventoryItems.TariffCode = TariffCodes.TariffCode ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND EntryDataDetails.InventoryItemId = InventoryItems.Id
--WHERE (InventorySources.Name = N'POS') --and (AsycudaDocumentSetEntryData.AsycudaDocumentSetId = 4257)
--GROUP BY  AsycudaDocumentSetEntryData.AsycudaDocumentSetId


GO
