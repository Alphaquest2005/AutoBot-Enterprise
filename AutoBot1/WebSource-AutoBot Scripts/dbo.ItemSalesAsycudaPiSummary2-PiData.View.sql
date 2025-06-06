USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary2-PiData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary2-PiData]
AS
SELECT   ItemNumber, SUM(PiQuantity) AS PiQuantity, MonthYear, DutyFreePaid, EntryDataType
FROM      (SELECT   dbo.AsycudaItemPiQuantityData.ItemNumber, ISNULL(SUM(dbo.AsycudaItemPiQuantityData.PiQuantity), 0) AS PiQuantity, CONVERT(varchar(7), MAX(dbo.AsycudaItemPiQuantityData.AssessmentDate), 126) AS MonthYear,DutyFreePaid, EntryDataType
                 FROM      dbo.AsycudaItemPiQuantityData LEFT OUTER JOIN
                                 dbo.InventoryItemAliasEx ON dbo.AsycudaItemPiQuantityData.ItemNumber = dbo.InventoryItemAliasEx.AliasName
				 where   (dbo.InventoryItemAliasEx.ItemNumber IS NULL)
                 GROUP BY dbo.AsycudaItemPiQuantityData.ItemNumber, CONVERT(varchar(7), dbo.AsycudaItemPiQuantityData.AssessmentDate, 126), dbo.InventoryItemAliasEx.ItemNumber,DutyFreePaid, EntryDataType
                 
                 UNION
                 SELECT   InventoryItemAliasEx_1.ItemNumber, ISNULL(SUM(AsycudaItemPiQuantityData_1.PiQuantity), 0) AS PiQuantity, CONVERT(varchar(7), MAX(AsycudaItemPiQuantityData_1.AssessmentDate), 126) AS MonthYear,DutyFreePaid, EntryDataType
                 FROM      dbo.AsycudaItemPiQuantityData AS AsycudaItemPiQuantityData_1 INNER JOIN
                                 dbo.InventoryItemAliasEx AS InventoryItemAliasEx_1 ON AsycudaItemPiQuantityData_1.ItemNumber = InventoryItemAliasEx_1.AliasName
                 GROUP BY CONVERT(varchar(7), AsycudaItemPiQuantityData_1.AssessmentDate, 126), InventoryItemAliasEx_1.ItemNumber,DutyFreePaid, EntryDataType) AS t
GROUP BY ItemNumber, MonthYear, DutyFreePaid,  EntryDataType
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
         Begin Table = "t"
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
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 2805
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-PiData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-PiData'
GO
