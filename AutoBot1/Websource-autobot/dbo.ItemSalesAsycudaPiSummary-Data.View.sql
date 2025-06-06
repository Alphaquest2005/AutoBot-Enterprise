USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary-Data]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary-Data]
AS

---select * from [ItemSalesAsycudaPiSummary-CTE]
select *, cast(ROW_NUMBER() OVER (ORDER BY PreviousItem_Id) AS int) AS Id 
from
 (
SELECT  distinct
			     COALESCE (ItemSalesPiSummary.PreviousItem_Id, AsycudaItemPiQuantityData.PreviousItem_Id) AS PreviousItem_Id, COALESCE (AsycudaItemPiQuantityData.ItemNumber,ItemSalesPiSummary.ItemNumber ) AS ItemNumber, 
                 ISNULL(ItemSalesPiSummary.SalesQty , 0) AS SalesQty, sum(ISNULL(ItemSalesPiSummary.QtyAllocated , 0)) AS QtyAllocated,
				 ISNULL(ItemSalesPiSummary.pQtyAllocated, PiQuantity) AS pQtyAllocated, -- when this is null is some mix up s gonna use the piquantity as pquantityallocated
				 isnull(PiQuantity, 0) as PiQuantity, 
                 COALESCE (ItemSalesPiSummary.pCNumber, AsycudaItemPiQuantityData.pCNumber) AS pCNumber, COALESCE (ItemSalesPiSummary.pLineNumber, AsycudaItemPiQuantityData.pLineNumber) AS pLineNumber, 
                 COALESCE (ItemSalesPiSummary.ApplicationSettingsId, AsycudaItemPiQuantityData.ApplicationSettingsId) AS ApplicationSettingsId, ItemSalesPiSummary.Type, COALESCE (ItemSalesPiSummary.MonthYear, AsycudaItemPiQuantityData.MonthYear) AS MonthYear, MAX(COALESCE (ItemSalesPiSummary.EntryDataDate, 
                 AsycudaItemPiQuantityData.EntryDataDate)) AS EntryDataDate, COALESCE (ItemSalesPiSummary.DutyFreePaid, AsycudaItemPiQuantityData.DutyFreePaid) AS DutyFreePaid,
				 COALESCE (ItemSalesPiSummary.EntryDataType, AsycudaItemPiQuantityData.EntryDataType) AS EntryDataType, case when AsycudaItemPiQuantityData.Type = null then AsycudaItemPiQuantityData.MonthYear else ItemSalesPiSummary.MonthYear end as ter--, entrydatadetailsid

FROM    [ItemSalesAsycudaPiSummary-Sales] as ItemSalesPiSummary FULL OUTER JOIN
                 [ItemSalesAsycudaPiSummary-Asycuda] as AsycudaItemPiQuantityData ON ItemSalesPiSummary.DutyFreePaid = AsycudaItemPiQuantityData.DutyFreePaid AND ItemSalesPiSummary.PreviousItem_Id = AsycudaItemPiQuantityData.PreviousItem_Id
				 and  ItemSalesPiSummary.MonthYear = AsycudaItemPiQuantityData.MonthYear and ItemSalesPiSummary.EntryDataType = AsycudaItemPiQuantityData.EntryDataType
             --  and  ItemSalesPiSummary.MonthYear = case when AsycudaItemPiQuantityData.Type = null then AsycudaItemPiQuantityData.MonthYear else ItemSalesPiSummary.MonthYear   end
where  COALESCE (ItemSalesPiSummary.PreviousItem_Id, AsycudaItemPiQuantityData.PreviousItem_Id)  is not null 
			--and  ItemSalesPiSummary.ItemNumber in ('SJT/039BL-GL-2P' ) 
GROUP BY ItemSalesPiSummary.PreviousItem_Id, AsycudaItemPiQuantityData.PreviousItem_Id, AsycudaItemPiQuantityData.ItemNumber, ItemSalesPiSummary.ItemNumber
		, ItemSalesPiSummary.SalesQty, ItemSalesPiSummary.pQtyAllocated,AsycudaItemPiQuantityData.PiQuantity,ItemSalesPiSummary.pCNumber, AsycudaItemPiQuantityData.pCNumber,ItemSalesPiSummary.pLineNumber,AsycudaItemPiQuantityData.pLineNumber
		, ItemSalesPiSummary.ApplicationSettingsId , AsycudaItemPiQuantityData.ApplicationSettingsId,ItemSalesPiSummary.Type,
            ItemSalesPiSummary.MonthYear,  AsycudaItemPiQuantityData.MonthYear,
             ItemSalesPiSummary.DutyFreePaid, AsycudaItemPiQuantityData.DutyFreePaid, ItemSalesPiSummary.EntryDataType, AsycudaItemPiQuantityData.EntryDataType
) as t
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ItemSalesPiSummary"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 282
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemPiQuantityData"
            Begin Extent = 
               Top = 6
               Left = 326
               Bottom = 161
               Right = 564
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary-Data'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary-Data'
GO
