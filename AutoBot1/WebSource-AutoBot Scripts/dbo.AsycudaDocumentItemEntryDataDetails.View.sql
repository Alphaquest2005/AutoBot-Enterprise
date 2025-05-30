USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO















--drop view [dbo].[AsycudaDocumentItemEntryDataDetails]

CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetails]
 
AS
select ROW_NUMBER() over (partition by t.applicationsettingsid order by entrydatadetailsid) as Id, t.* from
(SELECT ApplicationSettingsId, [AsycudaDocumentItemEntryDataDetailsA].AsycudaDocumentSetId, EntryDataDetailsId, EntryData_Id, [AsycudaDocumentItemEntryDataDetailsA].Item_Id, [AsycudaDocumentItemEntryDataDetailsA].ItemNumber, [key], DocumentType, CustomsProcedure, Quantity, ImportComplete, [AsycudaDocumentItemEntryDataDetailsA].Asycuda_id, EntryDataType, CNumber, LineNumber, CustomsOperation
FROM    [dbo].[AsycudaDocumentItemEntryDataDetailsA] 

union 

SELECT ApplicationSettingsId, AsycudaDocumentSetId, [AsycudaDocumentItemEntryDataDetailsC].EntryDataDetailsId, EntryData_Id, [AsycudaDocumentItemEntryDataDetailsC].Item_Id, ItemNumber, [key], DocumentType, CustomsProcedure, Quantity, ImportComplete, Asycuda_id, EntryDataType, CNumber, LineNumber, CustomsOperation
FROM    [dbo].[AsycudaDocumentItemEntryDataDetailsC] left outer join [dbo].[AsycudaDocumentItemEntryDataDetailsA-Basic] on [AsycudaDocumentItemEntryDataDetailsC].EntryDataDetailsId = [AsycudaDocumentItemEntryDataDetailsA-Basic].EntryDataDetailsId
where [AsycudaDocumentItemEntryDataDetailsA-Basic].EntryDataDetailsId is null) as t
GO
