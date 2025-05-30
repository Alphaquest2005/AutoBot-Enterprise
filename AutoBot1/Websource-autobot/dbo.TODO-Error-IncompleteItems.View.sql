USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-IncompleteItems]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-Error-IncompleteItems]
AS
SELECT dbo.xcuda_Item.Item_Id, COALESCE (CASE WHEN dbo.xcuda_Taxation.Item_Id IS NOT NULL THEN NULL ELSE 'Taxation Missing' END, CASE WHEN dbo.xcuda_Goods_description.Item_Id IS NOT NULL THEN NULL 
                 ELSE 'Goods Description Missing' END, CASE WHEN dbo.xcuda_Valuation_item.Item_Id IS NOT NULL THEN NULL ELSE 'Valuation Missing' END) AS Error, dbo.AsycudaDocumentBasicInfo.CNumber, 
                 dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.xcuda_Item.LineNumber, dbo.xcuda_Item.ASYCUDA_Id, 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.SourceFileName, dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
FROM    dbo.AsycudaDocumentBasicInfo INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id RIGHT OUTER JOIN
                 dbo.xcuda_Item ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.xcuda_HScode INNER JOIN
                 dbo.xcuda_Tarification ON dbo.xcuda_HScode.Item_Id = dbo.xcuda_Tarification.Item_Id INNER JOIN
                 dbo.xcuda_Supplementary_unit ON dbo.xcuda_Tarification.Item_Id = dbo.xcuda_Supplementary_unit.Tarification_Id ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Item_Invoice INNER JOIN
                 dbo.xcuda_Valuation_item ON dbo.xcuda_Item_Invoice.Valuation_item_Id = dbo.xcuda_Valuation_item.Item_Id INNER JOIN
                 dbo.xcuda_Weight_itm ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Weight_itm.Valuation_item_Id ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_PreviousItem ON dbo.xcuda_Item.Item_Id = dbo.xcuda_PreviousItem.PreviousItem_Id LEFT OUTER JOIN
                 dbo.xcuda_Goods_description ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Taxation ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Taxation.Item_Id
WHERE (dbo.xcuda_Taxation.Item_Id IS NULL) AND (dbo.AsycudaDocumentBasicInfo.ImportComplete = 1) OR
                 (dbo.xcuda_Goods_description.Item_Id IS NULL) AND (dbo.AsycudaDocumentBasicInfo.ImportComplete = 1) OR
                 (dbo.xcuda_Valuation_item.Item_Id IS NULL) AND (dbo.AsycudaDocumentBasicInfo.ImportComplete = 1)
GROUP BY dbo.xcuda_Item.Item_Id, dbo.AsycudaDocumentBasicInfo.CNumber, dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.xcuda_Item.LineNumber, 
                 COALESCE (CASE WHEN dbo.xcuda_Taxation.Item_Id IS NOT NULL THEN NULL ELSE 'Taxation Missing' END, CASE WHEN dbo.xcuda_Goods_description.Item_Id IS NOT NULL THEN NULL 
                 ELSE 'Goods Description Missing' END, CASE WHEN dbo.xcuda_Valuation_item.Item_Id IS NOT NULL THEN NULL ELSE 'Valuation Missing' END), dbo.xcuda_Item.ASYCUDA_Id, 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.SourceFileName, dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[54] 4[13] 2[12] 3) )"
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
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 194
               Left = 413
               Bottom = 349
               Right = 704
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 125
               Left = 1488
               Bottom = 280
               Right = 1688
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 8
               Left = 952
               Bottom = 163
               Right = 1226
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Supplementary_unit"
            Begin Extent = 
               Top = 0
               Left = 1410
               Bottom = 155
               Right = 1690
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item_Invoice"
            Begin Extent = 
               Top = 390
               Left = 1413
               Bottom = 545
               Right = 1669
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 248
               Left = 874
               Bottom = 403
               Right = 1181
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_Weight_itm"
            Begin Extent = 
               Top = 272
               Left = 1275
               Bottom = ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-IncompleteItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'406
               Right = 1477
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_PreviousItem"
            Begin Extent = 
               Top = 367
               Left = 476
               Bottom = 522
               Right = 774
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 238
               Left = 0
               Bottom = 393
               Right = 240
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Taxation"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 373
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 2
               Left = 456
               Bottom = 157
               Right = 730
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 51
               Left = 822
               Bottom = 206
               Right = 1067
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
      Begin ColumnWidths = 11
         Width = 284
         Width = 1309
         Width = 3521
         Width = 1309
         Width = 3914
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 7580
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 8404
         Alias = 903
         Table = 2854
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1348
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-IncompleteItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-IncompleteItems'
GO
