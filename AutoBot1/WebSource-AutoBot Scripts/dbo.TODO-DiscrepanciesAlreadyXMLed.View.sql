USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-DiscrepanciesAlreadyXMLed]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[TODO-DiscrepanciesAlreadyXMLed]
AS

SELECT EntryDataDetailsId, EntryData_Id, ApplicationSettingsId, AsycudaDocumentSetId, IsClassified, AdjustmentType, InvoiceNo, InvoiceQty, ReceivedQty, InvoiceDate, ItemNumber, Status, PreviousCNumber, 
                 Declarant_Reference_Number, pCNumber, pLineNumber, RegistrationDate, ReferenceNumber, DocumentType, Type, EffectiveDate, ItemDescription, EmailId, FileTypeId, Quantity, Cost, Subject, EmailDate, LineNumber, 
                 PreviousInvoiceNumber, Comment, DutyFreePaid
FROM    [TODO-AdjustmentsAlreadyXMLed]
WHERE (Type = 'DIS')


GO
