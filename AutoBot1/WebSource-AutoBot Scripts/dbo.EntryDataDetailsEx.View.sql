USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryDataDetailsEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[EntryDataDetailsEx]
AS
SELECT   EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, EntryDataDetails.EntryDataId, EntryDataDetails.LineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity, EntryDataDetails.Units, 
                EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.QtyAllocated, EntryDataDetails.VolumeLiters, EntryDataDetails.UnitWeight, EntryDataDetails.DoNotAllocate, InventoryItems.TariffCode, 
                xcuda_Registration.Number AS CNumber, xcuda_Item.LineNumber AS CLineNumber, CAST(CASE WHEN Item_Id IS NULL THEN 1 ELSE 0 END AS bit) AS Downloaded, CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 
                0)) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, CAST(EntryDataDetails.Cost * EntryDataDetails.Quantity AS float) AS Total, ISNULL(AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 0) AS AsycudaDocumentSetId, 
                EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryDataDetails.Status, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.CNumber AS PreviousCNumber, EntryDataDetails.CLineNumber AS PreviousCLineNumber, 
                EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryDataDetails.IsReconciled, EntryData.ApplicationSettingsId, EntryDataDetails.LastCost, EntryDataDetails.FileLineNumber, ISNULL(EntryDataDetails.TaxAmount, 
                EntryData_Sales.Tax) AS TaxAmount, EntryData.EmailId, EntryData.FileTypeId, min(InventorySources.Name) as Name, EntryDataDetails.InventoryItemId
FROM      AsycudaDocumentSetEntryData AS AsycudaDocumentSetEntryData_1 WITH (NOLOCK) INNER JOIN
                AsycudaDocumentSetEntryData INNER JOIN
                EntryData WITH (NOLOCK) ON AsycudaDocumentSetEntryData.EntryData_Id = EntryData.EntryData_Id ON AsycudaDocumentSetEntryData_1.EntryData_Id = EntryData.EntryData_Id AND 
                AsycudaDocumentSetEntryData_1.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId RIGHT OUTER JOIN
                EntryDataDetails WITH (NOLOCK) ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id LEFT OUTER JOIN
                InventoryItemSource WITH (NOLOCK) INNER JOIN
                InventorySources WITH (NOLOCK) ON InventoryItemSource.InventorySourceId = InventorySources.Id RIGHT OUTER JOIN
                InventoryItems WITH (NOLOCK) ON InventoryItemSource.InventoryId = InventoryItems.Id ON EntryDataDetails.InventoryItemId = InventoryItems.Id LEFT OUTER JOIN
                xcuda_Item WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = xcuda_Item.EntryDataDetailsId LEFT OUTER JOIN
                xcuda_Registration WITH (NOLOCK) ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id LEFT OUTER JOIN
                EntryData_Sales WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id
GROUP BY EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryDataId, EntryDataDetails.LineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.Quantity, EntryDataDetails.Units, EntryDataDetails.ItemDescription, EntryDataDetails.Cost, 
                EntryDataDetails.QtyAllocated, EntryDataDetails.UnitWeight, InventoryItems.TariffCode, xcuda_Registration.Number, xcuda_Item.LineNumber, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryDataDetails.InvoiceQty, 
                EntryDataDetails.ReceivedQty, EntryDataDetails.Status, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.CNumber, EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryData.ApplicationSettingsId, 
                EntryDataDetails.LastCost, EntryDataDetails.TaxAmount, EntryData_Sales.Tax, EntryData.EmailId, EntryData.FileTypeId, EntryDataDetails.DoNotAllocate, xcuda_Item.Item_Id, EntryDataDetails.IsReconciled, 
                EntryDataDetails.EntryData_Id, EntryDataDetails.InventoryItemId, EntryDataDetails.FileLineNumber, EntryDataDetails.VolumeLiters, EntryDataDetails.CLineNumber
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[25] 2[17] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[50] 4[25] 3) )"
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
         Begin Table = "AsycudaDocumentSetEntryData_1"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 140
               Right = 299
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 0
               Left = 1513
               Bottom = 178
               Right = 1808
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 144
               Left = 44
               Bottom = 299
               Right = 282
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemSource"
            Begin Extent = 
               Top = 14
               Left = 37
               Bottom = 148
               Right = 258
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 22
               Left = 301
               Bottom = 275
               Right = 505
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventorySources"
            Begin Extent = 
               Top = 166
               Left = 6
               Bottom = 279
               Right = 206
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 132
               Left = 1034
               Bottom = 379
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataDetailsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'            Right = 1344
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 221
               Left = 1333
               Bottom = 423
               Right = 1584
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 300
               Left = 44
               Bottom = 455
               Right = 249
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 26
               Left = 529
               Bottom = 297
               Right = 747
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
      Begin ColumnWidths = 27
         Width = 284
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
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
         Column = 11677
         Alias = 2788
         Table = 2356
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1361
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1361
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataDetailsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataDetailsEx'
GO
