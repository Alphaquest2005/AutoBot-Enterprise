USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetailsA-Basic]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetailsA-Basic]
-- with schemabinding
AS
SELECT EntryDataDetailsId, Item_Id
FROM    dbo.[AsycudaDocumentItemEntryDataDetailsA-TBL]
WHERE (position > 0) 
--where xcuda_Item.PreviousInvoiceItemNumber like '%' +  EntryDataDetails.ItemNumber + '%' -- 
--where entrydataid like '%122089%' 
GO
