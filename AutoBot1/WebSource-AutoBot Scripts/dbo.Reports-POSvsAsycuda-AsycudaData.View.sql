USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-AsycudaData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Reports-POSvsAsycuda-AsycudaData]
AS

SELECT   'C#' + AsycudaDocumentBasicInfo.CNumber + '-' + CAST(xcuda_Item.LineNumber AS varchar(50)) AS PrevDoc, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description AS Description, 
                SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(ISNULL(xcuda_Item.DFQtyAllocated, 0) + ISNULL(xcuda_Item.DPQtyAllocated, 0)) AS QtyAllocated, SUM(Primary_Supplementary_Unit.Suppplementary_unit_quantity) 
                AS Quantity, SUM(CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, AVG(CAST(AsycudaDocumentItemCost.LocalItemCost AS float)) AS Cost, 
                ISNULL(InventoryItemAlias.ItemNumber, xcuda_HScode.Precision_4) AS ItemNumber, ISNULL(InventoryItemAlias.InventoryItemId, xcuda_Inventory_Item.InventoryItemId) AS InventoryItemId, AsycudaDocumentBasicInfo.AssessmentDate, 
                AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.Reference
--into #AsycudaData
FROM      ApplicationSettings WITH (nolock) CROSS JOIN
                xcuda_HScode WITH (nolock) LEFT OUTER JOIN
                InventoryItemAliasEx AS InventoryItemAlias WITH (nolock) ON xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName INNER JOIN
                xcuda_Inventory_Item ON xcuda_HScode.Item_Id = xcuda_Inventory_Item.Item_Id RIGHT OUTER JOIN
                xcuda_Item WITH (nolock) ON xcuda_HScode.Item_Id = xcuda_Item.Item_Id LEFT OUTER JOIN
                AsycudaDocumentItemCost WITH (nolock) ON xcuda_Item.Item_Id = AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                    (SELECT   Item_Id, SUM(PiQuantity) AS PiQuantity, SUM(PiWeight) AS PiWeight
                     FROM      [AsycudaItemPiQuantityData-Basic] WITH (nolock) -------------This is for overs to generate correct amount
					/* FROM      [AsycudaItemPiQuantityData-BasicWithRegDate] WITH (nolock)           --------Must remove for the Shorts so that it can execute
					 Where RegistrationDate <= @asycudaEndDate*/  
					 GROUP BY Item_Id) AS AscyudaItemPiQuantity ON xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                xcuda_Goods_description WITH (nolock) ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                Primary_Supplementary_Unit WITH (nolock) ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id LEFT OUTER JOIN
                AsycudaDocumentBasicInfo WITH (nolock) ON xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
				inner join [Reports-POSvsAsycudaData] on AsycudaDocumentBasicInfo.ApplicationSettingsId = [Reports-POSvsAsycudaData].ApplicationSettingsId	 

WHERE   (AsycudaDocumentBasicInfo.RegistrationDate <=  [Reports-POSvsAsycudaData].EndDate  or  Reference like 'Audit%') AND (xcuda_Item.WarehouseError IS NULL) AND (isnull(AsycudaDocumentBasicInfo.Cancelled, 0) <> 1) AND (isnull(AsycudaDocumentBasicInfo.DoNotAllocate,0) <> 1) 
		and ((AsycudaDocumentBasicInfo.DocumentType = 'IM7' OR AsycudaDocumentBasicInfo.DocumentType = 'OS7'))
			   AND  (InventoryItemAlias.AliasId is null and xcuda_HScode.Precision_4 = xcuda_HScode.Precision_4)
GROUP BY AsycudaDocumentBasicInfo.DocumentType, xcuda_HScode.Precision_4, xcuda_Goods_description.Commercial_Description, AsycudaDocumentBasicInfo.CNumber, xcuda_Item.LineNumber, 
                         InventoryItemAlias.AliasName, AsycudaDocumentBasicInfo.AssessmentDate,  AsycudaDocumentBasicInfo.RegistrationDate,InventoryItemAlias.ItemNumber,
						 AsycudaDocumentBasicInfo.Reference, xcuda_Inventory_Item.InventoryItemId,InventoryItemAlias.InventoryItemId
--order by AsycudaDocumentBasicInfo.AssessmentDate asc
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
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 238
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias"
            Begin Extent = 
               Top = 6
               Left = 276
               Bottom = 119
               Right = 462
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 500
               Bottom = 136
               Right = 828
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentRegistrationDate"
            Begin Extent = 
               Top = 6
               Left = 866
               Bottom = 136
               Right = 1058
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 1096
               Bottom = 136
               Right = 1375
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemCost"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 287
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AscyudaItemPiQuantity"
            Begin Extent = 
               Top = 138
               Left = 325
               Bottom ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-AsycudaData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'= 251
               Right = 511
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 138
               Left = 549
               Bottom = 268
               Right = 784
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 138
               Left = 822
               Bottom = 268
               Right = 1089
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 138
               Left = 1127
               Bottom = 268
               Right = 1389
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Reports-POSvsAsycudaData"
            Begin Extent = 
               Top = 270
               Left = 472
               Bottom = 383
               Right = 658
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
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 2745
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-AsycudaData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-AsycudaData'
GO
