USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[OCR-PartsView]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OCR-PartsView]
AS
SELECT dbo.[OCR-Invoices].Name AS Invoice, dbo.[OCR-PartTypes].Name AS Part, dbo.[OCR-RegularExpressions].MultiLine AS StartMultiLine, dbo.[OCR-RegularExpressions].RegEx AS Start, 
                 [OCR-RegularExpressions_1].MultiLine AS EndMultiLine, [OCR-RegularExpressions_1].RegEx AS [End], CAST(CASE WHEN [ocr-recuringPart].Id IS NULL THEN 0 ELSE 1 END AS bit) AS IsRecuring, 
                 dbo.[OCR-RecuringPart].IsComposite, dbo.[OCR-Parts].Id, dbo.[OCR-Start].RegExId AS StartRegExId, dbo.[OCR-Invoices].Id AS InvoiceId
FROM    dbo.[OCR-PartTypes] INNER JOIN
                 dbo.[OCR-Parts] ON dbo.[OCR-PartTypes].Id = dbo.[OCR-Parts].PartTypeId LEFT OUTER JOIN
                 dbo.[OCR-RecuringPart] ON dbo.[OCR-Parts].Id = dbo.[OCR-RecuringPart].Id LEFT OUTER JOIN
                 dbo.[OCR-End] INNER JOIN
                 dbo.[OCR-RegularExpressions] AS [OCR-RegularExpressions_1] ON dbo.[OCR-End].RegExId = [OCR-RegularExpressions_1].Id ON dbo.[OCR-Parts].Id = dbo.[OCR-End].PartId LEFT OUTER JOIN
                 dbo.[OCR-Start] INNER JOIN
                 dbo.[OCR-RegularExpressions] ON dbo.[OCR-Start].RegExId = dbo.[OCR-RegularExpressions].Id ON dbo.[OCR-Parts].Id = dbo.[OCR-Start].PartId RIGHT OUTER JOIN
                 dbo.[OCR-Invoices] ON dbo.[OCR-Parts].TemplateId = dbo.[OCR-Invoices].Id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[30] 4[31] 2[10] 3) )"
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
         Top = -660
         Left = 0
      End
      Begin Tables = 
         Begin Table = "OCR-PartTypes"
            Begin Extent = 
               Top = 226
               Left = 44
               Bottom = 339
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Parts"
            Begin Extent = 
               Top = 226
               Left = 272
               Bottom = 360
               Right = 456
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-RecuringPart"
            Begin Extent = 
               Top = 340
               Left = 44
               Bottom = 453
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-End"
            Begin Extent = 
               Top = 364
               Left = 272
               Bottom = 498
               Right = 456
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-RegularExpressions_1"
            Begin Extent = 
               Top = 454
               Left = 44
               Bottom = 588
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Start"
            Begin Extent = 
               Top = 502
               Left = 272
               Bottom = 636
               Right = 456
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-RegularExpressions"
            Begin Extent = 
               Top = 592
               Left = 44
               Bottom = 726
               Righ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-PartsView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N't = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OCR-Invoices"
            Begin Extent = 
               Top = 730
               Left = 44
               Bottom = 885
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
      Begin ColumnWidths = 11
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
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 1846
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-PartsView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'OCR-PartsView'
GO
