USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentsToXML]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-AdjustmentsToXML]
AS

SELECT  row_number() OVER (ORDER BY Asycudadocumentsetid) AS Id, EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, IsClassified, AdjustmentType, InvoiceNo, InvoiceQty, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, Declarant_Reference_Number, EmailId, EntryDataDetailsKey
FROM    (SELECT t.EntryDataDetailsId, t.ApplicationSettingsId, t.AsycudaDocumentSetId, t.IsClassified, t.AdjustmentType, t.InvoiceNo, t.InvoiceQty, t.ReceivedQty, t.InvoiceDate, t.ItemNumber, t.Status, t.CNumber, 
                                  t.Declarant_Reference_Number, t.EmailId, t.EntryDataDetailsKey
                 FROM     (SELECT  EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                                                    CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
                                                    EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, 
                                                    AsycudaDocumentSet.Declarant_Reference_Number, EmailId, EntryDataDetails.EntryDataDetailsKey
                                   FROM     ApplicationSettings INNER JOIN
                                                    EntryData WITH (NOLOCK) ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                                                    AsycudaDocumentSetEntryData WITH (NOLOCK) ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                                                    AsycudaDocumentSet WITH (NOLOCK) ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                                                    AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                                                    InventoryItems WITH (NOLOCK) ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                                                    EntryDataDetails WITH (NOLOCK) ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                                                    EntryData_Adjustments WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                                                    AsycudaSalesAllocations WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                                                    SystemDocumentSets WITH (NOLOCK) ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
                                   WHERE  (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND (ISNULL(EntryDataDetails.DoNotAllocate, 0) <> 1) AND (ISNULL(EntryDataDetails.IsReconciled, 0) <> 1) AND 
                                                    (AsycudaSalesAllocations.xStatus IS NULL)
                                   --GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId,  dbo.InventoryItems.TariffCode, EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                                   --                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, EmailId, EntryDataDetails.EntryDataDetailsKey
                                  ) AS t 
								where t.EntryDataDetailsId not in 
								(SELECT EntryDataDetailsId--, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
                                       FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
									   inner join Customs_Procedure WITH (NOLOCK) ON Customs_Procedure.CustomsProcedure = AsycudaDocumentItemEntryDataDetails_1.CustomsProcedure
									   where Customs_Procedure.Discrepancy = 1 
                                      -- WHERE  (DocumentType NOT IN ('IM7', 'EX9')
									   ) 
									 ) AS xx
--where (EntryDataDetailsId IS NOT NULL) 
GROUP BY EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, AdjustmentType, InvoiceNo, InvoiceQty, IsClassified, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, 
                 Declarant_Reference_Number, EmailId, EntryDataDetailsKey



--SELECT  row_number() OVER (ORDER BY Asycudadocumentsetid) AS Id, EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, IsClassified, AdjustmentType, InvoiceNo, InvoiceQty, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, Declarant_Reference_Number, EmailId, EntryDataDetailsKey
--FROM    (SELECT t.EntryDataDetailsId, t.ApplicationSettingsId, t.AsycudaDocumentSetId, t.IsClassified, t.AdjustmentType, t.InvoiceNo, t.InvoiceQty, t.ReceivedQty, t.InvoiceDate, t.ItemNumber, t.Status, t.CNumber, 
--                                  t.Declarant_Reference_Number, t.EmailId, t.EntryDataDetailsKey
--                 FROM     (SELECT  EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
--                                                    CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
--                                                    EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, 
--                                                    AsycudaDocumentSet.Declarant_Reference_Number, EmailId, EntryDataDetails.EntryDataDetailsKey
--                                   FROM     ApplicationSettings INNER JOIN
--                                                    EntryData WITH (NOLOCK) ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
--                                                    AsycudaDocumentSetEntryData WITH (NOLOCK) ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
--                                                    AsycudaDocumentSet WITH (NOLOCK) ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
--                                                    AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
--                                                    InventoryItems WITH (NOLOCK) ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
--                                                    EntryDataDetails WITH (NOLOCK) ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
--                                                    EntryData_Adjustments WITH (NOLOCK) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
--                                                    AsycudaSalesAllocations WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
--                                                    SystemDocumentSets WITH (NOLOCK) ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
--                                   WHERE  (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND (ISNULL(EntryDataDetails.DoNotAllocate, 0) <> 1) AND (ISNULL(EntryDataDetails.IsReconciled, 0) <> 1) AND 
--                                                    (AsycudaSalesAllocations.xStatus IS NULL)
--                                   GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId,  dbo.InventoryItems.TariffCode, EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
--                                                    EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number, EmailId, EntryDataDetails.EntryDataDetailsKey
--                                  ) AS t 
--								 --full OUTER JOIN
--								 left outer join
--								(SELECT EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
--                                       FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
--									   inner join Customs_Procedure WITH (NOLOCK) ON Customs_Procedure.CustomsProcedure = AsycudaDocumentItemEntryDataDetails_1.CustomsProcedure
--									   where Customs_Procedure.Discrepancy = 1 
--                                      -- WHERE  (DocumentType NOT IN ('IM7', 'EX9')
--									   ) AS AsycudaDocumentItemEntryDataDetails  ON t.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
--									 where (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL )
--									 ) AS xx
--where (EntryDataDetailsId IS NOT NULL) 
--GROUP BY EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, AdjustmentType, InvoiceNo, InvoiceQty, IsClassified, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, 
--                 Declarant_Reference_Number, EmailId, EntryDataDetailsKey
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[48] 4[13] 2[7] 3) )"
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
               Right = 410
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 19
               Left = 461
               Bottom = 174
               Right = 699
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 736
               Bottom = 140
               Right = 991
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 14
               Left = 1059
               Bottom = 169
               Right = 1347
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemsEx"
            Begin Extent = 
               Top = 149
               Left = 0
               Bottom = 292
               Right = 238
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 256
               Left = 504
               Bottom = 423
               Right = 758
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 132
               Left = 228
               Bottom = 245
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-AdjustmentsToXML'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'               Right = 428
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 278
               Left = 817
               Bottom = 370
               Right = 1017
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 180
               Left = 1209
               Bottom = 335
               Right = 1547
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 95
               Left = 847
               Bottom = 250
               Right = 1070
            End
            DisplayFlags = 280
            TopColumn = 6
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
         Width = 2003
         Width = 1885
         Width = 2108
         Width = 1309
         Width = 2239
         Width = 1689
         Width = 1309
         Width = 1309
         Width = 2317
         Width = 2854
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-AdjustmentsToXML'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-AdjustmentsToXML'
GO
