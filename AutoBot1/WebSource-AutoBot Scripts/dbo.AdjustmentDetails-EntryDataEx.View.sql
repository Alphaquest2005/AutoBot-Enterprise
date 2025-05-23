USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentDetails-EntryDataEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO












CREATE VIEW [dbo].[AdjustmentDetails-EntryDataEx]
AS

SELECT EntryData.EntryData_Id, EntryData.EntryDataDate AS InvoiceDate,                
                 entrydata_adjustments.Type, EntryData.EntryDataId AS InvoiceNo, EntryData.InvoiceTotal, EntryData.ImportedLines, 
                 EntryData.Currency, EntryData.ApplicationSettingsId, CASE WHEN isnull( EntryData_Adjustments.Tax, 0) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, 
                 EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode, AsycudaDocumentSetEntryData.AsycudaDocumentSetId,  EntryData.SourceFile,  EntryData_Adjustments.Tax, Vendor
FROM    EntryData WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
GROUP BY EntryData.EntryData_Id, EntryData.EntryDataDate, EntryData.EntryDataId, EntryData.InvoiceTotal, EntryData.ImportedLines, EntryData.Currency, EntryData.ApplicationSettingsId, EntryData.EmailId, 
                 EntryData.FileTypeId, EntryData.SupplierCode, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryData.SourceFile, EntryData_Adjustments.EntryData_Id, EntryData_Adjustments.Type, 
                 EntryData_Adjustments.Tax, Vendor
GO
