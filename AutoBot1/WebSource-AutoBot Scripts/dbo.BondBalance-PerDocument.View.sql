USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[BondBalance-PerDocument]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[BondBalance-PerDocument]
AS
SELECT ApplicationSettings.BondQuantum, SUM(ISNULL(AsycudaItemDutyLiablity.DutyLiability, PreviousItemsEx.DutyLiablity))*case when CustomsOperation = 'Exwarehouse' then -1 else 1 end AS TotalDutyLiablity, AsycudaDocumentTotalCIF.TotalCIF, 
                isnull(SUM((AsycudaItemDutyLiablity.DutyLiability / AsycudaItemBasicInfo.ItemQuantity) 
                 * (AsycudaItemBasicInfo.ItemQuantity - ISNULL(PiQuantity, 0))),0)*case when CustomsOperation = 'Exwarehouse' then -1 else 1 end AS DutyLiabilityBal,
				AsycudaDocument.ApplicationSettingsId, AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, 
                 AsycudaDocument.Reference as ReferenceNumber, COUNT(DISTINCT AsycudaItemBasicInfo.Item_Id) AS Lines, AsycudaDocumentCustomsProcedures.CustomsOperation 
FROM   AsycudaDocumentBasicInfo as AsycudaDocument INNER JOIN
                 ApplicationSettings ON AsycudaDocument.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId INNER JOIN
                 AsycudaItemBasicInfo ON AsycudaDocument.ASYCUDA_Id = AsycudaItemBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaItemDutyLiablity ON AsycudaItemBasicInfo.Item_Id = AsycudaItemDutyLiablity.Item_Id INNER JOIN
                 AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id LEFT OUTER JOIN
                 PreviousItemsEx ON AsycudaItemBasicInfo.Item_Id = PreviousItemsEx.PreviousItem_Id
				 inner join AsycudaDocumentTotalCIF on AsycudaDocumentTotalCIF.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
				 left outer JOIN [AscyudaItemPiQuantity-Basic] ON AsycudaItemBasicInfo.Item_Id = [AscyudaItemPiQuantity-Basic].Item_Id 
WHERE (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse' OR
                 AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse') 
				 AND (AsycudaDocument.ImportComplete = 1) 
				 and (isnull(AsycudaDocument.Cancelled,0) <> 1)
				 and AsycudaItemBasicInfo.ItemQuantity <> 0
		--and AsycudaDocument.CNumber = '33816'
GROUP BY ApplicationSettings.BondQuantum, AsycudaDocument.ApplicationSettingsId, AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, AsycudaDocument.Reference, AsycudaDocumentTotalCIF.TotalCIF,AsycudaDocumentCustomsProcedures.CustomsOperation
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
               Bottom = 263
               Right = 250
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 628
               Bottom = 161
               Right = 994
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 6
               Left = 1038
               Bottom = 119
               Right = 1238
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 138
               Left = 1165
               Bottom = 293
               Right = 1475
            End
            DisplayFlags = 280
            TopColumn = 27
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1309
         Width = 1309
         Width = 3521
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 2513
         Alias = 903
         Table = 3273
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 13' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondBalance-PerDocument'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'48
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondBalance-PerDocument'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'BondBalance-PerDocument'
GO
