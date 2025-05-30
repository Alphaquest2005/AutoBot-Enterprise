USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UnExwarehousedAdjustments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO












CREATE VIEW [dbo].[TODO-UnExwarehousedAdjustments]
AS
SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN InventoryItemsEx.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS Expr1, EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryDataDetails.Quantity, 
                 EntryData.EntryDataDate, EntryDataDetails.ItemNumber, isnull(EntryDataDetails.Status,AsycudaSalesAllocations.Status ) as Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaSalesAllocations.xStatus, 
                 Emails.Subject, Emails.EmailDate, EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, 
                 EntryDataDetails.LineNumber, CASE WHEN isnull(EntryDataDetails.TaxAmount, 0) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, EntryData.EmailId, 
                 AsycudaDocumentItemEntryDataDetails.ImportComplete, AsycudaDocumentBasicInfo.CNumber AS Expr2, AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.Reference, AsycudaSalesAllocations.Status as allostatus
FROM    ApplicationSettings INNER JOIN
                 EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItems as InventoryItemsEx ON ApplicationSettings.ApplicationSettingsId = InventoryItemsEx.ApplicationSettingsId INNER JOIN
                 EntryDataDetails ON InventoryItemsEx.Id = EntryDataDetails.InventoryItemId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id INNER JOIN
                 Emails ON EntryData.EmailId = Emails.EmailId 
				 INNER JOIN  AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId 
				 LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                 xcuda_item as AsycudaItemBasicInfo INNER JOIN
                 AsycudaDocumentBasicInfo ON AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 (Select AsycudaDocumentItemEntryDataDetails.* from AsycudaDocumentItemEntryDataDetails
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails.ASYCUDA_Id
				 where AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse') as AsycudaDocumentItemEntryDataDetails ON AsycudaItemBasicInfo.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id ON 
                 EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) and isnull(EntryDataDetails.IsReconciled,0) <> 1 and (xStatus is not null or isnull(EntryDataDetails.Status,AsycudaSalesAllocations.Status ) is not null) and (AsycudaDocumentItemEntryDataDetails.ImportComplete IS NULL)
GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN InventoryItemsEx.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaSalesAllocations.xStatus, Emails.Subject, Emails.EmailDate, 
                 EntryDataDetails.ItemDescription, EntryDataDetails.Cost, EntryDataDetails.PreviousInvoiceNumber, EntryDataDetails.Comment, EntryDataDetails.EffectiveDate, EntryDataDetails.LineNumber, 
                 EntryDataDetails.TaxAmount, EntryData.EmailId, EntryDataDetails.Quantity, AsycudaDocumentItemEntryDataDetails.ImportComplete, AsycudaDocumentBasicInfo.CNumber, 
                 AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.Reference,AsycudaSalesAllocations.Status
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
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 394
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 438
               Bottom = 161
               Right = 660
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 704
               Bottom = 140
               Right = 943
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 6
               Left = 987
               Bottom = 161
               Right = 1259
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemsEx"
            Begin Extent = 
               Top = 6
               Left = 1303
               Bottom = 161
               Right = 1525
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 1569
               Bottom = 161
               Right = 1807
            End
            DisplayFlags = 280
            TopColumn = 16
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 144
               Left = 704
               Bottom = 257' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnExwarehousedAdjustments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
               Right = 888
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 163
               Left = 1143
               Bottom = 318
               Right = 1350
            End
            DisplayFlags = 280
            TopColumn = 6
         End
         Begin Table = "Emails"
            Begin Extent = 
               Top = 156
               Left = 943
               Bottom = 290
               Right = 1143
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 254
               Right = 228
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 162
               Left = 272
               Bottom = 317
               Right = 479
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
      Begin ColumnWidths = 15
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
         Width = 1309
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnExwarehousedAdjustments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnExwarehousedAdjustments'
GO
