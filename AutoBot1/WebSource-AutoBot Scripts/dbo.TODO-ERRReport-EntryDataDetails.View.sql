USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ERRReport-EntryDataDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-ERRReport-EntryDataDetails]
AS
SELECT ApplicationSettingsId, InvoiceNo, InvoiceDate, Type, LineNumber,ItemNumber, ItemDescription, 'Mismatched Allocation' AS Error,  'QtyAllocated: ' + CAST((QtyAllocated) AS nvarchar(50)) + '  PiQuantity: ' + CAST(PiQuantity AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Mismatched Allocations-EntryDataDetails]
union
SELECT ApplicationSettingsId, InvoiceNo, InvoiceDate, Type, LineNumber,ItemNumber, ItemDescription, 'Status' AS Error,  'Status: ' + status AS Info
FROM    dbo.[TODO-Error-EntryDataDetails Status]
union
SELECT ApplicationSettingsId, InvoiceNo, InvoiceDate, Type, LineNumber,ItemNumber, ItemDescription, 'Unknown ItemCodes' AS Error,  null AS Info
FROM    dbo.[TODO-Error-UnknownEntryData]
union
SELECT ApplicationSettingsId, FEntryDataId as InvoiceNo, EntryDataDate as InvoiceDate, FileType as Type, FLineNumber as LineNumber,FItemNumber as ItemNumber,ItemDescription as ItemDescription, 'Import File Errors' AS Error,  null AS Info
FROM    dbo.[TODO-Error-EntryDataFiles]


GO
