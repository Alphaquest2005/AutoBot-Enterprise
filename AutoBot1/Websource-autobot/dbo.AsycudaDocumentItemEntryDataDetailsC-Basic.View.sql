USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetailsC-Basic]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetailsC-Basic]
 
AS
--- don't consider quantity because of lumped items
--- this ignore the linenumber incase its wrong 
SELECT EntryDataDetails.EntryDataDetailsId, xcuda_Item.Item_Id
FROM    EntryDataDetails WITH (NOLOCK) INNER JOIN
                 xcuda_Item WITH (NOLOCK) ON xcuda_Item.PreviousInvoiceKey <> EntryDataDetails.EntryDataDetailsKey AND xcuda_Item.PreviousInvoiceItemNumber = EntryDataDetails.ItemNumber AND 
                 xcuda_Item.PreviousInvoiceNumber = EntryDataDetails.EntryDataId INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE (CHARINDEX(xcuda_Item.PreviousInvoiceItemNumber, EntryDataDetails.ItemNumber) > 0) and  Cancelled = 0
GO
