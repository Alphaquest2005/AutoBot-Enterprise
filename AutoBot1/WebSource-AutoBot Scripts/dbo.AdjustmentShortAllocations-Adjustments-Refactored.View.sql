USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-Adjustments-Refactored]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[AdjustmentShortAllocations-Adjustments-Refactored]
AS
SELECT  EntryDataDetails.TotalValue, isnull(EntryDataDetails.EffectiveDate, EntryData.EntryDataDate) AS InvoiceDate, ROUND(EntryDataDetails.Quantity, 1) AS SalesQuantity, ROUND(EntryDataDetails.QtyAllocated, 1) 
                 AS SalesQtyAllocated, EntryDataDetails.EntryDataId AS InvoiceNo, EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
                 EntryDataDetails.EntryDataDetailsId, EntryDataDetails.Cost, EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, 
                  EntryDataDetails.EffectiveDate, 
                 EntryDataDetails.Comment, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, coalesce(
		CASE when AdjustmentComments.Comments is not null then AdjustmentComments.DutyFreePaid else null end,
		case when isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) <> 0
			THEN 'Duty Paid'
		ELSE 'Duty Free'
		END) AS DutyFreePaid, EntryData.ApplicationSettingsId, EntryData.FileTypeId, EntryData.EmailId, EntryData_Sales.Type,
				 0 as SANumber
				 -- cast(row_number() OVER (partition BY dbo.Entrydatadetails.entrydataid	ORDER BY dbo.Entrydatadetails.entrydatadetailsid) AS int) AS SANumber
					, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 
                 0)) AS TaxAmount
FROM    AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
                 EntryData_Adjustments AS EntryData_Sales WITH (NOLOCK) INNER JOIN
                 EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id ON AsycudaDocumentSetEntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 SystemDocumentSets WITH (nolock) ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = SystemDocumentSets.Id
				 left outer join AdjustmentComments on EntryDataDetails.Comment = AdjustmentComments.Comments
--GROUP BY EntryData.EntryDataDate, EntryDataDetails.Quantity, EntryDataDetails.QtyAllocated, EntryDataDetails.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
--                 EntryDataDetails.EntryDataDetailsId, EntryDataDetails.LineNumber, EntryDataDetails.Cost, EntryDataDetails.Quantity, EntryDataDetails.DoNotAllocate, EntryDataDetails.Cost, EntryDataDetails.EffectiveDate, 
--                 EntryDataDetails.Comment, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryDataDetails.TaxAmount, EntryData.ApplicationSettingsId, EntryData_Sales.Tax, EntryData.FileTypeId, EntryData.EmailId, 
--                 EntryData_Sales.Type, EntryDataDetails.LineNumber, EntryDataDetails.TaxAmount, EntryData_Sales.Tax
GO
