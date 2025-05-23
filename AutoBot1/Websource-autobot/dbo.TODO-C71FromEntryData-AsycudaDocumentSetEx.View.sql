USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71FromEntryData-AsycudaDocumentSetEx]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[TODO-C71FromEntryData-AsycudaDocumentSetEx]
AS
SELECT AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.Currency_Code, AsycudaDocumentSet.ExpectedEntries, ISNULL(AsycudaDocumentSet.ApplicationSettingsId, 0) AS ApplicationSettingsId, 
                 AsycudaDocumentSetEntryDataExTotals.ImportedInvoices, CurrencyRates.Rate AS CurrencyRate, AsycudaDocumentSet.TotalInvoices
FROM    AsycudaDocumentSetEntryDataExTotals RIGHT OUTER JOIN
                 AsycudaDocumentSet WITH (NOLOCK) ON AsycudaDocumentSetEntryDataExTotals.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 CurrencyRates ON CurrencyRates.CurrencyCode = AsycudaDocumentSet.Currency_Code
GROUP BY AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.Currency_Code, AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSetEntryDataExTotals.ImportedInvoices, CurrencyRates.Rate, 
                 AsycudaDocumentSet.ExpectedEntries, AsycudaDocumentSet.TotalInvoices
GO
