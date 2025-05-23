USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[CustomsProcedureEntryDataTypes]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CustomsProcedureEntryDataTypes]
AS
	SELECT        Customs_Procedure.CustomsProcedure, 'DIS' AS EntryDataType, CustomsOperations.Id AS CustomsOperationsId, CustomsOperations.Name AS CustomsOperation
	FROM            Customs_Procedure INNER JOIN
							 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id
	WHERE        (Customs_Procedure.Discrepancy = 1) AND (ISNULL(Customs_Procedure.IsObsolete, 0) <> 1)
union 
	SELECT        Customs_Procedure.CustomsProcedure, 'PO' AS EntryDataType, CustomsOperations.Id AS CustomsOperationsId, CustomsOperations.Name AS CustomsOperation
	FROM            Customs_Procedure INNER JOIN
							 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id where isnull(IsObsolete,0) <> 1 and CustomsOperationId in (1,2) and isnull(Stock, 0) = 0 and isnull(Discrepancy, 0) = 0 and isnull(Adjustment, 0) = 0 --and isnull(Sales, 0) = 0
union
	SELECT        Customs_Procedure.CustomsProcedure, 'ADJ' AS EntryDataType, CustomsOperations.Id AS CustomsOperationsId, CustomsOperations.Name AS CustomsOperation
	FROM            Customs_Procedure INNER JOIN
							 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id where Adjustment = 1 and isnull(IsObsolete,0) <> 1
union
	SELECT        Customs_Procedure.CustomsProcedure, 'Sales' AS EntryDataType, CustomsOperations.Id AS CustomsOperationsId, CustomsOperations.Name AS CustomsOperation
	FROM            Customs_Procedure INNER JOIN
							 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id where Sales = 1 and isnull(IsObsolete,0) <> 1 and CustomsOperationId = 3
union
	SELECT        Customs_Procedure.CustomsProcedure, 'OPS' AS EntryDataType, CustomsOperations.Id AS CustomsOperationsId, CustomsOperations.Name AS CustomsOperation
	FROM            Customs_Procedure INNER JOIN
							 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id where Stock = 1 and isnull(IsObsolete,0) <> 1 and CustomsOperationId = 2
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[6] 3) )"
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CustomsProcedureEntryDataTypes'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CustomsProcedureEntryDataTypes'
GO
