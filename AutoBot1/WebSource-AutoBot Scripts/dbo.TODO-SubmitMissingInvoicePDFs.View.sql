USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitMissingInvoicePDFs]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-SubmitMissingInvoicePDFs]
AS
SELECT EntryDataEx.EntryDataId as InvoiceNo, EntryDataEx.SourceFile, dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.AsycudaDocumentSet.ApplicationSettingsId, dbo.AsycudaDocumentSet.Declarant_Reference_Number, 
                 EntryDataEx.EmailId
FROM   EntryData as EntryDataEx INNER JOIN
                 dbo.AsycudaDocumentSetEntryData ON EntryDataEx.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
				  AsycudaDocumentSet on dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId inner join
                 dbo.[TODO-PODocSet-Criteria] ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.[TODO-PODocSet-Criteria].AsycudaDocumentSetId LEFT OUTER JOIN
                 dbo.[TODO-PODocSetAttachements-EntryData] ON dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId = dbo.[TODO-PODocSetAttachements-EntryData].AsycudaDocumentSetId AND 
                 dbo.AsycudaDocumentSetEntryData.EntryData_Id = dbo.[TODO-PODocSetAttachements-EntryData].EntryData_Id
WHERE (dbo.[TODO-PODocSetAttachements-EntryData].EntryData_Id IS NULL)
GROUP BY EntryDataex.EntryDataId, EntryDataEx.SourceFile, dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.AsycudaDocumentSet.ApplicationSettingsId, dbo.AsycudaDocumentSet.Declarant_Reference_Number, 
                 EntryDataEx.EmailId
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
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 299
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 343
               Bottom = 140
               Right = 598
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-PODocSet"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 317
               Right = 354
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-PODocSetAttachements"
            Begin Extent = 
               Top = 318
               Left = 44
               Bottom = 473
               Right = 299
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
     ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitMissingInvoicePDFs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitMissingInvoicePDFs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitMissingInvoicePDFs'
GO
