USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentDetails-Old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO















CREATE VIEW [dbo].[AdjustmentDetails-Old]
AS
SELECT EntryDataDetailsEx.EntryDataDetailsId
	,Adjustments.EntryData_Id
	,EntryDataDetailsEx.EntryDataId
	,EntryDataDetailsEx.LineNumber
	,EntryDataDetailsEx.ItemNumber
	,ISNULL(EntryDataDetailsEx.ReceivedQty, 0) - ISNULL(EntryDataDetailsEx.InvoiceQty, 0) AS Quantity
	,EntryDataDetailsEx.Units
	,EntryDataDetailsEx.ItemDescription
	,CASE 
		WHEN Cost = 0
			THEN isnull(lastcost, 0)
		ELSE cost
		END AS Cost
	,EntryDataDetailsEx.QtyAllocated
	,EntryDataDetailsEx.UnitWeight
	,EntryDataDetailsEx.DoNotAllocate
	,EntryDataDetailsEx.TariffCode
	,EntryDataDetailsEx.Total
	,EntryDataDetailsEx.AsycudaDocumentSetId
	,EntryDataDetailsEx.InvoiceQty
	,EntryDataDetailsEx.ReceivedQty
	,EntryDataDetailsEx.STATUS
	,EntryDataDetailsEx.PreviousInvoiceNumber
	,EntryDataDetailsEx.CNumber
	,EntryDataDetailsEx.CLineNumber
	,EntryDataDetailsEx.Downloaded
	,EntryDataDetailsEx.PreviousCNumber
	,CASE 
		WHEN isnull(EntryDataDetailsEx.TaxAmount, isnull(Adjustments.Tax, 0)) <> 0
			THEN 'Duty Paid'
		ELSE 'Duty Free'
		END AS DutyFreePaid
	,Adjustments.Type
	,EntryDataDetailsEx.Comment
	,EntryDataDetailsEx.EffectiveDate
	,Adjustments.Currency
	,EntryDataDetailsEx.IsReconciled
	,Adjustments.ApplicationSettingsId
	,EntryDataDetailsEx.EmailId
	,EntryDataDetailsEx.FileTypeId
	,Adjustments.InvoiceDate
	,Emails.Subject
	,Emails.EmailDate
	,EntryDataDetailsEx.InventoryItemId
FROM  EntryDataDetailsEx WITH (NOLOCK)
INNER JOIN [EntryDataEx-Classifications] AS Adjustments WITH (NOLOCK) ON EntryDataDetailsEx.EntryData_Id = Adjustments.EntryData_Id
	AND EntryDataDetailsEx.ApplicationSettingsId = Adjustments.ApplicationSettingsId
LEFT OUTER JOIN Emails ON EntryDataDetailsEx.EmailId = Emails.EmailUniqueId
LEFT OUTER JOIN (
	SELECT InvoiceDate
		,Type
		,ImportedTotal
		,InvoiceNo
		,InvoiceTotal
		,ImportedLines
		,TotalLines
		,Currency
		,ApplicationSettingsId
		,DutyFreePaid
		,EmailId
		,FileTypeId
		,SupplierCode
		,ExpectedTotal
		,ClassifiedLines
		,AsycudaDocumentSetId
		,TotalInternalFreight
		,TotalInternalInsurance
		,TotalOtherCost
		,TotalDeductions
		,TotalFreight
	FROM [EntryDataEx-Classifications] as EntryDataEx WITH (NOLOCK)
	WHERE (Type = 'SALES')
	) AS Sales ON EntryDataDetailsEx.ApplicationSettingsId = Sales.ApplicationSettingsId
	AND EntryDataDetailsEx.PreviousInvoiceNumber = Sales.InvoiceNo
WHERE (Sales.InvoiceNo IS NULL)
	AND (Adjustments.Type = N'ADJ')
	OR (Adjustments.Type = N'DIS')
GROUP BY EntryDataDetailsEx.EntryDataDetailsId
	,EntryDataDetailsEx.EntryDataId
	,EntryDataDetailsEx.LineNumber
	,EntryDataDetailsEx.ItemNumber
	,EntryDataDetailsEx.Units
	,EntryDataDetailsEx.ItemDescription
	,EntryDataDetailsEx.QtyAllocated
	,EntryDataDetailsEx.UnitWeight
	,EntryDataDetailsEx.TariffCode
	,EntryDataDetailsEx.Total
	,EntryDataDetailsEx.AsycudaDocumentSetId
	,EntryDataDetailsEx.InvoiceQty
	,EntryDataDetailsEx.ReceivedQty
	,EntryDataDetailsEx.STATUS
	,EntryDataDetailsEx.PreviousInvoiceNumber
	,EntryDataDetailsEx.CNumber
	,EntryDataDetailsEx.CLineNumber
	,EntryDataDetailsEx.PreviousCNumber
	,EntryDataDetailsEx.DutyFreePaid
	,Adjustments.Type
	,EntryDataDetailsEx.Comment
	,EntryDataDetailsEx.EffectiveDate
	,Adjustments.Currency
	,Adjustments.ApplicationSettingsId
	,EntryDataDetailsEx.DoNotAllocate
	,EntryDataDetailsEx.Downloaded
	,EntryDataDetailsEx.IsReconciled
	,EntryDataDetailsEx.Cost
	,EntryDataDetailsEx.LastCost
	,EntryDataDetailsEx.EmailId
	,EntryDataDetailsEx.FileTypeId
	,Adjustments.InvoiceDate
	,Emails.Subject
	,Emails.EmailDate
	,Adjustments.EntryData_Id
	,EntryDataDetailsEx.InventoryItemId
	,Adjustments.Tax
	,EntryDataDetailsEx.TaxAmount
GO
