USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ExistingAllocations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




	CREATE VIEW [dbo].[ExistingAllocations]
	AS
	select   TOP (100) PERCENT ROW_NUMBER() over (partition by ApplicationSettingsId order by ApplicationSettingsId) as Id, * from
	(
		
	--------------anything that matches the AsycudaDocumentItemEntryDataDetails...
		SELECT			AsycudaDocumentItemEntryDataDetails.Asycuda_id AS xAsycudaId, AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId, AsycudaDocumentItemEntryDataDetails.Item_Id AS xItemId, 
								 PreviousItemsEx.PreviousDocumentItemId AS pItemId, PreviousItemsEx.Prev_reg_nbr AS pCnumber, PreviousItemsEx.pLineNumber, ISNULL(EntryDataDetails.EffectiveDate, EntryData.EntryDataDate) AS Date, 
								 AsycudaDocumentItemEntryDataDetails.ApplicationSettingsId, AsycudaItemBasicInfo.CNumber AS xCnumber, AsycudaItemBasicInfo.LineNumber AS xLineNumber, EntryData.EntryDataId, EntryData.EntryDataDate, 
								 EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, AsycudaDocumentItemEntryDataDetails.DocumentType, 
								 AsycudaDocumentItemEntryDataDetails.Quantity AS xQuantity , EntryDataDetails.Quantity as SalesQuantity, PreviousItemsEx.Suplementary_Quantity, PreviousItemsEx.DutyFreePaid, EntryDataDetails.InventoryItemId,PreviousItemsEx.CustomsProcedure
		FROM            EntryDataDetails INNER JOIN
								 AsycudaDocumentItemEntryDataDetails ON EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId INNER JOIN
								 PreviousItemsEx ON AsycudaDocumentItemEntryDataDetails.Item_Id = PreviousItemsEx.AsycudaDocumentItemId INNER JOIN
								 AsycudaItemBasicInfo ON AsycudaDocumentItemEntryDataDetails.Item_Id = AsycudaItemBasicInfo.Item_Id INNER JOIN
								 EntryData ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id
								  left outer JOIN     AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId
		WHERE        (AsycudaDocumentItemEntryDataDetails.ImportComplete = 1)  and  PreviousItemsEx.PreviousDocumentItemId is not null AND (AsycudaSalesAllocations.EntryDataDetailsId IS NULL)


		union

		SELECT   AsycudaItemBasicInfo.ASYCUDA_Id AS xAsycudaId, EntryDataDetails.EntryDataDetailsId, AsycudaItemBasicInfo.Item_Id AS xItemId, PreviousItemsEx.PreviousDocumentItemId AS pItemId, 
							 PreviousItemsEx.Prev_reg_nbr AS pCnumber, PreviousItemsEx.pLineNumber, ISNULL(EntryDataDetails.EffectiveDate, EntryData.EntryDataDate) AS Date, EntryData.ApplicationSettingsId, 
							 AsycudaItemBasicInfo.CNumber AS xCnumber, AsycudaItemBasicInfo.LineNumber AS xLineNumber, EntryData.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
							 PreviousItemsEx.DocumentType, PreviousItemsEx.Suplementary_Quantity AS xQuantity, EntryDataDetails.Quantity as SalesQuantity, PreviousItemsEx.Suplementary_Quantity, PreviousItemsEx.DutyFreePaid, EntryDataDetails.InventoryItemId,PreviousItemsEx.CustomsProcedure
		FROM            EntryData INNER JOIN
								 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
								 AsycudaItemBasicInfo INNER JOIN
								 PreviousItemsEx ON AsycudaItemBasicInfo.Item_Id = PreviousItemsEx.AsycudaDocumentItemId ON EntryDataDetails.EntryDataId = 'Asycuda-C#' + PreviousItemsEx.CNumber +  '-' + cast(PreviousItemsEx.Current_item_number as nvarchar(50))
								 left outer JOIN     AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId
		where PreviousItemsEx.PreviousDocumentItemId is not null AND (AsycudaSalesAllocations.EntryDataDetailsId IS NULL)

		union 

		-------------- ex9 with out sales
		SELECT PreviousItemsEx.ASYCUDA_Id AS xAsycudaId, 0 AS EntryDataDetailsId, PreviousItemsEx.AsycudaDocumentItemId AS xItemId, PreviousItemsEx.PreviousDocumentItemId AS pItemId, 
                 PreviousItemsEx.Prev_reg_nbr AS pCnumber, PreviousItemsEx.pLineNumber, PreviousItemsEx.AssessmentDate AS Date, PreviousItemsEx.ApplicationSettingsId, PreviousItemsEx.CNumber AS xCnumber, 
                 PreviousItemsEx.Current_item_number AS xLineNumber, 'Unknown' AS EntryDataId, PreviousItemsEx.AssessmentDate AS EntryDataDate, PreviousItemsEx.Prev_decl_HS_spec AS ItemNumber, 
                 'See previous declaration Details' AS ItemDescription, PreviousItemsEx.DocumentType, PreviousItemsEx.Suplementary_Quantity AS xQuantity, PreviousItemsEx.Suplementary_Quantity AS SalesQuantity, PreviousItemsEx.Suplementary_Quantity, PreviousItemsEx.DutyFreePaid, 
                 0 AS InventoryItemId,PreviousItemsEx.CustomsProcedure
		FROM    PreviousItemsEx INNER JOIN
						 AsycudaDocumentBasicInfo ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
						 Customs_Procedure ON PreviousItemsEx.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId LEFT OUTER JOIN
						 AsycudaDocumentItemEntryDataDetails ON PreviousItemsEx.AsycudaDocumentItemId = AsycudaDocumentItemEntryDataDetails.Item_Id
		WHERE (PreviousItemsEx.PreviousDocumentItemId IS NOT NULL) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (AsycudaDocumentBasicInfo.ImportComplete = 1) AND 
						 (Customs_Procedure.Sales <> 1) AND ((Customs_Procedure.Adjustment = 1) or (Customs_Procedure.Discrepancy = 1) or (Customs_Procedure.Stock = 1))


	) as t 

	ORDER BY Date
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[17] 2[11] 3) )"
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
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 32
               Left = 1122
               Bottom = 187
               Right = 1361
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 273
               Left = 473
               Bottom = 463
               Right = 767
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "PreviousItemsEx"
            Begin Extent = 
               Top = 0
               Left = 499
               Bottom = 191
               Right = 797
            End
            DisplayFlags = 280
            TopColumn = 16
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 207
               Left = 1536
               Bottom = 362
               Right = 1774
            End
            DisplayFlags = 280
            TopColumn = 6
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 318
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "CustomsOperations"
            Begin Extent = 
               Top = 306
               Left = 77
               Bottom = 411
               Right = 261
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 180
               Left = 1087
               Bottom = ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ExistingAllocations'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'335
               Right = 1309
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemBasicInfo"
            Begin Extent = 
               Top = 8
               Left = 1519
               Bottom = 163
               Right = 1759
            End
            DisplayFlags = 280
            TopColumn = 7
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 0
               Left = 815
               Bottom = 155
               Right = 1022
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
      Begin ColumnWidths = 17
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1689
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1623
         Width = 2657
         Width = 2108
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2867
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 2108
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ExistingAllocations'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ExistingAllocations'
GO
