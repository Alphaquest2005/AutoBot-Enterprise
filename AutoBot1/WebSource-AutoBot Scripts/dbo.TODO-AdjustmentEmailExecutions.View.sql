USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentEmailExecutions]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-AdjustmentEmailExecutions]
AS
SELECT TOP (100) PERCENT dbo.Emails.EmailDate, dbo.Emails.Subject, dbo.EntryData_Adjustments.Type, dbo.EntryData.EntryDataId AS InvoiceNo, dbo.EntryDataDetails.LineNumber, dbo.EntryDataDetails.ItemNumber, 
                 dbo.EntryDataDetails.ItemDescription, dbo.EntryDataDetails.Quantity, dbo.EntryDataDetails.Cost, dbo.EntryDataDetails.InvoiceQty, dbo.EntryDataDetails.ReceivedQty, dbo.EntryDataDetails.PreviousInvoiceNumber, 
                 dbo.EntryDataDetails.CNumber, dbo.EntryDataDetails.Comment, dbo.EntryDataDetails.EffectiveDate, dbo.AsycudaDocument.ReferenceNumber AS xReferenceNumber, 
                 dbo.AsycudaDocument.RegistrationDate AS xRegistrationDate, dbo.AsycudaDocument.AssessmentDate AS xAssessmentDate, dbo.AsycudaDocument.CNumber AS xCNumber, 
                 dbo.xcuda_Item.LineNumber AS xLineNumber, dbo.AsycudaDocumentItemEntryDataDetails.ItemNumber AS xItemNumber, dbo.AsycudaDocumentItemEntryDataDetails.Quantity AS xQuantity, 
                 dbo.AsycudaDocument.DocumentType AS xDocumentType, dbo.AsycudaDocument.CustomsProcedure AS xCustomProcedure, dbo.EntryData.ApplicationSettingsId
FROM    dbo.AsycudaDocument INNER JOIN
                 dbo.AsycudaDocumentItemEntryDataDetails ON dbo.AsycudaDocument.ASYCUDA_Id = dbo.AsycudaDocumentItemEntryDataDetails.Asycuda_id INNER JOIN
                 dbo.xcuda_Item ON dbo.AsycudaDocumentItemEntryDataDetails.Item_Id = dbo.xcuda_Item.Item_Id RIGHT OUTER JOIN
                 dbo.EntryData_Adjustments INNER JOIN
                 dbo.EntryData ON dbo.EntryData_Adjustments.EntryData_Id = dbo.EntryData.EntryData_Id INNER JOIN
                 dbo.Emails ON dbo.EntryData.EmailId = dbo.Emails.EmailId LEFT OUTER JOIN
                 dbo.EntryDataDetails ON dbo.EntryData.EntryData_Id = dbo.EntryDataDetails.EntryData_Id ON dbo.AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = dbo.EntryDataDetails.EntryDataDetailsId
--WHERE (dbo.AsycudaDocumentItemEntryDataDetails.ImportComplete = 1)
ORDER BY dbo.Emails.EmailDate DESC
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[45] 4[17] 2[21] 3) )"
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
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 183
               Left = 93
               Bottom = 338
               Right = 387
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 181
               Left = 501
               Bottom = 336
               Right = 740
            End
            DisplayFlags = 280
            TopColumn = 8
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 6
               Left = 665
               Bottom = 140
               Right = 849
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 0
               Left = 627
               Bottom = 155
               Right = 849
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Emails"
            Begin Extent = 
               Top = 5
               Left = 383
               Bottom = 139
               Right = 567
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 1
               Left = 971
               Bottom = 156
               Right = 1209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 233
               Left = 995
               Bottom = 388
              ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-AdjustmentEmailExecutions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Right = 1286
            End
            DisplayFlags = 280
            TopColumn = 9
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-AdjustmentEmailExecutions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-AdjustmentEmailExecutions'
GO
