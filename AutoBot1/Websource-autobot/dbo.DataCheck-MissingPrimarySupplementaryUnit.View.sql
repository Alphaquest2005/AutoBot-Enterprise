USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[DataCheck-MissingPrimarySupplementaryUnit]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DataCheck-MissingPrimarySupplementaryUnit]
AS
SELECT        dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_Item.Item_Id, 'Missing Supplimentary Unit' AS Type, dbo.xcuda_Supplementary_unit.Supplementary_unit_Id, dbo.AsycudaDocumentBasicInfo.CNumber, 
                         dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.xcuda_Item.LineNumber
FROM            dbo.xcuda_Item INNER JOIN
                         dbo.xcuda_Tarification ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id INNER JOIN
                         dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                         dbo.Primary_Supplementary_Unit INNER JOIN
                         dbo.xcuda_Supplementary_unit ON dbo.Primary_Supplementary_Unit.Supplementary_unit_Id = dbo.xcuda_Supplementary_unit.Supplementary_unit_Id ON 
                         dbo.xcuda_Tarification.Item_Id = dbo.xcuda_Supplementary_unit.Tarification_Id
WHERE        (dbo.xcuda_Supplementary_unit.Tarification_Id IS NULL)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[12] 3) )"
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
               Top = 52
               Left = 529
               Bottom = 293
               Right = 888
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 216
               Left = 57
               Bottom = 421
               Right = 396
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 214
               Right = 401
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Supplementary_unit"
            Begin Extent = 
               Top = 9
               Left = 458
               Bottom = 214
               Right = 802
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 13
               Left = 1041
               Bottom = 260
               Right = 1287
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
         Width = 1500
         Width = 1500
         Width = 2775
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 2040
         Width = 1500
         Width = 1500
      End
   En' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-MissingPrimarySupplementaryUnit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'd
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
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-MissingPrimarySupplementaryUnit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-MissingPrimarySupplementaryUnit'
GO
