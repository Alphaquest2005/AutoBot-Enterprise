USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-DiscrepanciesErrors]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[TODO-DiscrepanciesErrors]
AS
/****** Script for SelectTopNRows command from SSMS 
No Already XMLed errors
 ******/
SELECT Entrydatadetailsid, AsycudaDocumentSetId, Type, InvoiceDate, EffectiveDate, EntryDataId as InvoiceNo, LineNumber, ItemNumber,ItemDescription, InvoiceQty, ReceivedQty,Quantity, Cost, PreviousCNumber, PreviousInvoiceNumber, Comment, Status,DutyFreePaid, Subject, EmailDate,EmailId, ApplicationSettingsId  FROM [dbo].[TODO-UnallocatedDiscreprencies] 
union 

SELECT Entrydatadetailsid, AsycudaDocumentSetId, Type, InvoiceDate, EffectiveDate, EntryDataId as InvoiceNo, LineNumber, ItemNumber,ItemDescription, InvoiceQty, ReceivedQty,Quantity, Cost, PreviousCNumber, PreviousInvoiceNumber, Comment, Status,DutyFreePaid, Subject,  EmailDate,EmailId, ApplicationSettingsId  FROM [dbo].[TODO-UngeneratedDiscrepancyOvers]

union
select Entrydatadetailsid , AsycudaDocumentSetId, Type, entrydatadate as InvoiceDate, EffectiveDate, EntryDataId as InvoiceNo, LineNumber, ItemNumber,ItemDescription, InvoiceQty, ReceivedQty,Quantity, Cost, CNumber as PreviousCNumber, PreviousInvoiceNumber, Comment, case when xStatus like '%Ex9 Bucket%' or xStatus like '%Failed universal Sales%' then 'Already Ex-warehoused' when xStatus like '%Failed All Sales Check:%'  then 'Over Ex-warehoused' when Status like 'Over Allocated%' then 'Over Allocated' else isnull(status,'') + '-' + isnull(xstatus,'') end as Status, DutyFreePaid, Subject,  EmailDate,EmailId, ApplicationSettingsId from [dbo].[TODO-UnExwarehousedAdjustments]  

union
select Entrydatadetailsid , AsycudaDocumentSetId, Type, InvoiceDate, EffectiveDate, InvoiceNo, LineNumber, ItemNumber,ItemDescription, InvoiceQty, ReceivedQty,Quantity, Cost, pCNumber as PreviousCNumber, PreviousInvoiceNumber, Comment, 'Already EXECUTED Previous CNumber:' + pCNumber + '-Line:' + cast(pLineNumber as nvarchar(50))  as Status, DutyFreePaid, Subject,  EmailDate,EmailId, ApplicationSettingsId from [dbo].[TODO-DiscrepanciesAlreadyXMLed] where pCNumber is not null
GO
