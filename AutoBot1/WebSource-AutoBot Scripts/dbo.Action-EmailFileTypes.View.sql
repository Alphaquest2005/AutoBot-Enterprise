USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Action-EmailFileTypes]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  CREATE VIEW [dbo].[Action-EmailFileTypes]
AS
SELECT top 100 percent       EmailFileTypes.Id, EmailMapping.ApplicationSettingsId, EmailMapping.Pattern, FileTypes.FilePattern, FileTypes.Description, EmailMapping.Id AS EmailMappingId, FileTypes.Id AS FileTypeId, 
                         [FileTypes-FileImporterInfo].EntryType, [FileTypes-FileImporterInfo].Format, Actions.Name AS Action
FROM            EmailMapping INNER JOIN
                         EmailFileTypes ON EmailMapping.Id = EmailFileTypes.EmailMappingId INNER JOIN
                         FileTypes ON EmailFileTypes.FileTypeId = FileTypes.Id INNER JOIN
                         [FileTypes-FileImporterInfo] ON FileTypes.FileInfoId = [FileTypes-FileImporterInfo].Id INNER JOIN
                         FileTypeActions ON FileTypes.Id = FileTypeActions.FileTypeId INNER JOIN
                         Actions ON FileTypeActions.ActionId = Actions.Id
ORDER BY EmailMappingId
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
         Begin Table = "EmailMapping"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 140
               Right = 266
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EmailFileTypes"
            Begin Extent = 
               Top = 6
               Left = 310
               Bottom = 161
               Right = 502
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "FileTypes"
            Begin Extent = 
               Top = 6
               Left = 546
               Bottom = 161
               Right = 785
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
      Begin ColumnWidths = 11
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Action-EmailFileTypes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Action-EmailFileTypes'
GO
