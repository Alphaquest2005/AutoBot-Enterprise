USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitOversToCustoms]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-SubmitOversToCustoms]
AS
SELECT dbo.AdjustmentOvers.EntryDataDetailsId, dbo.AdjustmentOvers.InvoiceDate, dbo.AdjustmentOvers.EntryDataId AS InvoiceNo, dbo.AdjustmentOvers.LineNumber AS InvoiceLineNumber, 
                 dbo.AdjustmentOvers.ItemNumber, dbo.AdjustmentOvers.Quantity, dbo.AdjustmentOvers.ItemDescription, dbo.AdjustmentOvers.Cost, dbo.AdjustmentOvers.InvoiceQty, dbo.AdjustmentOvers.ReceivedQty, 
                 dbo.AdjustmentOvers.PreviousInvoiceNumber, dbo.AdjustmentOvers.PreviousCNumber, dbo.AdjustmentOvers.Type, dbo.AdjustmentOvers.DutyFreePaid, dbo.AdjustmentOvers.Subject, 
                 dbo.AdjustmentOvers.EmailDate, dbo.AsycudaDocumentBasicInfo.CNumber, dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.Reference, dbo.xcuda_Item.LineNumber, 
                 dbo.AdjustmentOvers.ApplicationSettingsId
FROM    dbo.AsycudaDocumentBasicInfo INNER JOIN
                 dbo.AdjustmentOversAllocations ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.AdjustmentOversAllocations.Asycuda_Id RIGHT OUTER JOIN
                 dbo.AdjustmentOvers ON dbo.AdjustmentOversAllocations.EntryDataDetailsId = dbo.AdjustmentOvers.EntryDataDetailsId LEFT OUTER JOIN
                 dbo.SystemDocumentSets ON dbo.AdjustmentOvers.AsycudaDocumentSetId = dbo.SystemDocumentSets.Id LEFT OUTER JOIN
                 dbo.xcuda_Item ON dbo.AdjustmentOversAllocations.PreviousItem_Id = dbo.xcuda_Item.Item_Id
WHERE (dbo.SystemDocumentSets.Id IS NULL)
GROUP BY dbo.AdjustmentOvers.InvoiceDate, dbo.AdjustmentOvers.EntryDataId, dbo.AdjustmentOvers.LineNumber, dbo.AdjustmentOvers.ItemNumber, dbo.AdjustmentOvers.Quantity, dbo.AdjustmentOvers.ItemDescription, 
                 dbo.AdjustmentOvers.Cost, dbo.AdjustmentOvers.InvoiceQty, dbo.AdjustmentOvers.ReceivedQty, dbo.AdjustmentOvers.PreviousInvoiceNumber, dbo.AdjustmentOvers.PreviousCNumber, dbo.AdjustmentOvers.Type, 
                 dbo.AdjustmentOvers.DutyFreePaid, dbo.AdjustmentOvers.Subject, dbo.AdjustmentOvers.EmailDate, dbo.AsycudaDocumentBasicInfo.CNumber, dbo.AsycudaDocumentBasicInfo.RegistrationDate, 
                 dbo.AsycudaDocumentBasicInfo.Reference, dbo.xcuda_Item.LineNumber, dbo.AdjustmentOvers.ApplicationSettingsId, dbo.AdjustmentOvers.EntryDataDetailsId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[37] 4[17] 2[12] 3) )"
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
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 23
               Left = 1292
               Bottom = 167
               Right = 1566
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentOversAllocations"
            Begin Extent = 
               Top = 25
               Left = 738
               Bottom = 279
               Right = 1014
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentOvers"
            Begin Extent = 
               Top = 7
               Left = 271
               Bottom = 267
               Right = 580
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 183
               Left = 1290
               Bottom = 338
               Right = 1581
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 98
               Right = 228
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
      Begin ColumnWidths = 22
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
         Width = 1309' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitOversToCustoms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
         Width = 1309
         Width = 1309
         Width = 2644
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 2029
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitOversToCustoms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitOversToCustoms'
GO
