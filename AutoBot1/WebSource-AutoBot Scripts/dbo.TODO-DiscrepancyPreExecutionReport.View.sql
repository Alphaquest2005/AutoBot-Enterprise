USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-DiscrepancyPreExecutionReport]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-DiscrepancyPreExecutionReport]
AS
SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, AdjustmentDetails.Units, 
                 AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, AdjustmentDetails.DoNotAllocate, AdjustmentDetails.TariffCode, 
                 AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, AsycudaDocumentSet.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, AdjustmentDetails.ReceivedQty, AdjustmentDetails.PreviousInvoiceNumber, 
                 AdjustmentDetails.PreviousCNumber, AdjustmentDetails.Comment, AdjustmentDetails.Status, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Currency, AdjustmentDetails.ApplicationSettingsId, 
                 AdjustmentDetails.Type, AsycudaDocumentSet.Declarant_Reference_Number, AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, AdjustmentDetails.DutyFreePaid, 
                 AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.Reference
FROM    AsycudaDocumentSet INNER JOIN
                 AdjustmentDetails INNER JOIN
                 AsycudaDocumentSetEntryData ON AdjustmentDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id ON 
                 AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId AND AsycudaDocumentSet.AsycudaDocumentSetId <> AdjustmentDetails.AsycudaDocumentSetId INNER JOIN
                 AsycudaDocumentItemEntryDataDetails ON AdjustmentDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId INNER JOIN
                 AsycudaDocumentBasicInfo ON AsycudaDocumentItemEntryDataDetails.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (AsycudaDocumentItemEntryDataDetails.ImportComplete = 0) AND (SystemDocumentSets.Id IS NULL) and (AdjustmentDetails.Type = N'DIS')
GROUP BY AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId, AdjustmentDetails.LineNumber, AdjustmentDetails.ItemNumber, AdjustmentDetails.Quantity, AdjustmentDetails.Units, 
                 AdjustmentDetails.ItemDescription, AdjustmentDetails.Cost, AdjustmentDetails.QtyAllocated, AdjustmentDetails.UnitWeight, AdjustmentDetails.TariffCode, AdjustmentDetails.CNumber, AdjustmentDetails.CLineNumber, 
                 AsycudaDocumentSet.AsycudaDocumentSetId, AdjustmentDetails.InvoiceQty, AdjustmentDetails.ReceivedQty, AdjustmentDetails.PreviousInvoiceNumber, AdjustmentDetails.PreviousCNumber, 
                 AdjustmentDetails.Comment, AdjustmentDetails.Status, AdjustmentDetails.EffectiveDate, AdjustmentDetails.Currency, AdjustmentDetails.ApplicationSettingsId, AdjustmentDetails.Type, 
                 AsycudaDocumentSet.Declarant_Reference_Number, AdjustmentDetails.InvoiceDate, AdjustmentDetails.Subject, AdjustmentDetails.EmailDate, AdjustmentDetails.DutyFreePaid, 
                 AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.Reference, AdjustmentDetails.DoNotAllocate
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
         Begin Table = "AdjustmentDetails"
            Begin Extent = 
               Top = 6
               Left = 360
               Bottom = 161
               Right = 599
            End
            DisplayFlags = 280
            TopColumn = 0
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
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 1177
               Bottom = 161
               Right = 1468
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 1512
               Bottom = 161
               Right = 1786
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
   Begin' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepancyPreExecutionReport'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' CriteriaPane = 
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepancyPreExecutionReport'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepancyPreExecutionReport'
GO
