USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentCustomsProcedures]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentCustomsProcedures]
AS
SELECT dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id, dbo.Customs_Procedure.Document_TypeId, dbo.Customs_Procedure.Customs_ProcedureId, dbo.Customs_Procedure.IsObsolete, 
                 dbo.Customs_Procedure.IsPaid, dbo.Customs_Procedure.BondTypeId, dbo.Customs_Procedure.Stock, dbo.Customs_Procedure.Discrepancy, dbo.Customs_Procedure.Adjustment, dbo.Customs_Procedure.Sales, 
                 dbo.Customs_Procedure.CustomsOperationId, dbo.Customs_Procedure.SubmitToCustoms, dbo.CustomsOperations.Name AS CustomsOperation, dbo.Customs_Procedure.CustomsProcedure
FROM    dbo.xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 dbo.Customs_Procedure ON dbo.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = dbo.Customs_Procedure.Customs_ProcedureId INNER JOIN
                 dbo.CustomsOperations ON dbo.Customs_Procedure.CustomsOperationId = dbo.CustomsOperations.Id
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
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 289
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 6
               Left = 333
               Bottom = 310
               Right = 607
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CustomsOperations"
            Begin Extent = 
               Top = 6
               Left = 651
               Bottom = 119
               Right = 835
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
      Begin ColumnWidths = 14
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
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
         Column = 4700
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentCustomsProcedures'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentCustomsProcedures'
GO
