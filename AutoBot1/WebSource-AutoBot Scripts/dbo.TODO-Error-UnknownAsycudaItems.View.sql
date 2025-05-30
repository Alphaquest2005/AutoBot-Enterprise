USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-UnknownAsycudaItems]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-Error-UnknownAsycudaItems]
AS
SELECT DISTINCT 
                 dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.Description_of_goods, dbo.AsycudaDocumentItem.Item_price, 
                 dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocument.DocumentType, dbo.AsycudaDocument.ReferenceNumber, dbo.AsycudaDocument.ApplicationSettingsId, 
                 dbo.AsycudaDocumentItem.ItemNumber
FROM    dbo.AsycudaDocumentItem INNER JOIN
                 dbo.xcuda_HScode ON dbo.AsycudaDocumentItem.ItemNumber = dbo.xcuda_HScode.Precision_4 INNER JOIN
                 dbo.AsycudaDocument ON dbo.AsycudaDocumentItem.CNumber = dbo.AsycudaDocument.CNumber LEFT OUTER JOIN
                 dbo.InventoryItems ON dbo.xcuda_HScode.Precision_4 = dbo.InventoryItems.ItemNumber
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
WHERE (dbo.InventoryItems.ItemNumber IS NULL) AND (dbo.AsycudaDocumentItem.RegistrationDate >=
                     (SELECT TOP (1) OpeningStockDate
                      FROM     dbo.ApplicationSettings)) 
					  and (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse')
					  --AND (dbo.AsycudaDocument.DocumentType = 'IM7' OR  dbo.AsycudaDocument.DocumentType = 'OS7') 
					  AND (dbo.AsycudaDocument.ImportComplete = 1)
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
               Top = 6
               Left = 44
               Bottom = 161
               Right = 321
            End
            DisplayFlags = 280
            TopColumn = 13
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 6
               Left = 365
               Bottom = 161
               Right = 565
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 72
               Left = 614
               Bottom = 227
               Right = 908
            End
            DisplayFlags = 280
            TopColumn = 29
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 6
               Left = 947
               Bottom = 161
               Right = 1169
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
         Column = 1440
         Alias = 903
         Table = 3247
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1348
         Or = 1350
         Or = 1350
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnknownAsycudaItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnknownAsycudaItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnknownAsycudaItems'
GO
