USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-DiscrepanciesToDoList]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-DiscrepanciesToDoList]
AS
/* and entrydatadetailsid = 532649*/
SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, AdjustmentDetails.Units, 
                 AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, AdjustmentDetails.DoNotAllocate, AdjustmentDetails.TariffCode, 
                 AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, AdjustmentDetails.ReceivedQty, AdjustmentDetails.PreviousInvoiceNumber, 
                 AdjustmentDetails.PreviousCNumber, AdjustmentDetails.Comment, AdjustmentDetails.Status, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Currency, AdjustmentDetails.ApplicationSettingsId, 
                 AdjustmentDetails.Type, AsycudaDocumentSet.Declarant_Reference_Number, AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.EmailId
FROM    AsycudaDocumentSet WITH (NOLOCK) INNER JOIN
                 AdjustmentDetails WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEntryData WITH (NOLOCK) ON AdjustmentDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id ON 
                 AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId AND 
                 AsycudaDocumentSet.AsycudaDocumentSetId <> AdjustmentDetails.AsycudaDocumentSetId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (AdjustmentDetails.Type = 'DIS') AND (SystemDocumentSets.Id IS NULL) 
GO
