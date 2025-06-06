USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaItemRemainingQuantities]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaItemRemainingQuantities]
AS
SELECT        TOP (100) PERCENT dbo.AsycudaDocumentItem.Item_Id, dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.RegistrationDate, 
                         dbo.AsycudaDocumentItem.AssessmentDate, dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.DPQtyAllocated, 
                         dbo.AsycudaDocumentItem.DFQtyAllocated, dbo.AsycudaDocumentItem.DPQtyAllocated + dbo.AsycudaDocumentItem.DFQtyAllocated AS QtyAllocated, ISNULL(dbo.AsycudaDocumentItem.PiQuantity, 0) AS PiQuantity, 
                         ISNULL(dbo.AsycudaDocumentItem.ItemQuantity, 0) - ISNULL(dbo.AsycudaDocumentItem.PiQuantity, 0) AS RemainingBalance, ISNULL(dbo.AsycudaDocumentItem.ItemQuantity, 0) 
                         - (dbo.AsycudaDocumentItem.DPQtyAllocated + dbo.AsycudaDocumentItem.DFQtyAllocated) AS xRemainingBalance, dbo.AsycudaDocumentItem.ApplicationSettingsId
FROM            dbo.AsycudaDocumentItem INNER JOIN
                         dbo.Customs_Procedure ON dbo.AsycudaDocumentItem.CustomsProcedure = dbo.Customs_Procedure.CustomsProcedure
WHERE        (dbo.Customs_Procedure.CustomsOperationId = 2) AND (dbo.AsycudaDocumentItem.Cancelled = 0)
GROUP BY dbo.AsycudaDocumentItem.Item_Id, dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocumentItem.AssessmentDate, 
                         dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.DPQtyAllocated, dbo.AsycudaDocumentItem.DFQtyAllocated, 
                         dbo.AsycudaDocumentItem.DPQtyAllocated + dbo.AsycudaDocumentItem.DFQtyAllocated, ISNULL(dbo.AsycudaDocumentItem.PiQuantity, 0), ISNULL(dbo.AsycudaDocumentItem.ItemQuantity, 0) 
                         - ISNULL(dbo.AsycudaDocumentItem.PiQuantity, 0), ISNULL(dbo.AsycudaDocumentItem.ItemQuantity, 0) - (dbo.AsycudaDocumentItem.DPQtyAllocated + dbo.AsycudaDocumentItem.DFQtyAllocated), 
                         dbo.AsycudaDocumentItem.ApplicationSettingsId
ORDER BY dbo.AsycudaDocumentItem.AssessmentDate
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
               Left = 38
               Bottom = 136
               Right = 288
            End
            DisplayFlags = 280
            TopColumn = 43
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 6
               Left = 326
               Bottom = 136
               Right = 573
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
         Column = 13245
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemRemainingQuantities'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemRemainingQuantities'
GO
