USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-UnlinkedPreviousItems]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-Error-UnlinkedPreviousItems]
AS
SELECT dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_PreviousItem.PreviousItem_Id, 'UnLinkedPreviousItem' AS Type, dbo.xcuda_Item.IsAssessed, dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.CNumber, 
                 dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocumentItem.ReferenceNumber, dbo.AsycudaDocumentItem.ApplicationSettingsId, dbo.AsycudaDocumentBasicInfo.DocumentType, 
                 dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description AS Description
FROM    dbo.xcuda_PreviousItem WITH (Nolock) INNER JOIN
                 dbo.xcuda_Item WITH (Nolock) ON dbo.xcuda_PreviousItem.PreviousItem_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                 dbo.AsycudaDocumentItem ON dbo.xcuda_Item.Item_Id = dbo.AsycudaDocumentItem.Item_Id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.AsycudaDocumentItem.AsycudaDocumentId = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.EntryPreviousItems WITH (Nolock) ON dbo.xcuda_PreviousItem.PreviousItem_Id = dbo.EntryPreviousItems.PreviousItem_Id
WHERE (dbo.EntryPreviousItems.PreviousItem_Id IS NULL)
GROUP BY dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_PreviousItem.PreviousItem_Id, dbo.xcuda_Item.IsAssessed, dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.CNumber, 
                 dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocumentItem.ReferenceNumber, dbo.AsycudaDocumentItem.ApplicationSettingsId, dbo.AsycudaDocumentBasicInfo.DocumentType, 
                 dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description
HAVING (dbo.xcuda_Item.IsAssessed = 1)
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
         Begin Table = "xcuda_PreviousItem"
            Begin Extent = 
               Top = 123
               Left = 1342
               Bottom = 278
               Right = 1656
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 154
               Left = 734
               Bottom = 309
               Right = 1041
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItem"
            Begin Extent = 
               Top = 36
               Left = 384
               Bottom = 255
               Right = 677
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "EntryPreviousItems"
            Begin Extent = 
               Top = 3
               Left = 1517
               Bottom = 137
               Right = 1750
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 41
               Left = 49
               Bottom = 196
               Right = 339
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
      Begin ColumnWidths' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnlinkedPreviousItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 12
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
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnlinkedPreviousItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnlinkedPreviousItems'
GO
