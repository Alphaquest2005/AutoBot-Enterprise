USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary-CTE]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary-CTE]
AS
WITH cte(ItemNumber, iSalesQty, MonthYear) AS (SELECT   dbo.EntryDataDetails.ItemNumber, SUM(dbo.EntryDataDetails.Quantity) AS SalesQuantity, CONVERT(varchar(7), MAX(dbo.EntryDataEx.InvoiceDate), 126) AS MonthYear
                                                                                 FROM      dbo.EntryDataDetails INNER JOIN
                                                                                                 dbo.EntryDataEx ON dbo.EntryDataDetails.EntryData_Id = dbo.EntryDataEx.EntryData_Id 
																								 left outer join EntryData_Sales on dbo.EntryDataEx.EntryData_Id = dbo.EntryData_Sales.EntryData_Id 
																								 left outer join EntryData_Adjustments on dbo.EntryDataEx.EntryData_Id = dbo.EntryData_Adjustments.EntryData_Id 
																				where (dbo.EntryData_Sales.EntryData_Id is not null or dbo.EntryData_Adjustments.EntryData_Id is not null )
                                                                                 --WHERE   (NOT (dbo.EntryDataEx.Type IN (N'PO', N'OPS')) or dbo.EntryDataEx.Type is null)
                                                                                 GROUP BY dbo.EntryDataDetails.ItemNumber, CONVERT(varchar(7), dbo.EntryDataEx.InvoiceDate, 126))
    SELECT itemSummary.Id, itemSummary.PreviousItem_Id, cte_1.ItemNumber, cte_1.iSalesQty As SalesQty, SUM(itemSummary.QtyAllocated) AS QtyAllocated, SUM(itemSummary.PiQuantity) AS PiQuantity, itemSummary.pQtyAllocated AS pQtyAllocated,itemSummary.pCNumber,
			itemSummary.pLineNumber, itemSummary.ApplicationSettingsId, itemSummary.Type, cte_1.MonthYear, itemSummary.EntryDataDate, itemSummary.DutyFreePaid, itemSummary.EntryDataType
    FROM      cte AS cte_1 LEFT OUTER JOIN
                    dbo.[ItemSalesAsycudaPiSummary-data] as itemSummary  ON cte_1.ItemNumber = itemSummary.ItemNumber AND cte_1.MonthYear = itemSummary.MonthYear
	where itemSummary.Id is not null
    GROUP BY cte_1.ItemNumber, cte_1.MonthYear, cte_1.iSalesQty,itemSummary.Id, itemSummary.PreviousItem_Id,itemSummary.pCNumber,
			itemSummary.pLineNumber, itemSummary.ApplicationSettingsId, itemSummary.Type, cte_1.MonthYear, itemSummary.EntryDataDate, itemSummary.DutyFreePaid, itemSummary.EntryDataType, itemSummary.pQtyAllocated
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
         Begin Table = "ItemSalesAsycudaPiSummary"
            Begin Extent = 
               Top = 6
               Left = 266
               Bottom = 146
               Right = 493
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "cte_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 127
               Right = 228
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary-CTE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary-CTE'
GO
