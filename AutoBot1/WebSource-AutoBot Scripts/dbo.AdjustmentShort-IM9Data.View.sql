USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShort-IM9Data]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AdjustmentShort-IM9Data]
AS
SELECT AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.EntryDataDetailsId, xcuda_Goods_description.Commercial_Description AS pItemDescription, (CASE WHEN isnull(EntryDataDetails.TaxAmount, 
                 isnull(EntryData_Adjustments.Tax, 0)) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END) AS DutyFreePaid, EntryDataDetails.EffectiveDate, EntryData.EntryDataDate AS InvoiceDate, 
                 EntryDataDetails.EntryDataId AS InvoiceNo, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, AsycudaDocumentBasicInfo.CNumber AS pCNumber, xcuda_Item.LineNumber AS pLineNumber, 
                 xcuda_HScode.Precision_4 AS pItemNumber, AsycudaDocumentItemCost.LocalItemCost AS pItemCost, AsycudaSalesAllocations.Status, AsycudaSalesAllocations.PreviousItem_Id, xcuda_Item.SalesFactor, 
                 AsycudaSalesAllocations.QtyAllocated, EntryDataDetails.QtyAllocated AS SalesQtyAllocated, EntryDataDetails.Quantity AS SalesQuantity, xcuda_HScode.Precision_1 AS pPrecision1, xcuda_Item.DFQtyAllocated, 
                 xcuda_Item.DPQtyAllocated, xcuda_HScode.Commodity_code AS pTariffCode, EntryDataDetails.LineNumber, EntryDataDetails.Comment, xcuda_Border_office.Code AS Customs_clearance_office_code, 
                 Primary_Supplementary_Unit.Suppplementary_unit_quantity AS pQuantity, ISNULL(AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate) AS pRegistrationDate, 
                 AsycudaDocumentBasicInfo.AssessmentDate AS pAssessmentDate, AsycudaDocumentBasicInfo.ExpiryDate, xcuda_Goods_description.Country_of_origin_code, EntryData.FileTypeId, EntryData.EmailId, 
                 EntryDataDetails.InventoryItemId, ISNULL(AsycudaSalesAllocations.xEntryItem_Id, 0) AS xBond_Item_Id, EntryData.ApplicationSettingsId, xcuda_Weight_itm.Net_weight_itm, SourceFile
FROM    AsycudaSalesAllocations INNER JOIN
                 EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                 xcuda_Goods_description ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Goods_description.Item_Id INNER JOIN
                 EntryData_Adjustments ON EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id INNER JOIN
                 EntryData ON EntryData_Adjustments.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 xcuda_Item ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Item.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 xcuda_HScode ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_HScode.Item_Id INNER JOIN
                 Primary_Supplementary_Unit ON AsycudaSalesAllocations.PreviousItem_Id = Primary_Supplementary_Unit.Tarification_Id INNER JOIN
                 xcuda_Weight_itm ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Weight_itm.Valuation_item_Id LEFT OUTER JOIN
                 xcuda_Border_office ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Border_office.Border_office_Id LEFT OUTER JOIN
                 AsycudaDocumentItemCost ON AsycudaSalesAllocations.PreviousItem_Id = AsycudaDocumentItemCost.Item_Id
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
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 251
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 295
               Bottom = 161
               Right = 533
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 6
               Left = 577
               Bottom = 161
               Right = 817
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 6
               Left = 861
               Bottom = 161
               Right = 1045
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 1089
               Bottom = 161
               Right = 1311
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 1355
               Bottom = 161
               Right = 1646
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 1690
               Bottom = 161
 ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShort-IM9Data'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'              Right = 1964
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 6
               Left = 2008
               Bottom = 161
               Right = 2208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 6
               Left = 2252
               Bottom = 161
               Right = 2532
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Border_office"
            Begin Extent = 
               Top = 6
               Left = 2576
               Bottom = 161
               Right = 2766
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemCost"
            Begin Extent = 
               Top = 6
               Left = 2810
               Bottom = 161
               Right = 3068
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
      Begin ColumnWidths = 11
         Column = 2671
         Alias = 2252
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShort-IM9Data'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShort-IM9Data'
GO
