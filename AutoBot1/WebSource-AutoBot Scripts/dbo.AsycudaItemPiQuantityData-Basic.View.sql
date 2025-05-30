USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaItemPiQuantityData-Basic]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[AsycudaItemPiQuantityData-Basic]
 
AS
SELECT EntryPreviousItems.Item_Id, xcuda_PreviousItem.Suplementary_Quantity AS PiQuantity, xcuda_PreviousItem.Net_weight AS PiWeight, xcuda_PreviousItem.PreviousItem_Id AS xItem_Id, 
                  
                 PreviousDocumentItem.LineNumber AS pLineNumber
			
FROM    dbo.EntryPreviousItems  INNER JOIN
                 dbo.xcuda_PreviousItem  ON EntryPreviousItems.PreviousItem_Id = xcuda_PreviousItem.PreviousItem_Id AND EntryPreviousItems.Item_Id <> xcuda_PreviousItem.PreviousItem_Id INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties  ON xcuda_PreviousItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_item AS PreviousDocumentItem ON EntryPreviousItems.Item_Id = PreviousDocumentItem.Item_Id 
WHERE (xcuda_ASYCUDA_ExtendedProperties.Cancelled IS NULL) OR
                 (xcuda_ASYCUDA_ExtendedProperties.Cancelled = 0)
--GROUP BY EntryPreviousItems.Item_Id, xcuda_PreviousItem.PreviousItem_Id,  xcuda_PreviousItem.Suplementary_Quantity, xcuda_PreviousItem.Net_weight, 
--                 PreviousDocumentItem.LineNumber
GO
