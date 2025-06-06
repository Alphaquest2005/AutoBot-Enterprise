USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitDocSetWithIncompleteInvoices]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-SubmitDocSetWithIncompleteInvoices]
AS
SELECT TOP (100) PERCENT dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId, dbo.AsycudaDocumentSetEx.Declarant_Reference_Number, dbo.AsycudaDocumentSetEx.TotalInvoices, 
                 dbo.AsycudaDocumentSetEx.ImportedInvoices, dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, dbo.EntryDataEx.InvoiceTotal, dbo.AsycudaDocumentSetEx.ApplicationSettingsId, 
                 dbo.EntryDataEx.EmailId
FROM    dbo.AsycudaDocumentSetEx INNER JOIN
                 dbo.AsycudaDocumentSetEntryData ON dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId INNER JOIN
                 dbo.EntryDataEx ON dbo.AsycudaDocumentSetEntryData.EntryData_Id = dbo.EntryDataEx.EntryData_Id AND dbo.AsycudaDocumentSetEx.ApplicationSettingsId = dbo.EntryDataEx.ApplicationSettingsId AND 
                 dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId = dbo.EntryDataEx.AsycudaDocumentSetId LEFT OUTER JOIN
                 dbo.SystemDocumentSets ON dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId = dbo.SystemDocumentSets.Id
WHERE (dbo.AsycudaDocumentSetEx.TotalInvoices <> dbo.AsycudaDocumentSetEx.ImportedInvoices) AND (dbo.SystemDocumentSets.Id IS NULL)
ORDER BY dbo.EntryDataEx.InvoiceNo
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[27] 4[23] 2[13] 3) )"
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
         Begin Table = "AsycudaDocumentSetEx"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 305
               Right = 316
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 4
               Left = 455
               Bottom = 138
               Right = 694
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 45
               Left = 967
               Bottom = 330
               Right = 1524
            End
            DisplayFlags = 280
            TopColumn = 8
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 204
               Left = 773
               Bottom = 296
               Right = 957
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
      Begin ColumnWidths = 10
         Width = 284
         Width = 1309
         Width = 3823
         Width = 1309
         Width = 2710
         Width = 2657
         Width = 1309
         Width = 1951
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitDocSetWithIncompleteInvoices'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitDocSetWithIncompleteInvoices'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitDocSetWithIncompleteInvoices'
GO
