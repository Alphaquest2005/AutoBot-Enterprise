USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaItemCostingBase]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaItemCostingBase] AS
WITH OriginalQuery AS (
    SELECT      
        dbo.AsycudaItemBasicInfo.CNumber,  
        dbo.AsycudaItemBasicInfo.Item_Id AS BaseItem_Id,
        dbo.AsycudaItemBasicInfo.LineNumber,  
        dbo.AsycudaItemBasicInfo.ItemNumber, 
        dbo.AsycudaItemBasicInfo.Commercial_Description, 
        dbo.AsycudaItemBasicInfo.ItemQuantity,
        dbo.xcuda_Valuation_item.Total_CIF_itm,
        dbo.xcuda_Item_Invoice.Amount_national_currency,
        ISNULL(dbo.xcuda_item_external_freight.Amount_national_currency, 0) AS External_Freight, 
        ISNULL(dbo.xcuda_item_insurance.Amount_national_currency, 0) AS Insurance, 
        ISNULL(dbo.xcuda_item_internal_freight.Amount_national_currency, 0) AS Internal_Freight, 
        ISNULL(dbo.xcuda_item_other_cost.Amount_national_currency,0) AS Other_Cost, 
        ISNULL(dbo.xcuda_item_deduction.Amount_national_currency,0) AS Deduction
    FROM            
        dbo.xcuda_item_insurance RIGHT OUTER JOIN
        dbo.xcuda_item_internal_freight RIGHT OUTER JOIN
        dbo.xcuda_item_other_cost RIGHT OUTER JOIN
        dbo.xcuda_Item_Invoice INNER JOIN
        dbo.AsycudaItemBasicInfo INNER JOIN
        dbo.xcuda_Valuation_item ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.xcuda_Valuation_item.Item_Id ON 
        dbo.xcuda_Item_Invoice.Valuation_item_Id = dbo.AsycudaItemBasicInfo.Item_Id ON 
        dbo.xcuda_item_other_cost.Valuation_item_Id = dbo.AsycudaItemBasicInfo.Item_Id ON 
        dbo.xcuda_item_internal_freight.Valuation_item_Id = dbo.AsycudaItemBasicInfo.Item_Id LEFT OUTER JOIN
        dbo.xcuda_item_deduction ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.xcuda_item_deduction.Valuation_item_Id ON 
        dbo.xcuda_item_insurance.Valuation_item_Id = dbo.AsycudaItemBasicInfo.Item_Id LEFT OUTER JOIN
        dbo.xcuda_item_external_freight ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.xcuda_item_external_freight.Valuation_item_Id
)
SELECT 
    o.CNumber,
    o.BaseItem_Id,
    o.LineNumber,
    o.ItemNumber, 
    o.Commercial_Description, 
    o.ItemQuantity,
    '' as [Received Qty], 
    o.Total_CIF_itm,
    '' as [Rate Factor], 
    o.Amount_national_currency AS [Invoice Value], 
    ISNULL(o.External_Freight, 0) AS [External Freight], 
    ISNULL(o.Insurance, 0) AS Insurance, 
    ISNULL(o.Internal_Freight, 0) AS [Internal Freight], 
    ISNULL(o.Other_Cost, 0) AS [Other Cost], 
    ISNULL(o.Deduction, 0) AS Deduction,
    pvt.*
FROM OriginalQuery o
LEFT JOIN (
    SELECT Item_Id, Duty_tax_code, isnull(Amount,0) as Amount 
    FROM [AutoBrokerage-AutoBot].[dbo].[AsycudaItemTaxes]
) t 
PIVOT (
    MAX(Amount)
    FOR Duty_tax_code IN (
        [CET], [EVL], [CSC], [VAT], [EXT] -- Add your actual tax codes here
    )
) AS pvt ON o.BaseItem_Id = pvt.Item_Id

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[14] 3) )"
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
         Begin Table = "AsycudaItemBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 223
               Right = 257
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_item_deduction"
            Begin Extent = 
               Top = 203
               Left = 795
               Bottom = 333
               Right = 1027
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_item_external_freight"
            Begin Extent = 
               Top = 9
               Left = 611
               Bottom = 139
               Right = 843
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_item_insurance"
            Begin Extent = 
               Top = 14
               Left = 880
               Bottom = 185
               Right = 1112
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_item_internal_freight"
            Begin Extent = 
               Top = 6
               Left = 1105
               Bottom = 241
               Right = 1337
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_item_other_cost"
            Begin Extent = 
               Top = 6
               Left = 1375
               Bottom = 217
               Right = 1607
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 70
               Left = 17' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemCostingBase'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'64
               Bottom = 271
               Right = 2042
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item_Invoice"
            Begin Extent = 
               Top = 103
               Left = 332
               Bottom = 329
               Right = 564
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
      Begin ColumnWidths = 14
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1695
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 7275
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemCostingBase'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemCostingBase'
GO
