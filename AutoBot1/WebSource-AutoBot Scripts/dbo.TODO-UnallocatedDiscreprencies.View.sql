USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UnallocatedDiscreprencies]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-UnallocatedDiscreprencies]
AS
SELECT dbo.AdjustmentDetails.EntryDataDetailsId, dbo.AdjustmentDetails.EntryDataId, dbo.AdjustmentDetails.LineNumber, dbo.AdjustmentDetails.ItemNumber, dbo.AdjustmentDetails.Quantity, dbo.AdjustmentDetails.Units, 
                 dbo.AdjustmentDetails.ItemDescription, dbo.AdjustmentDetails.Cost, dbo.AdjustmentDetails.QtyAllocated, dbo.AdjustmentDetails.UnitWeight, dbo.AdjustmentDetails.DoNotAllocate, dbo.AdjustmentDetails.TariffCode, 
                 dbo.AdjustmentDetails.Total, dbo.AdjustmentDetails.AsycudaDocumentSetId, dbo.AdjustmentDetails.InvoiceQty, dbo.AdjustmentDetails.ReceivedQty, case when EffectiveDate is null and dbo.AdjustmentDetails.Status is null then 'AutoMatch Failed' when AdjustmentDetails.DoNotAllocate = 1 then 'Do Not Allocated' when AdjustmentDetails.Status like 'Over Allocated%' then 'Over Allocated' else dbo.AdjustmentDetails.Status end as Status, 
                 dbo.AdjustmentDetails.PreviousInvoiceNumber, dbo.AdjustmentDetails.CNumber, dbo.AdjustmentDetails.CLineNumber, dbo.AdjustmentDetails.Downloaded, dbo.AdjustmentDetails.PreviousCNumber, 
                 dbo.AdjustmentDetails.DutyFreePaid, dbo.AdjustmentDetails.Type, dbo.AdjustmentDetails.Comment, dbo.AdjustmentDetails.EffectiveDate, dbo.AdjustmentDetails.Currency, dbo.AdjustmentDetails.IsReconciled, 
                 dbo.AdjustmentDetails.ApplicationSettingsId, dbo.AdjustmentDetails.FileTypeId, dbo.AdjustmentDetails.EmailId, dbo.AsycudaDocumentSet.Declarant_Reference_Number, dbo.AdjustmentDetails.InvoiceDate, 
                 dbo.AdjustmentDetails.Subject, dbo.AdjustmentDetails.EmailDate
FROM    dbo.AdjustmentDetails INNER JOIN
                 dbo.AsycudaDocumentSetEntryData ON dbo.AdjustmentDetails.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 dbo.AsycudaDocumentSet ON dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId = dbo.AsycudaDocumentSet.AsycudaDocumentSetId AND 
                 dbo.AdjustmentDetails.AsycudaDocumentSetId = dbo.AsycudaDocumentSet.AsycudaDocumentSetId LEFT OUTER JOIN
                 dbo.SystemDocumentSets ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.SystemDocumentSets.Id LEFT OUTER JOIN
                 dbo.AsycudaSalesAllocations ON dbo.AdjustmentDetails.EntryDataDetailsId = dbo.AsycudaSalesAllocations.EntryDataDetailsId
WHERE (dbo.AdjustmentDetails.Type = 'DIS') and isnull(AdjustmentDetails.IsReconciled,0) <> 1  AND (dbo.AdjustmentDetails.InvoiceQty > 0) and (case when EffectiveDate is null and dbo.AdjustmentDetails.Status is null then 'AutoMatch Failed' when AdjustmentDetails.DoNotAllocate = 1 then 'Do Not Allocated' else dbo.AdjustmentDetails.Status end is not null) AND (dbo.AsycudaSalesAllocations.AllocationId IS NULL) AND (dbo.SystemDocumentSets.Id IS NULL)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[37] 4[17] 2[23] 3) )"
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
         Top = -310
         Left = 0
      End
      Begin Tables = 
         Begin Table = "AdjustmentDetails"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 287
               Right = 283
            End
            DisplayFlags = 280
            TopColumn = 24
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 288
               Left = 44
               Bottom = 422
               Right = 283
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 365
               Left = 771
               Bottom = 520
               Right = 1043
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 582
               Left = 44
               Bottom = 737
               Right = 251
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 400
               Left = 1339
               Bottom = 492
               Right = 1523
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
      Begin ColumnWidths = 30
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
       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnallocatedDiscreprencies'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  Width = 1309
         Width = 1309
         Width = 1309
         Width = 1807
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
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnallocatedDiscreprencies'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnallocatedDiscreprencies'
GO
