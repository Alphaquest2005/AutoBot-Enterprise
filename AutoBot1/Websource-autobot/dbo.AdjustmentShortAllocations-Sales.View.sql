USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-Sales]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AdjustmentShortAllocations-Sales]
AS

SELECT        EntryDataDetails.TotalValue, EntryData.EntryDataDate AS InvoiceDate, ROUND(EntryDataDetails.Quantity, 1) AS SalesQuantity, ROUND(EntryDataDetails.QtyAllocated, 1) AS SalesQtyAllocated, 
                         EntryDataDetails.EntryDataId AS InvoiceNo, EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, EntryDataDetails.Cost, 
                         EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, EntryData_Sales.CustomerName, EntryDataDetails.EffectiveDate, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 0)) AS TaxAmount, 
                         EntryDataDetails.Comment, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) 
                         <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, EntryData.ApplicationSettingsId, EntryData.FileTypeId, EntryData.EmailId, 'Sales' AS Type, 0 AS SANumber
FROM            AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
                         SystemDocumentSets WITH (nolock) ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = SystemDocumentSets.Id RIGHT OUTER JOIN
                         EntryData_Sales AS EntryData_Sales WITH (NOLOCK) INNER JOIN
                         EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                         EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id ON AsycudaDocumentSetEntryData.EntryData_Id = EntryDataDetails.EntryData_Id
GO
