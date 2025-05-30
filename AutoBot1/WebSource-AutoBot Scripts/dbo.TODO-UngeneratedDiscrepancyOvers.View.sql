USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UngeneratedDiscrepancyOvers]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-UngeneratedDiscrepancyOvers]
AS
SELECT AdjustmentOvers.EntryDataDetailsId, AdjustmentOvers.EntryDataId, AdjustmentOvers.LineNumber, AdjustmentOvers.ItemNumber, AdjustmentOvers.Quantity, AdjustmentOvers.Units, 
                 AdjustmentOvers.ItemDescription, AdjustmentOvers.Cost, AdjustmentOvers.QtyAllocated, AdjustmentOvers.UnitWeight, AdjustmentOvers.DoNotAllocate, AdjustmentOvers.TariffCode, AdjustmentOvers.CNumber, 
                 AdjustmentOvers.CLineNumber, AdjustmentOvers.AsycudaDocumentSetId, AdjustmentOvers.InvoiceQty, AdjustmentOvers.ReceivedQty, AdjustmentOvers.PreviousInvoiceNumber, AdjustmentOvers.PreviousCNumber, 
                 AdjustmentOvers.Comment, CASE WHEN Status IS NULL AND cost = 0 THEN 'Zero Cost' ELSE status END AS Status, AdjustmentOvers.EffectiveDate, AdjustmentOvers.Currency, AdjustmentOvers.ApplicationSettingsId, 
                 AdjustmentOvers.Type, AsycudaDocumentSet.Declarant_Reference_Number, AdjustmentOvers.InvoiceDate, AdjustmentOvers.Subject, AdjustmentOvers.EmailDate, AdjustmentOvers.DutyFreePaid, 
                 AdjustmentOvers.EmailId
FROM    AsycudaDocumentSet INNER JOIN
                 AdjustmentOvers INNER JOIN
                 AsycudaDocumentSetEntryData ON AdjustmentOvers.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id ON 
                 AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId AND 
                 AsycudaDocumentSet.AsycudaDocumentSetId = AdjustmentOvers.AsycudaDocumentSetId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                 (select AsycudaDocumentItemEntryDataDetails.* 
				 from AsycudaDocumentItemEntryDataDetails 
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails.ASYCUDA_Id
				 where AsycudaDocumentCustomsProcedures.Discrepancy = 1 --AsycudaDocumentItemEntryDataDetails.DocumentType not in ('IM7','EX9')
				 ) as AsycudaDocumentItemEntryDataDetails ON AdjustmentOvers.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (AdjustmentOvers.Type = 'DIS') AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId is null) AND (SystemDocumentSets.Id IS NULL) and isnull(AdjustmentOvers.IsReconciled,0) <> 1
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
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 316
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentOvers"
            Begin Extent = 
               Top = 6
               Left = 360
               Bottom = 161
               Right = 599
            End
            DisplayFlags = 280
            TopColumn = 25
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 643
               Bottom = 140
               Right = 882
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 926
               Bottom = 161
               Right = 1133
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
      Begin ColumnWidths = 10
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
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 7082
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UngeneratedDiscrepancyOvers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UngeneratedDiscrepancyOvers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UngeneratedDiscrepancyOvers'
GO
