USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemCost]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentItemCost]
AS
SELECT dbo.xcuda_HScode.Item_Id, dbo.Primary_Supplementary_Unit.Suppplementary_unit_code, dbo.xcuda_HScode.Commodity_code AS TariffCode, 
                 dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS ItemQuantity, dbo.xcuda_HScode.Precision_4 AS ItemNumber, CASE WHEN currency_code = 'XCD' AND 
                 amount_national_currency = 0 THEN amount_foreign_currency ELSE dbo.xcuda_Item_Invoice.Amount_national_currency END / dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS LocalItemCost, 
                 dbo.xcuda_Item_Invoice.Amount_national_currency AS LocalTotalCost, dbo.xcuda_Item_Invoice.Amount_foreign_currency AS ForexTotalCost, 
                 dbo.xcuda_Item_Invoice.Amount_foreign_currency / dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS ForexItemCost, dbo.xcuda_Item_Invoice.Currency_code, dbo.xcuda_Item_Invoice.Currency_rate, 
                 dbo.xcuda_ItemApplicationSettings.ApplicationSettingsId, dbo.AsycudaDocumentBasicInfo.AssessmentDate
FROM    dbo.xcuda_HScode WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item_Invoice WITH (NOLOCK) ON dbo.xcuda_HScode.Item_Id = dbo.xcuda_Item_Invoice.Valuation_item_Id INNER JOIN
                 dbo.Primary_Supplementary_Unit WITH (NOLOCK) ON dbo.xcuda_Item_Invoice.Valuation_item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id INNER JOIN
                 dbo.xcuda_ItemApplicationSettings WITH (NOLOCK) ON dbo.xcuda_HScode.Item_Id = dbo.xcuda_ItemApplicationSettings.Item_Id INNER JOIN
                 dbo.xcuda_Item ON dbo.xcuda_HScode.Item_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity <> 0)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[35] 4[21] 2[17] 3) )"
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
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 40
               Left = 906
               Bottom = 245
               Right = 1151
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item_Invoice"
            Begin Extent = 
               Top = 13
               Left = 371
               Bottom = 236
               Right = 651
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 15
               Left = 18
               Bottom = 217
               Right = 269
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ItemApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 648
               Bottom = 119
               Right = 870
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 161
               Left = 578
               Bottom = 316
               Right = 869
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 161
               Left = 122
               Bottom = 316
               Right = 396
            End
            DisplayFlags = 280
            TopColumn = 5
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemCost'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'= 9
         Width = 284
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 12358
         Alias = 2553
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemCost'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemCost'
GO
