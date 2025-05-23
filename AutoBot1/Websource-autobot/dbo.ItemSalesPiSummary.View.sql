USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesPiSummary]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[ItemSalesPiSummary]
AS
select [ItemSalesPiSummary-Adjustments].* from [dbo].[ItemSalesPiSummary-Adjustments]

union 

select [ItemSalesPiSummary-Sales].* from [dbo].[ItemSalesPiSummary-Sales]

--SELECT distinct  TOP (100) PERCENT AsycudaSalesAllocations.PreviousItem_Id, EntryDataDetails.ItemNumber AS ItemNumber, EntryDataDetails.Quantity AS SalesQty, EntryDataDetails.QtyAllocated AS SalesAllocatedQty, 
--                 CASE WHEN isnull(entrydatadetails.taxamount, COALESCE (EntryData_Sales.Tax, isnull(EntryData_Adjustments.Tax, 0))) = 0 THEN 'Duty Free' ELSE 'Duty Paid' END AS DutyFreePaid, 
--                 PreviousDocumentItem.LineNumber AS pLineNumber, CASE WHEN isnull(entrydatadetails.taxamount, isnull(EntryData_Sales.Tax, isnull(EntryData_Adjustments.Tax, 0))) 
--                 = 0 THEN DfqtyAllocated ELSE DPQtyallocated END AS pQtyAllocated, AsycudaDocumentBasicInfo.CNumber AS pCNumber, AsycudaDocumentBasicInfo.RegistrationDate AS pRegistrationDate, 
--                 AsycudaDocumentBasicInfo.AssessmentDate AS pAssessmentDate, ISNULL(AsycudaSalesAllocations.QtyAllocated, 0) AS QtyAllocated, CASE WHEN entrydata_adjustments.entrydata_id IS NULL 
--                 THEN entrydatadate ELSE effectivedate END AS EntryDataDate, EntryData.ApplicationSettingsId, CASE WHEN EntryData_sales.entrydata_id IS NOT NULL 
--                 THEN 'Sales' WHEN EntryData_Adjustments.entrydata_id IS NOT NULL THEN EntryData_Adjustments.Type ELSE '' END AS Type, AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.Status
--FROM    AsycudaSalesAllocations INNER JOIN
--                 xcuda_Item AS PreviousDocumentItem ON AsycudaSalesAllocations.PreviousItem_Id = PreviousDocumentItem.Item_Id INNER JOIN
--                 AsycudaDocumentBasicInfo ON PreviousDocumentItem.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
--                 EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
--                 EntryData ON EntryDataDetails.EntryDataId = EntryData.EntryDataId LEFT OUTER JOIN
--                 EntryData_Sales ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id LEFT OUTER JOIN
--                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
--WHERE (AsycudaSalesAllocations.Status IS NULL) and (EntryData_Sales.EntryData_Id is not null or EntryData_Adjustments.EntryData_Id is not null)
--GROUP BY AsycudaSalesAllocations.PreviousItem_Id, PreviousDocumentItem.LineNumber, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, 
--                 EntryData.ApplicationSettingsId, AsycudaSalesAllocations.QtyAllocated, EntryData_Adjustments.EntryData_Id, EntryData.EntryDataDate, EntryDataDetails.EffectiveDate, AsycudaSalesAllocations.AllocationId, 
--                 AsycudaSalesAllocations.Status, EntryData_Sales.EntryData_Id, EntryData_Adjustments.Type, EntryDataDetails.Quantity, EntryDataDetails.QtyAllocated, EntryData_Adjustments.Tax, EntryDataDetails.TaxAmount, 
--                 EntryData_Sales.Tax, PreviousDocumentItem.DFQtyAllocated, PreviousDocumentItem.DPQtyAllocated, EntryDataDetails.ItemNumber
--ORDER BY EntryDataDate
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[35] 4[21] 2[23] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
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
         Left = -440
      End
      Begin Tables = 
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 0
               Left = 782
               Bottom = 283
               Right = 1083
            End
            DisplayFlags = 280
            TopColumn = 4
         End
         Begin Table = "PreviousDocumentItem"
            Begin Extent = 
               Top = 7
               Left = 387
               Bottom = 162
               Right = 678
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 0
               Left = 0
               Bottom = 155
               Right = 274
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 1183
               Bottom = 161
               Right = 1421
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 42
               Left = 1593
               Bottom = 197
               Right = 1815
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 6
               Left = 1859
               Bottom = 140
               Right = 2048
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 144
               Left = 1859
               Bottom ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesPiSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'= 236
               Right = 2043
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
      Begin ColumnWidths = 12
         Column = 11180
         Alias = 2880
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1348
         Filter = 5001
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesPiSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesPiSummary'
GO
