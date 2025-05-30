USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[AdjustmentEx]
AS
SELECT dbo.EntryDataEx.EntryData_Id, dbo.EntryDataEx.InvoiceDate,dbo.EntryData_Adjustments.EffectiveDate, ISNULL(dbo.EntryData_Adjustments.Type, dbo.EntryDataEx.Type) AS Type, dbo.EntryDataEx.DutyFreePaid, dbo.EntryDataEx.ImportedTotal, dbo.EntryDataEx.InvoiceNo, 
                 dbo.EntryDataEx.InvoiceTotal, dbo.EntryDataEx.ImportedLines, dbo.EntryDataEx.TotalLines, dbo.EntryDataEx.Currency, dbo.EntryDataEx.ApplicationSettingsId, dbo.EntryDataEx.EmailId, 
                 dbo.EntryDataEx.FileTypeId
FROM    dbo.EntryDataEx WITH (NOLOCK) INNER JOIN
                 dbo.EntryData_Adjustments WITH (NOLOCK) ON dbo.EntryDataEx.EntryData_Id = dbo.EntryData_Adjustments.EntryData_Id
GROUP BY dbo.EntryDataEx.InvoiceDate, dbo.EntryData_Adjustments.Type, dbo.EntryDataEx.Type, dbo.EntryDataEx.DutyFreePaid, dbo.EntryDataEx.ImportedTotal, dbo.EntryDataEx.InvoiceNo, 
                 dbo.EntryDataEx.InvoiceTotal, dbo.EntryDataEx.ImportedLines, dbo.EntryDataEx.TotalLines, dbo.EntryDataEx.Currency, dbo.EntryDataEx.ApplicationSettingsId, dbo.EntryDataEx.EmailId, dbo.EntryDataEx.FileTypeId,EntryDataEx.EntryData_Id, dbo.EntryData_Adjustments.EffectiveDate
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[29] 4[14] 2[15] 3) )"
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
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 260
               Right = 389
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 7
               Left = 637
               Bottom = 120
               Right = 821
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
      Begin ColumnWidths = 13
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
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
         Column = 4097
         Alias = 903
         Table = 4307
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentEx'
GO
