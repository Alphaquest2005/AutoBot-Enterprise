USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ERRReport-Allocations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-ERRReport-Allocations]
AS
SELECT ApplicationSettingsId, pcnumber as CNumber, preferencenumber as Reference, pregistrationdate as RegistrationDate, DocumentType,plinenumber as LineNumber,ItemNumber, Description,InvoiceNo, InvoiceDate, 'Allocation Error' AS Error, 'Error: ' + CAST(status AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-AllocationErrors]
union
SELECT ApplicationSettingsId, pcnumber as CNumber, preferencenumber as Reference, pregistrationdate as RegistrationDate, DocumentType,plinenumber as LineNumber,ItemNumber, itemDescription,InvoiceNo, InvoiceDate, 'XBond Error' AS Error, 'Error: ' + CAST(xstatus AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Error-ExBondErrors]
union
SELECT ApplicationSettingsId, CNumber, referencenumber as Reference, RegistrationDate, DocumentType, LineNumber,ItemNumber, Commercial_Description,null as InvoiceNo,null as InvoiceDate, 'Mismatched Allocations-Asycuda' AS Error, 'QtyAllocated: ' + CAST((DFQtyAllocated + DPQtyAllocated) AS nvarchar(50)) + '  PiQuantity: ' + CAST(PiQuantity AS nvarchar(50)) AS Info
FROM    dbo.[TODO-Mismatched Allocations-Asycuda]


GO
