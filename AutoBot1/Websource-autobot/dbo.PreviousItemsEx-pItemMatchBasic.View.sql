USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[PreviousItemsEx-pItemMatchBasic]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[PreviousItemsEx-pItemMatchBasic]
with schemabinding
as
SELECT xcuda_PreviousItem.PreviousItem_Id, pItem.Item_Id AS PreviousDocumentItemId, SalesFactor, pItem.EntryDataType
FROM    dbo.xcuda_Item AS pItem INNER JOIN
                 dbo.xcuda_Registration ON pItem.ASYCUDA_Id = dbo.xcuda_Registration.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_HScode ON pItem.Item_Id = dbo.xcuda_HScode.Item_Id INNER JOIN
                 dbo.xcuda_PreviousItem ON dbo.xcuda_HScode.Precision_4 = dbo.xcuda_PreviousItem.Prev_decl_HS_spec AND pItem.LineNumber = dbo.xcuda_PreviousItem.Previous_item_number AND 
                 dbo.xcuda_Registration.Number = dbo.xcuda_PreviousItem.Prev_reg_nbr
--where xcuda_PreviousItem.PreviousItem_Id = 417852
--order by xcuda_PreviousItem.PreviousItem_Id

--CREATE UNIQUE CLUSTERED INDEX IDX_V5
--   ON [PreviousItemsEx-pItemMatchBasic] (PreviousItem_Id, PreviousDocumentItemId, SalesFactor);
GO
