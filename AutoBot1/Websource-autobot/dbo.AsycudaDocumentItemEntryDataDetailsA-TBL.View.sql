USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetailsA-TBL]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaDocumentItemEntryDataDetailsA-TBL]
  with schemabinding
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId, xcuda_Item.Item_Id, CHARINDEX(xcuda_Item.PreviousInvoiceItemNumber, EntryDataDetails.ItemNumber) as Position
FROM    dbo.EntryDataDetails  INNER JOIN
                 dbo.xcuda_Item  ON xcuda_Item.PreviousInvoiceKey = EntryDataDetails.EntryDataDetailsKey INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties ON xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE  Cancelled = 0
GO
