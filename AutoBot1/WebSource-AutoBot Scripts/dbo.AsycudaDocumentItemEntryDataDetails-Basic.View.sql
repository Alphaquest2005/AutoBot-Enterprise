USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetails-Basic]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetails-Basic]
--with schemabinding 
AS

SELECT  EntryDataDetailsId,  Item_Id 
FROM    [dbo].[AsycudaDocumentItemEntryDataDetailsA-Basic]

union 

SELECT  [AsycudaDocumentItemEntryDataDetailsC-Basic].EntryDataDetailsId, [AsycudaDocumentItemEntryDataDetailsC-Basic].Item_Id
FROM    [dbo].[AsycudaDocumentItemEntryDataDetailsC-Basic] left outer join [dbo].[AsycudaDocumentItemEntryDataDetailsA-Basic] on [AsycudaDocumentItemEntryDataDetailsC-Basic].EntryDataDetailsId = [AsycudaDocumentItemEntryDataDetailsA-Basic].EntryDataDetailsId
where [AsycudaDocumentItemEntryDataDetailsA-Basic].EntryDataDetailsId is null --and [AsycudaDocumentItemEntryDataDetailsC-Basic].EntryDataDetailsId = 198231
GO
