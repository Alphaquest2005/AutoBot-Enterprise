USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[DataProcessCheck-Unknown Asycuda Items]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DataProcessCheck-Unknown Asycuda Items]
AS
SELECT DISTINCT 
                         dbo.xcuda_HScode.Precision_4, dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.Description_of_goods, dbo.AsycudaDocumentItem.Item_price, 
                         dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocument.DocumentType
FROM            dbo.AsycudaDocumentItem INNER JOIN
                         dbo.xcuda_HScode ON dbo.AsycudaDocumentItem.ItemNumber = dbo.xcuda_HScode.Precision_4 INNER JOIN
                         dbo.AsycudaDocument ON dbo.AsycudaDocumentItem.CNumber = dbo.AsycudaDocument.CNumber LEFT OUTER JOIN
                         dbo.InventoryItems ON dbo.xcuda_HScode.Precision_4 = dbo.InventoryItems.ItemNumber 
						 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
WHERE        (dbo.InventoryItems.ItemNumber IS NULL) AND (dbo.AsycudaDocumentItem.RegistrationDate >=
                             (SELECT        TOP (1) OpeningStockDate
                               FROM            dbo.ApplicationSettings)) AND (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse')
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
         Begin Table = "AsycudaDocumentItem"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 206
               Right = 400
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 125
               Left = 497
               Bottom = 322
               Right = 742
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 0
               Left = 1073
               Bottom = 197
               Right = 1437
            End
            DisplayFlags = 280
            TopColumn = 25
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 85
               Left = 825
               Bottom = 282
               Right = 1059
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
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 4635
         Or = 1350
         Or = 135' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataProcessCheck-Unknown Asycuda Items'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'0
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataProcessCheck-Unknown Asycuda Items'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataProcessCheck-Unknown Asycuda Items'
GO
