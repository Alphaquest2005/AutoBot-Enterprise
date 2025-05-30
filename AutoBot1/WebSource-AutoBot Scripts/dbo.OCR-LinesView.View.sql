USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[OCR-LinesView]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OCR-LinesView]
AS
SELECT        dbo.[OCR-Invoices].Name AS Invoice, dbo.[OCR-PartTypes].Name AS Part, dbo.[OCR-Lines].Name AS Line, dbo.[OCR-RegularExpressions].MultiLine, dbo.[OCR-RegularExpressions].RegEx, dbo.[OCR-Lines].Id, 
                         dbo.[OCR-RegularExpressions].Id AS RegExId, dbo.[OCR-Lines].ParentId, dbo.[OCR-Parts].Id AS PartId, dbo.[OCR-Lines].IsColumn, dbo.[OCR-Lines].DistinctValues, dbo.[OCR-Lines].IsActive, dbo.[OCR-Lines].Comments
FROM            dbo.[OCR-PartTypes] INNER JOIN
                         dbo.[OCR-Parts] ON dbo.[OCR-PartTypes].Id = dbo.[OCR-Parts].PartTypeId INNER JOIN
                         dbo.[OCR-Lines] ON dbo.[OCR-Parts].Id = dbo.[OCR-Lines].PartId INNER JOIN
                         dbo.[OCR-RegularExpressions] ON dbo.[OCR-Lines].RegExId = dbo.[OCR-RegularExpressions].Id RIGHT OUTER JOIN
                         dbo.[OCR-Invoices] ON dbo.[OCR-Parts].TemplateId = dbo.[OCR-Invoices].Id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[11] 3) )"
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
         Left = -340
      End
      Begin Tables = 
         Begin Table = "OCR-PartTypes"
            Begin Extent = 
               Top = 6
               Left = 378
               Bottom = 102
               Right = 548
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Parts"
            Begin Extent = 
               Top = 6
               Left = 586
               Bottom = 119
               Right = 756
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Lines"
            Begin Extent = 
               Top = 102
               Left = 378
               Bottom = 232
               Right = 548
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "OCR-RegularExpressions"
            Begin Extent = 
               Top = 120
               Left = 586
               Bottom = 250
               Right = 756
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Invoices"
            Begin Extent = 
               Top = 234
               Left = 378
               Bottom = 364
               Right = 580
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
         Width = 1305
         Width = 1305
         Width = 1305
         Width = 1305
         Width = 7005
         Width = 1305
         Width = 1305
         Width = 1305
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-LinesView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'365
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-LinesView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-LinesView'
GO
