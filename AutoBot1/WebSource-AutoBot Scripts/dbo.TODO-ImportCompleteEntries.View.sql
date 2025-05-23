USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ImportCompleteEntries]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[TODO-ImportCompleteEntries]
AS

SELECT o.AsycudaDocumentSetId, o.ApplicationSettingsId, o.EntryDataId,  o.EntryData_Id, o.EmailId, o.FileTypeId, i.ASYCUDA_Id as NewAsycuda_Id, o.ASYCUDA_Id as AssessedAsycuda_Id
			FROM [dbo].[TODO-ImportCompleteEntries-New] AS i WITH (NOLOCK)   INNER JOIN
                  [dbo].[TODO-ImportCompleteEntries-Assessed]  AS o WITH (NOLOCK)
				  ON o.Id = i.Id 
--where o.AsycudaDocumentSetId = 5395
GROUP BY o.AsycudaDocumentSetId, o.ApplicationSettingsId, o.EntryDataId, o.EntryData_Id, o.EmailId, o.FileTypeId, i.ASYCUDA_Id, o.ASYCUDA_Id
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
         Begin Table = "o"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "i"
            Begin Extent = 
               Top = 6
               Left = 311
               Bottom = 161
               Right = 534
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 6
               Left = 578
               Bottom = 161
               Right = 833
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 6
               Left = 877
               Bottom = 161
               Right = 1132
            End
            DisplayFlags = 280
            TopColumn = 14
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 131
               Left = 1311
               Bottom = 223
               Right = 1511
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 0
               Left = 1395
               Bottom = 260
               Right = 1683
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
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ImportCompleteEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       Width = 1309
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
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ImportCompleteEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ImportCompleteEntries'
GO
