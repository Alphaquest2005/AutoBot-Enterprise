USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-EntryDataFiles]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-Error-EntryDataFiles]
AS
SELECT e.SourceFile AS FSourceFile, s.SourceFile AS DSourceFile, e.LineNumber AS FLineNumber, s.LineNumber AS DLineNumber, e.ItemNumber AS FItemNumber, s.ItemNumber AS dItemNumber, 
                 e.EntryDataId AS FEntryDataId, s.EntryDataId AS DEntryDataId, e.ApplicationSettingsId, e.ItemDescription, e.FileType, e.EntryDataDate
FROM    (SELECT TOP (100) PERCENT ApplicationSettingsId, EntryDataId, ItemNumber, LineNumber, SourceFile, ItemDescription, FileType, EntryDataDate
                 FROM     dbo.EntryDataFiles
                 GROUP BY EntryDataId, ItemNumber, LineNumber, SourceFile, ApplicationSettingsId, ItemDescription, FileType, EntryDataDate
                 ORDER BY SourceFile, LineNumber) AS e LEFT OUTER JOIN
                     (SELECT TOP (100) PERCENT dbo.AdjustmentDetails.ApplicationSettingsId, dbo.AdjustmentDetails.EntryDataId, dbo.AdjustmentDetails.ItemNumber, dbo.AdjustmentDetails.LineNumber, 
                                       dbo.EntryData.SourceFile
                      FROM     dbo.AdjustmentDetails INNER JOIN
                                       dbo.EntryData ON dbo.AdjustmentDetails.EntryDataId = dbo.EntryData.EntryDataId
                      WHERE  (dbo.AdjustmentDetails.AsycudaDocumentSetId <> 8) AND (dbo.AdjustmentDetails.Type = 'DIS')
                      GROUP BY dbo.AdjustmentDetails.EntryDataId, dbo.AdjustmentDetails.ItemNumber, dbo.AdjustmentDetails.LineNumber, dbo.EntryData.SourceFile, dbo.AdjustmentDetails.ApplicationSettingsId
                      ORDER BY dbo.AdjustmentDetails.ItemNumber) AS s ON e.ItemNumber = s.ItemNumber AND e.LineNumber = s.LineNumber AND e.SourceFile = s.SourceFile AND e.EntryDataId = s.EntryDataId AND 
                 e.ApplicationSettingsId = s.ApplicationSettingsId
WHERE (s.SourceFile IS NULL) AND (e.LineNumber IS NULL) OR
                 (s.LineNumber IS NULL)
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
         Begin Table = "s"
            Begin Extent = 
               Top = 11
               Left = 522
               Bottom = 259
               Right = 744
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 295
               Right = 266
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
      Begin ColumnWidths = 11
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-EntryDataFiles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-EntryDataFiles'
GO
