USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentOversToXML]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[TODO-AdjustmentOversToXML]
AS

SELECT t.EntryDataDetailsId, isnull(t.EntryData_Id, AsycudaDocumentItemEntryDataDetails.EntryData_Id) as EntryData_Id,   t.EntryDataId, t.LineNumber, t.ItemNumber, t.Quantity, t.Units, t.ItemDescription, t.Cost, t.QtyAllocated, t.UnitWeight, t.DoNotAllocate, t.TariffCode, t.CNumber, t.CLineNumber, 
                 t.AsycudaDocumentSetId, t.InvoiceQty, t.ReceivedQty, t.PreviousInvoiceNumber, t.PreviousCNumber, t.Comment, t.Status, t.EffectiveDate, t.Currency, t.ApplicationSettingsId, t.Type, t.DutyFreePaid, t.EmailId, 
                 t.FileTypeId, t.InvoiceDate, t.Subject, t.EmailDate, CASE WHEN asycudadocumentitementrydatadetails.entrydatadetailsid IS NULL THEN 0 ELSE 1 END AS AlreadyExecuted, Vendor, SourceFile
FROM    (SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryData_Id, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, 
                                  AdjustmentDetails.Units, AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, ISNULL(AdjustmentDetails.DoNotAllocate, 0) 
                                  AS DoNotAllocate, AdjustmentDetails.TariffCode, AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, 
                                  AdjustmentDetails.ReceivedQty, AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.PreviousCNumber, AdjustmentDetails.Comment, AdjustmentDetails.Status, AdjustmentDetails.EffectiveDate, 
                                  AdjustmentDetails.Currency, AdjustmentDetails.ApplicationSettingsId, AdjustmentDetails.Type, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.EmailId, AdjustmentDetails.FileTypeId, 
                                  AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, Vendor, SourceFile
                 FROM     AdjustmentDetails WITH (NOLOCK) LEFT OUTER JOIN
                                  SystemDocumentSets ON AdjustmentDetails.AsycudaDocumentSetId = SystemDocumentSets.Id
                 WHERE  /*(SystemDocumentSets.Id IS NULL) AND*/ (ISNULL(AdjustmentDetails.IsReconciled, 0) <> 1) and (AdjustmentDetails.Quantity > 0) AND (ISNULL(AdjustmentDetails.DoNotAllocate, 0) <> 1) AND (AdjustmentDetails.Cost > 0)
                 GROUP BY AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.Units, AdjustmentDetails.ItemDescription, AdjustmentDetails.QtyAllocated, 
                                  AdjustmentDetails.UnitWeight, AdjustmentDetails.TariffCode, AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.Status, 
                                  AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.PreviousCNumber, AdjustmentDetails.Comment, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Cost, AdjustmentDetails.Currency, 
                                  AdjustmentDetails.ItemNumber, AdjustmentDetails.ReceivedQty, AdjustmentDetails.InvoiceQty, AdjustmentDetails.Quantity, AdjustmentDetails.DoNotAllocate, 
                                  AdjustmentDetails.ApplicationSettingsId, AdjustmentDetails.Type, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.EmailId, AdjustmentDetails.FileTypeId, AdjustmentDetails.InvoiceDate, 
                                  AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, AdjustmentDetails.EntryData_Id, AdjustmentDetails.InvoiceDate, Vendor, SourceFile
                  ) AS t FULL OUTER JOIN
                     (SELECT EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
                      FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
					  inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails_1.Asycuda_id
                      WHERE  (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse')) AS AsycudaDocumentItemEntryDataDetails ON t.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
where (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL)	
GROUP BY t.EntryDataDetailsId, t.EntryData_Id, t.EntryDataId, t.LineNumber, t.ItemNumber, t.Quantity, t.Units, t.ItemDescription, t.Cost, t.QtyAllocated, t.UnitWeight, t.TariffCode, t.CNumber, t.CLineNumber, 
                 t.AsycudaDocumentSetId, t.InvoiceQty, t.ReceivedQty, t.PreviousInvoiceNumber, t.PreviousCNumber, t.Comment, t.Status, t.EffectiveDate, t.Currency, t.ApplicationSettingsId, t.Type, t.DutyFreePaid, t.EmailId, 
                 t.FileTypeId, t.InvoiceDate, t.Subject, t.EmailDate, t.DoNotAllocate, AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId, AsycudaDocumentItemEntryDataDetails.EntryData_Id, Vendor, SourceFile



--SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryData_Id, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, 
--                 AdjustmentDetails.Units, AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, ISNULL(AdjustmentDetails.DoNotAllocate, 0) AS DoNotAllocate, 
--                 AdjustmentDetails.TariffCode, AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, AdjustmentDetails.ReceivedQty, 
--                 AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.PreviousCNumber, AdjustmentDetails.Comment, AdjustmentDetails.Status, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Currency, 
--                 AdjustmentDetails.ApplicationSettingsId, AdjustmentDetails.Type, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.EmailId, AdjustmentDetails.FileTypeId, AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, 
--                 AdjustmentDetails.EmailDate, CASE WHEN asycudadocumentitementrydatadetails.entrydatadetailsid IS NULL THEN 0 ELSE 1 END AS AlreadyExecuted
--FROM    AdjustmentDetails WITH (NOLOCK) LEFT OUTER JOIN
--                 SystemDocumentSets ON AdjustmentDetails.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
--                     (SELECT EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
--                      FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
--                      WHERE  (DocumentType IN ('IM7', 'OS7'))) AS AsycudaDocumentItemEntryDataDetails ON AdjustmentDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
--WHERE (SystemDocumentSets.Id IS NULL) and isnull(AdjustmentDetails.IsReconciled,0) <> 1 
--GROUP BY AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.Units, AdjustmentDetails.ItemDescription, AdjustmentDetails.QtyAllocated, 
--                 AdjustmentDetails.UnitWeight, AdjustmentDetails.TariffCode, AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AdjustmentDetails.AsycudaDocumentSetId, AdjustmentDetails.Status, 
--                 AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.PreviousCNumber, AdjustmentDetails.Comment, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Cost, AdjustmentDetails.Currency, 
--                 AdjustmentDetails.ItemNumber, AdjustmentDetails.ReceivedQty, AdjustmentDetails.InvoiceQty, AdjustmentDetails.Quantity, ISNULL(AdjustmentDetails.DoNotAllocate, 0), AdjustmentDetails.ApplicationSettingsId, 
--                 AdjustmentDetails.Type, AdjustmentDetails.DutyFreePaid, AdjustmentDetails.EmailId, AdjustmentDetails.FileTypeId, AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, 
--                 AdjustmentDetails.EntryData_Id, AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId, AdjustmentDetails.InvoiceDate
--HAVING (AdjustmentDetails.Quantity > 0) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (ISNULL(AdjustmentDetails.DoNotAllocate, 0) <> 1) AND (AdjustmentDetails.Cost > 0)
GO
