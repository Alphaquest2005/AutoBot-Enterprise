USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ERRReport-ByItemNumber]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-ERRReport-ByItemNumber]
AS
SELECT ApplicationSettingsId, CNumber,  Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description,  Error,  Info
FROM    dbo.[TODO-ERRReport-Allocations] 
union
SELECT ApplicationSettingsId, CNumber,  Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description,  Error,  Info
FROM    dbo.[TODO-ERRReport-AsycudaLines] 
union
SELECT ApplicationSettingsId, CNumber,  Reference, RegistrationDate, DocumentType,LineNumber,ItemNumber, Description,  Error,  Info
FROM    dbo.[TODO-ERRReport-UnmappedItems] 
union
SELECT ApplicationSettingsId, InvoiceNo as CNumber,  null as Reference, InvoiceDate as RegistrationDate, Type as DocumentType, LineNumber,ItemNumber, itemdescription as Description,  Error,  Info
FROM    dbo.[TODO-ERRReport-EntryDataDetails] 

GO
