USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[BondBalance-PerDocumentDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[BondBalance-PerDocumentDetails]
AS
SELECT top (100) percent AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.AssessmentDate, dbo.PreviousItemsEx.Current_item_number, AsycudaItemBasicInfo.ItemNumber, dbo.AsycudaItemBasicInfo.ItemQuantity, dbo.PreviousItemsEx.Prev_reg_nbr, 
                 dbo.PreviousItemsEx.Prev_reg_year, dbo.PreviousItemsEx.Previous_value, dbo.PreviousItemsEx.Preveious_suplementary_quantity, dbo.PreviousItemsEx.pLineNumber, dbo.PreviousItemsEx.TotalDutyLiablity, 
                 dbo.PreviousItemsEx.DutyLiablity, dbo.PreviousItemsEx.Previous_value * dbo.PreviousItemsEx.Preveious_suplementary_quantity AS TotalCIF, dbo.xcuda_Valuation_item.Total_CIF_itm, AsycudaDocument.ApplicationSettingsId
FROM    dbo.xcuda_Valuation_item INNER JOIN
                 dbo.PreviousItemsEx ON dbo.xcuda_Valuation_item.Item_Id = dbo.PreviousItemsEx.PreviousDocumentItemId RIGHT OUTER JOIN
                AsycudaDocumentBasicInfo as AsycudaDocument INNER JOIN
                 dbo.ApplicationSettings ON AsycudaDocument.ApplicationSettingsId = dbo.ApplicationSettings.ApplicationSettingsId INNER JOIN
                 dbo.AsycudaItemBasicInfo ON AsycudaDocument.ASYCUDA_Id = dbo.AsycudaItemBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.AsycudaItemDutyLiablity ON dbo.AsycudaItemBasicInfo.Item_Id = dbo.AsycudaItemDutyLiablity.Item_Id INNER JOIN
                 dbo.AsycudaDocumentCustomsProcedures ON dbo.AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id ON 
                 dbo.PreviousItemsEx.PreviousItem_Id = dbo.AsycudaItemBasicInfo.Item_Id
WHERE (dbo.AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse' OR
                 dbo.AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse') AND (AsycudaDocument.ImportComplete = 1) AND (ISNULL(AsycudaDocument.Cancelled, 0) <> 1)
GROUP BY AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, AsycudaDocument.Reference, dbo.AsycudaItemBasicInfo.ItemNumber, 
                 dbo.AsycudaItemBasicInfo.ItemQuantity, dbo.PreviousItemsEx.Prev_reg_nbr, dbo.PreviousItemsEx.Prev_reg_year, dbo.PreviousItemsEx.Previous_value, dbo.PreviousItemsEx.Preveious_suplementary_quantity, 
                 dbo.PreviousItemsEx.pLineNumber, dbo.PreviousItemsEx.TotalDutyLiablity, dbo.PreviousItemsEx.DutyLiablity, dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.PreviousItemsEx.Current_item_number, AsycudaDocument.ApplicationSettingsId,
				  AsycudaDocument.RegistrationDate, AsycudaDocument.AssessmentDate
order by AsycudaDocument.AssessmentDate
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
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 367
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PreviousItemsEx"
            Begin Extent = 
               Top = 6
               Left = 411
               Bottom = 161
               Right = 725
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 6
               Left = 769
               Bottom = 161
               Right = 1079
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 1123
               Bottom = 161
               Right = 1489
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemBasicInfo"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 317
               Right = 250
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 162
               Left = 294
               Bottom = 275
               Right = 494
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentCustomsProcedures"
            Begin Extent = 
               Top = 162
               Left = 538
          ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondBalance-PerDocumentDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     Bottom = 317
               Right = 781
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondBalance-PerDocumentDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondBalance-PerDocumentDetails'
GO
