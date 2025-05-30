USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ERRReport-UnmappedItems]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[TODO-ERRReport-UnmappedItems]
AS
SELECT ApplicationSettingsId, CNumber, ReferenceNumber as Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, commercial_description as Description, 'Unknown Items' AS Error, cast(null as nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-UnknownAsycudaItems]
--union
--SELECT ApplicationSettingsId, null as CNumber, null as  Reference, null as RegistrationDate, null as DocumentType,null as LineNumber, ItemNumber, itemdescription as Description, 'Entry Data Items' AS Error, 'InvoiceNo:' + cast(invoiceno as nvarchar(30)) + '  Invoice Date:' + cast(invoicedate as nvarchar(10)) AS Info
--FROM    dbo.[TODO-Error-UnknownEntryData]
union 
SELECT ApplicationSettingsId, CNumber,  Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, commercial_description as Description, 'Asycuda Items' AS Error, cast(null as nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-UnmappedAsycudaItems]
GO
