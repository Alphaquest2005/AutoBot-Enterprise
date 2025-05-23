USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ItemAliasSuggestions]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-ItemAliasSuggestions]
AS
SELECT TOP (100) PERCENT dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ApplicationSettingsId, dbo.AsycudaDocumentItem.LineNumber, 
                 dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocumentItem.ReferenceNumber
FROM    dbo.AsycudaDocumentItem INNER JOIN
                     (SELECT dbo.xcuda_Goods_description.Commercial_Description, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
                      FROM     dbo.AsycudaItemBasicInfo INNER JOIN
                                       dbo.xcuda_Goods_description ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.xcuda_Goods_description.Item_Id INNER JOIN
                                       dbo.AsycudaDocumentBasicInfo ON dbo.AsycudaItemBasicInfo.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id
                      WHERE  (dbo.AsycudaDocumentBasicInfo.DocumentType LIKE N'%7')
                      GROUP BY dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.xcuda_Goods_description.Commercial_Description
                      HAVING (COUNT(DISTINCT dbo.AsycudaItemBasicInfo.ItemNumber) > 1)) AS derivedtbl_1 ON dbo.AsycudaDocumentItem.Commercial_Description = derivedtbl_1.Commercial_Description AND 
                 dbo.AsycudaDocumentItem.ApplicationSettingsId = derivedtbl_1.ApplicationSettingsId LEFT OUTER JOIN
                 dbo.InventoryItemAliasEx AS InventoryItemAliasEx_1 ON dbo.AsycudaDocumentItem.ItemNumber = InventoryItemAliasEx_1.AliasName LEFT OUTER JOIN
                 dbo.InventoryItemAliasEx ON dbo.AsycudaDocumentItem.ItemNumber = dbo.InventoryItemAliasEx.ItemNumber
WHERE (dbo.InventoryItemAliasEx.ItemNumber IS NULL) AND (InventoryItemAliasEx_1.AliasName IS NULL)
GROUP BY dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ApplicationSettingsId, dbo.AsycudaDocumentItem.LineNumber, 
                 dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocumentItem.ReferenceNumber
ORDER BY dbo.AsycudaDocumentItem.Commercial_Description
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
               Bottom = 284
               Right = 337
            End
            DisplayFlags = 280
            TopColumn = 36
         End
         Begin Table = "derivedtbl_1"
            Begin Extent = 
               Top = 6
               Left = 381
               Bottom = 119
               Right = 637
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAliasEx_1"
            Begin Extent = 
               Top = 6
               Left = 681
               Bottom = 161
               Right = 919
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAliasEx"
            Begin Extent = 
               Top = 6
               Left = 963
               Bottom = 161
               Right = 1201
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
         Width = 1820
         Width = 4438
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1780
         Width = 2095
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
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
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ItemAliasSuggestions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ItemAliasSuggestions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ItemAliasSuggestions'
GO
