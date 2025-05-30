USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryDataEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO













CREATE VIEW [dbo].[EntryDataEx]
AS

SELECT EntryData.EntryData_Id, EntryData.EntryDataDate AS InvoiceDate, EntryData.EntryType AS Type, [EntryData-Summary].Total AS ImportedTotal, EntryData.EntryDataId AS InvoiceNo, 
                 EntryData_PurchaseOrders.SupplierInvoiceNo, COALESCE ([EntryData-Summary].DetailsTax, EntryData_Adjustments.Tax) AS Tax, EntryData.InvoiceTotal, EntryData.ImportedLines, Cast(isnull([EntryData-Summary].TotalLines,0) as int) as TotalLines, 
                 EntryData.Currency, EntryData.ApplicationSettingsId, CASE WHEN isnull(DetailsTax, COALESCE (EntryData_Sales.Tax, EntryData_Adjustments.Tax)) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, 
                 EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode, ISNULL([EntryData-Summary].Total, 0) + ISNULL(EntryData.TotalInternalFreight, 0) + ISNULL(EntryData.TotalInsurance, 0) 
                 + ISNULL(EntryData.TotalOtherCost, 0) - ISNULL(EntryData.TotalDeduction, 0) + ISNULL(EntryData.TotalFreight, 0) AS ExpectedTotal, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                 ISNULL(EntryData.TotalInternalFreight, 0) AS TotalInternalFreight, ISNULL(EntryData.TotalInsurance, 0) AS TotalInternalInsurance, ISNULL(EntryData.TotalOtherCost, 0) AS TotalOtherCost, 
                 ISNULL(EntryData.TotalDeduction, 0) AS TotalDeductions, ISNULL(EntryData.TotalFreight, 0) AS TotalFreight, ISNULL([EntryData-Summary].Total, 0) AS Totals, EntryData.SourceFile
FROM    EntryData WITH (NOLOCK) LEFT OUTER JOIN
                 [EntryData-Summary] ON EntryData.EntryData_Id = [EntryData-Summary].EntryData_Id INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 EntryData_Adjustments WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 EntryData_Sales WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id LEFT OUTER JOIN
                 EntryData_OpeningStock WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_OpeningStock.EntryData_Id LEFT OUTER JOIN
                 EntryData_PurchaseOrders WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id
--GROUP BY EntryData.EntryData_Id, EntryData.EntryDataDate, [EntryData-Summary].Total, EntryData.EntryDataId, EntryData.InvoiceTotal, EntryData.ImportedLines, [EntryData-Summary].TotalLines, EntryData.Currency, 
--                 EntryData.ApplicationSettingsId, EntryData.EmailId, EntryData.FileTypeId, EntryData.SupplierCode, EntryData.SourceFile, EntryData_PurchaseOrders.PONumber, EntryData_Sales.EntryData_Id, 
--                 EntryData_OpeningStock.EntryData_Id, EntryData_Adjustments.EntryData_Id, EntryData_Adjustments.Type, EntryData_Sales.Tax, EntryData_Adjustments.Tax, EntryData_PurchaseOrders.SupplierInvoiceNo, 
--                 EntryData.TotalInternalFreight, EntryData.TotalInsurance, EntryData.TotalOtherCost, EntryData.TotalDeduction, EntryData.TotalFreight, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
--                 COALESCE ([EntryData-Summary].DetailsTax, EntryData_Adjustments.Tax), ISNULL([EntryData-Summary].Total, 0) + ISNULL(EntryData.TotalInternalFreight, 0) + ISNULL(EntryData.TotalInsurance, 0) 
--                 + ISNULL(EntryData.TotalOtherCost, 0) - ISNULL(EntryData.TotalDeduction, 0) + ISNULL(EntryData.TotalFreight, 0), EntryData.EntryType, [EntryData-Summary].DetailsTax

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[35] 4[26] 2[7] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[38] 4[19] 3) )"
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
         Configuration = "(H (1[57] 4) )"
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
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 11
               Left = 592
               Bottom = 195
               Right = 887
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 1506
               Bottom = 140
               Right = 1745
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 143
               Left = 1283
               Bottom = 257
               Right = 1596
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 134
               Left = 752
               Bottom = 353
               Right = 980
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_OpeningStock"
            Begin Extent = 
               Top = 106
               Left = 375
               Bottom = 248
               Right = 597
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_PurchaseOrders"
            Begin Extent = 
               Top = 0
               Left = 985
               Bottom = 153
               Right = 1252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataExTotals"
            Begin Extent = 
               Top = 26
               Left = 19
               B' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'ottom = 235
               Right = 241
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
      Begin ColumnWidths = 15
         Width = 284
         Width = 3129
         Width = 995
         Width = 2304
         Width = 1126
         Width = 2225
         Width = 1977
         Width = 995
         Width = 995
         Width = 995
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 21443
         Alias = 2186
         Table = 2697
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataEx'
GO
