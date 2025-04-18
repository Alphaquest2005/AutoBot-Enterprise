USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[PreviousDocumentItem]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[PreviousDocumentItem]
AS
SELECT dbo.xcuda_Item.Amount_deducted_from_licence, dbo.xcuda_Item.Item_Id, dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_Item.Licence_number, dbo.xcuda_Item.Free_text_1, dbo.xcuda_Item.Free_text_2, 
                 dbo.xcuda_Item.EntryDataDetailsId, dbo.xcuda_Item.LineNumber, dbo.xcuda_Item.IsAssessed, dbo.xcuda_Item.DPQtyAllocated, dbo.xcuda_Item.DFQtyAllocated, dbo.xcuda_Item.EntryTimeStamp, 
                 dbo.xcuda_Item.AttributeOnlyAllocation, dbo.xcuda_Item.DoNotAllocate, dbo.xcuda_Item.DoNotEX, dbo.xcuda_Tarification.Item_price, dbo.xcuda_HScode.Precision_4 AS ItemNumber, 
                 dbo.xcuda_HScode.Commodity_code AS TariffCode, dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.xcuda_item_external_freight.Amount_national_currency AS Freight, 
                 dbo.xcuda_Valuation_item.Statistical_value, SUM(AscyudaItemPiQuantity.PiQuantity) AS PiQuantity, dbo.xcuda_Goods_description.Description_of_goods, dbo.xcuda_Goods_description.Commercial_Description, 
                 CAST(xcuda_Supplementary_unit.Suppplementary_unit_quantity AS float) AS ItemQuantity, xcuda_Supplementary_unit.Suppplementary_unit_code, dbo.xcuda_Registration.Number, 
                 dbo.xcuda_Type.Type_of_declaration + dbo.xcuda_Type.Declaration_gen_procedure_code AS DocumentType, dbo.xcuda_Registration.Number AS CNumber, dbo.AsycudaDocumentBasicInfo.AssessmentDate, 
                 dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    dbo.xcuda_Item WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Type WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_Type.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_Registration WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_Registration.ASYCUDA_Id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.xcuda_item_external_freight WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_item_external_freight.Valuation_item_Id LEFT OUTER JOIN
                 dbo.Primary_Supplementary_Unit AS xcuda_Supplementary_unit WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = xcuda_Supplementary_unit.Tarification_Id LEFT OUTER JOIN
                 dbo.AsycudaItemDutyLiablity WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.AsycudaItemDutyLiablity.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Goods_description WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                 dbo.[AscyudaItemPiQuantity-basic] as AscyudaItemPiQuantity WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Valuation_item WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_HScode WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_HScode.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Tarification WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE ((dbo.xcuda_Registration.Number IS NOT NULL) OR
                 (ISNULL(dbo.AsycudaDocumentBasicInfo.IsManuallyAssessed, 0) = 1)) and AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse'
GROUP BY dbo.xcuda_Item.Amount_deducted_from_licence, dbo.xcuda_Item.Item_Id, dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_Item.Licence_number, dbo.xcuda_Item.Free_text_1, dbo.xcuda_Item.Free_text_2, 
                 dbo.xcuda_Item.EntryDataDetailsId, dbo.xcuda_Item.LineNumber, dbo.xcuda_Item.DPQtyAllocated, dbo.xcuda_Item.DFQtyAllocated, dbo.xcuda_Item.EntryTimeStamp, dbo.xcuda_Tarification.Item_price, 
                 dbo.xcuda_HScode.Precision_4, dbo.xcuda_HScode.Commodity_code, dbo.xcuda_Item.AttributeOnlyAllocation, dbo.xcuda_Item.DoNotAllocate, dbo.xcuda_Item.DoNotEX, dbo.xcuda_Item.IsAssessed, 
                 dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.xcuda_item_external_freight.Amount_national_currency, dbo.xcuda_Valuation_item.Statistical_value, 
                 dbo.xcuda_Goods_description.Description_of_goods, dbo.xcuda_Goods_description.Commercial_Description, xcuda_Supplementary_unit.Suppplementary_unit_quantity, 
                 xcuda_Supplementary_unit.Suppplementary_unit_code, dbo.xcuda_Registration.Number, dbo.xcuda_Type.Type_of_declaration + dbo.xcuda_Type.Declaration_gen_procedure_code, 
                 dbo.AsycudaDocumentBasicInfo.AssessmentDate, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
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
         Configuration = "(H (1[53] 4[26] 3) )"
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
         Configuration = "(H (1[48] 4[26] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1[75] 4) )"
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
         Left = -220
      End
      Begin Tables = 
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 0
               Left = 606
               Bottom = 277
               Right = 965
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Type"
            Begin Extent = 
               Top = 388
               Left = 444
               Bottom = 566
               Right = 824
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 452
               Left = 930
               Bottom = 630
               Right = 1168
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 29
               Left = 1050
               Bottom = 264
               Right = 1340
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "xcuda_item_external_freight"
            Begin Extent = 
               Top = 393
               Left = 1234
               Bottom = 589
               Right = 1562
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_Supplementary_unit"
            Begin Extent = 
               Top = 58
               Left = 1248
               Bottom = 254
               Right = 1608
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 55
               Left = 1695
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PreviousDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         Bottom = 246
               Right = 1933
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 28
               Left = 1693
               Bottom = 224
               Right = 2004
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AscyudaItemPiQuantity"
            Begin Extent = 
               Top = 0
               Left = 1266
               Bottom = 142
               Right = 1504
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 209
               Left = 89
               Bottom = 463
               Right = 488
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 64
               Left = 14
               Bottom = 260
               Right = 259
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 0
               Left = 47
               Bottom = 196
               Right = 389
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 30
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
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 7344
         Alias = 2225
         Table = 2998
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PreviousDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PreviousDocumentItem'
GO
