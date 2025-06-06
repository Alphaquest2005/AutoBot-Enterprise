USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[BondReport-ItemNumber]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[BondReport-ItemNumber]
AS
SELECT dbo.AsycudaItemBasicInfo.ItemNumber, dbo.AsycudaItemBasicInfo.Commercial_Description, SUM(dbo.AsycudaItemBasicInfo.ItemQuantity) AS ItemQuantity, SUM(dbo.[AscyudaItemPiQuantity-Basic].PiQuantity) 
                 AS PiQuantity, SUM(dbo.AsycudaItemBasicInfo.ItemQuantity - ISNULL(dbo.[AscyudaItemPiQuantity-Basic].PiQuantity, 0)) AS Balance, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    dbo.AsycudaItemBasicInfo INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.AsycudaItemBasicInfo.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.[AscyudaItemPiQuantity-Basic] ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.[AscyudaItemPiQuantity-Basic].Item_Id
WHERE (dbo.AsycudaItemBasicInfo.CNumber IS NOT NULL) AND (dbo.AsycudaDocumentBasicInfo.DocumentType = N'IM7') AND (ISNULL(dbo.AsycudaDocumentBasicInfo.Cancelled, 0) <> 1)
GROUP BY dbo.AsycudaItemBasicInfo.ItemNumber, dbo.AsycudaItemBasicInfo.Commercial_Description, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
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
         Begin Table = "AsycudaItemBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 300
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 344
               Bottom = 161
               Right = 634
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AscyudaItemPiQuantity-Basic"
            Begin Extent = 
               Top = 6
               Left = 678
               Bottom = 140
               Right = 878
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
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondReport-ItemNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondReport-ItemNumber'
GO
