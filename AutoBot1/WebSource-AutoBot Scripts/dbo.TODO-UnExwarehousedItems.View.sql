USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UnExwarehousedItems]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-UnExwarehousedItems]
AS
SELECT DATENAME(MONTH, EX9AsycudaSalesAllocations.InvoiceDate) + '-' + DATENAME(YEAR, EX9AsycudaSalesAllocations.InvoiceDate) AS MonthYear, EX9AsycudaSalesAllocations.ItemNumber, 
                 EX9AsycudaSalesAllocations.ItemDescription, ApplicationSettings.ApplicationSettingsId, SUM(EX9AsycudaSalesAllocations.QtyAllocated) AS QtyAllocated, EX9AsycudaSalesAllocations.Status, 
                 EX9AsycudaSalesAllocations.xStatus, EX9AsycudaSalesAllocations.pReferenceNumber, EX9AsycudaSalesAllocations.pLineNumber, EX9AsycudaSalesAllocations.pCNumber, EX9AsycudaSalesAllocations.EmailId, 
                 sum(AsycudaItemPiQuantityData.PiQuantity) AS PiQuantity
FROM    EX9AsycudaSalesAllocations INNER JOIN
                 ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                 EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                 AsycudaItemPiQuantityData ON EX9AsycudaSalesAllocations.PreviousItem_Id = AsycudaItemPiQuantityData.Item_Id AND DATENAME(MONTH, EX9AsycudaSalesAllocations.InvoiceDate) + '-' + DATENAME(YEAR, 
                 EX9AsycudaSalesAllocations.InvoiceDate) = DATENAME(MONTH, AsycudaItemPiQuantityData.AssessmentDate) + '-' + DATENAME(YEAR, AsycudaItemPiQuantityData.AssessmentDate) AND 
                 ApplicationSettings.ApplicationSettingsId = AsycudaItemPiQuantityData.ApplicationSettingsId AND 
                 EX9AsycudaSalesAllocations.ApplicationSettingsId = AsycudaItemPiQuantityData.ApplicationSettingsId LEFT OUTER JOIN
                 AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.Customs_ProcedureId = EX9AsycudaSalesAllocations.Customs_ProcedureId
WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                 (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                 EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                 (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) 
				 and (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse')
				-- AND (EX9AsycudaSalesAllocations.DocumentType = 'IM7' OR   EX9AsycudaSalesAllocations.DocumentType = 'OS7') 
				 AND (AllocationErrors.ItemNumber IS NULL)
GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId, EX9AsycudaSalesAllocations.InvoiceDate, EX9AsycudaSalesAllocations.Status, EX9AsycudaSalesAllocations.xStatus, EX9AsycudaSalesAllocations.ItemDescription, EX9AsycudaSalesAllocations.pReferenceNumber, 
                 EX9AsycudaSalesAllocations.pLineNumber, EX9AsycudaSalesAllocations.pCNumber, EX9AsycudaSalesAllocations.EmailId,AsycudaItemPiQuantityData.AssessmentDate--, AsycudaItemPiQuantityData.PiQuantity
HAVING (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL) AND (sum(AsycudaItemPiQuantityData.PiQuantity) < SUM(EX9AsycudaSalesAllocations.QtyAllocated))
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
         Begin Table = "EX9AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 302
               Right = 342
            End
            DisplayFlags = 280
            TopColumn = 46
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 386
               Bottom = 161
               Right = 752
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AllocationErrors"
            Begin Extent = 
               Top = 6
               Left = 796
               Bottom = 140
               Right = 1034
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
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 10159
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnExwarehousedItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnExwarehousedItems'
GO
