USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentOvers]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[AdjustmentOvers]
AS
SELECT EntryDataDetailsId,EntryData_Id, EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, QtyAllocated, UnitWeight, DoNotAllocate, TariffCode, CNumber, CLineNumber, AsycudaDocumentSetId, InvoiceQty, 
                 ReceivedQty, PreviousInvoiceNumber, PreviousCNumber, Comment, Status, EffectiveDate,IsReconciled, Currency, ApplicationSettingsId, Type, DutyFreePaid, EmailId, FileTypeId, InvoiceDate, Subject, EmailDate,
				 AdjustmentDetails.InventoryItemId
FROM    dbo.AdjustmentDetails WITH (NOLOCK)
where isnull(AdjustmentDetails.DoNotAllocate,0) <> 1
GROUP BY EntryDataDetailsId, EntryDataId, LineNumber, Units, ItemDescription, QtyAllocated, UnitWeight, TariffCode, CNumber, CLineNumber, AsycudaDocumentSetId, Status, PreviousInvoiceNumber, PreviousCNumber, 
                 Comment, EffectiveDate, Cost, Currency, ItemNumber, ReceivedQty, InvoiceQty, Quantity, DoNotAllocate, ApplicationSettingsId, Type, DutyFreePaid, EmailId, FileTypeId, InvoiceDate,IsReconciled, Subject, EmailDate,EntryData_Id,
				 InventoryItemId
HAVING (Quantity > 0)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[15] 2[25] 3) )"
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
         Begin Table = "AdjustmentDetails"
            Begin Extent = 
               Top = 0
               Left = 211
               Bottom = 258
               Right = 450
            End
            DisplayFlags = 280
            TopColumn = 19
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 27
         Width = 284
         Width = 1309
         Width = 1741
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
         Width = 1309
         Width = 1309
         Width = 2160
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 7828
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 4673
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentOvers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentOvers'
GO
