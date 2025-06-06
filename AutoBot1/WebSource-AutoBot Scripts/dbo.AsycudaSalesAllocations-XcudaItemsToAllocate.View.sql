USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaSalesAllocations-XcudaItemsToAllocate]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaSalesAllocations-XcudaItemsToAllocate]
AS
SELECT   dbo.xcuda_Item.Item_Id, dbo.AsycudaDocumentAssessmentDate.ApplicationSettingsId, dbo.AsycudaDocumentAssessmentDate.AsycudaDocumentSetId, dbo.AsycudaDocumentAssessmentDate.AssessmentDate
FROM      dbo.CustomsOperations INNER JOIN
                dbo.Customs_Procedure ON dbo.CustomsOperations.Id = dbo.Customs_Procedure.CustomsOperationId INNER JOIN
                dbo.AsycudaDocumentAssessmentDate ON dbo.Customs_Procedure.Customs_ProcedureId = dbo.AsycudaDocumentAssessmentDate.Customs_ProcedureId INNER JOIN
                dbo.xcuda_Item ON dbo.AsycudaDocumentAssessmentDate.ASYCUDA_Id = dbo.xcuda_Item.ASYCUDA_Id
WHERE   (dbo.CustomsOperations.Name = N'Warehouse') 
and isnull(AsycudaDocumentAssessmentDate.Cancelled,0) <> 1 
AND ((dbo.Customs_Procedure.Stock = 1) OR (dbo.Customs_Procedure.Sales = 1))
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
         Begin Table = "CustomsOperations"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 108
               Right = 212
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 6
               Left = 250
               Bottom = 146
               Right = 512
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentAssessmentDate"
            Begin Extent = 
               Top = 6
               Left = 550
               Bottom = 146
               Right = 775
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 813
               Bottom = 146
               Right = 1087
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
         Filter = 1350
         Or = 1350
         O' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocations-XcudaItemsToAllocate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'r = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocations-XcudaItemsToAllocate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocations-XcudaItemsToAllocate'
GO
