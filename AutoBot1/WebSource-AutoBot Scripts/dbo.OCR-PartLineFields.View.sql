USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[OCR-PartLineFields]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OCR-PartLineFields]
AS
SELECT dbo.[OCR-Invoices].Id, dbo.[OCR-Invoices].Name AS Invoice, dbo.[OCR-PartTypes].Name AS Part, dbo.[OCR-Lines].Name AS Line, dbo.[OCR-RegularExpressions].RegEx, dbo.[OCR-Fields].[Key], dbo.[OCR-Fields].Field, 
                 dbo.[OCR-Fields].EntityType, dbo.[OCR-Fields].IsRequired, dbo.[OCR-Fields].DataType, dbo.[OCR-FieldValue].Value, dbo.[OCR-Fields].AppendValues, dbo.[OCR-Fields].Id AS FieldId, dbo.[OCR-Fields].ParentId, 
                 dbo.[OCR-Lines].Id AS LineId
FROM    dbo.[OCR-FieldValue] RIGHT OUTER JOIN
                 dbo.[OCR-Fields] ON dbo.[OCR-FieldValue].Id = dbo.[OCR-Fields].Id RIGHT OUTER JOIN
                 dbo.[OCR-PartTypes] INNER JOIN
                 dbo.[OCR-Parts] INNER JOIN
                 dbo.[OCR-Invoices] ON dbo.[OCR-Parts].TemplateId = dbo.[OCR-Invoices].Id INNER JOIN
                 dbo.[OCR-Lines] ON dbo.[OCR-Parts].Id = dbo.[OCR-Lines].PartId ON dbo.[OCR-PartTypes].Id = dbo.[OCR-Parts].PartTypeId INNER JOIN
                 dbo.[OCR-RegularExpressions] ON dbo.[OCR-Lines].RegExId = dbo.[OCR-RegularExpressions].Id ON dbo.[OCR-Fields].LineId = dbo.[OCR-Lines].Id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[45] 2[9] 3) )"
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
         Top = -770
         Left = 0
      End
      Begin Tables = 
         Begin Table = "OCR-FieldValue"
            Begin Extent = 
               Top = 336
               Left = 44
               Bottom = 449
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Fields"
            Begin Extent = 
               Top = 336
               Left = 272
               Bottom = 491
               Right = 456
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-PartTypes"
            Begin Extent = 
               Top = 450
               Left = 44
               Bottom = 563
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Parts"
            Begin Extent = 
               Top = 492
               Left = 272
               Bottom = 626
               Right = 456
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Invoices"
            Begin Extent = 
               Top = 564
               Left = 44
               Bottom = 719
               Right = 266
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Lines"
            Begin Extent = 
               Top = 720
               Left = 44
               Bottom = 875
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-RegularExpressions"
            Begin Extent = 
               Top = 720
               Left = 272
               Bottom = 854
               Right = 456
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-PartLineFields'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         End
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
      Begin ColumnWidths = 11
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1833
         Width = 2252
         Width = 3129
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-PartLineFields'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-PartLineFields'
GO
