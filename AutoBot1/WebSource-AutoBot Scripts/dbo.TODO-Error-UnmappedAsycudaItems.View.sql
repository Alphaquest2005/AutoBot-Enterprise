USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-UnmappedAsycudaItems]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-Error-UnmappedAsycudaItems]
AS
SELECT DISTINCT 
                 dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.Description_of_goods, 
                 dbo.AsycudaDocumentItem.Item_price, dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentBasicInfo.AssessmentDate, dbo.AsycudaDocumentBasicInfo.DocumentType, 
                 dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.Reference, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    dbo.AsycudaDocumentItem INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.AsycudaDocumentItem.CNumber = dbo.AsycudaDocumentBasicInfo.CNumber LEFT OUTER JOIN
                 dbo.InventoryItemAliasEx ON dbo.AsycudaDocumentItem.ItemNumber = dbo.InventoryItemAliasEx.ItemNumber LEFT OUTER JOIN
                 dbo.InventoryItemAliasEx AS InventoryItemAlias_1 ON dbo.AsycudaDocumentItem.ItemNumber = InventoryItemAlias_1.AliasName LEFT OUTER JOIN
                 dbo.EntryDataDetails ON dbo.AsycudaDocumentItem.ItemNumber = dbo.EntryDataDetails.ItemNumber
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (dbo.EntryDataDetails.ItemNumber IS NULL)
	and (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse')
--AND (dbo.AsycudaDocumentBasicInfo.DocumentType = 'IM7' OR  dbo.AsycudaDocumentBasicInfo.DocumentType = 'OS7')
AND (dbo.InventoryItemAliasEx.AliasName IS NULL) AND (InventoryItemAlias_1.AliasName IS NULL)
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
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 365
               Bottom = 161
               Right = 639
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAliasEx"
            Begin Extent = 
               Top = 6
               Left = 683
               Bottom = 161
               Right = 905
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias_1"
            Begin Extent = 
               Top = 6
               Left = 949
               Bottom = 161
               Right = 1171
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 1215
               Bottom = 161
               Right = 1453
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
      Begin ColumnWidth' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnmappedAsycudaItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N's = 11
         Column = 2134
         Alias = 903
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnmappedAsycudaItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnmappedAsycudaItems'
GO
