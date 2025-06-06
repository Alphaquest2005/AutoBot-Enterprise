USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[License-UnitOfMeasure]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[License-UnitOfMeasure]
AS
SELECT dbo.TariffCodes.TariffCode, MAX(dbo.TariffSupUnitLkps.SuppQty) AS SuppQty, MIN(dbo.TariffSupUnitLkps.SuppUnitCode2) AS SuppUnitCode2
FROM    dbo.TariffCategoryCodeSuppUnit INNER JOIN
                 dbo.TariffCategory ON dbo.TariffCategoryCodeSuppUnit.TariffCategoryCode = dbo.TariffCategory.TariffCategoryCode INNER JOIN
                 dbo.TariffSupUnitLkps ON dbo.TariffCategoryCodeSuppUnit.TariffSupUnitId = dbo.TariffSupUnitLkps.Id RIGHT OUTER JOIN
                 dbo.TariffCodes ON dbo.TariffCategory.TariffCategoryCode = dbo.TariffCodes.TariffCategoryCode
WHERE (dbo.TariffCodes.LicenseRequired = 1)
GROUP BY dbo.TariffCodes.TariffCode
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
         Begin Table = "TariffCategoryCodeSuppUnit"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 140
               Right = 255
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCategory"
            Begin Extent = 
               Top = 6
               Left = 299
               Bottom = 161
               Right = 550
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffSupUnitLkps"
            Begin Extent = 
               Top = 6
               Left = 594
               Bottom = 161
               Right = 788
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 6
               Left = 832
               Bottom = 161
               Right = 1065
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
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
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
         Or = 1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'License-UnitOfMeasure'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'License-UnitOfMeasure'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'License-UnitOfMeasure'
GO
