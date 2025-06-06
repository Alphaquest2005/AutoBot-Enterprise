USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[AdjustmentShortAllocations]
AS
SELECT allo.AllocationId, Adj.TotalValue, Allo.QtyAllocated * Adj.Cost AS AllocatedValue, 
                 allo.Status, Allo.QtyAllocated, xLineNumber, PreviousItem_Id, 
                 adj.InvoiceDate, adj. SalesQuantity, adj.SalesQtyAllocated, InvoiceNo,  SalesLineNumber, 
                 adj.ItemNumber, ItemDescription, adj.EntryDataDetailsId, ISNULL(xBond_Item_Id, 0) as xBond_Item_Id, pCNumber, 
                 pRegistrationDate, pQuantity, pQtyAllocated, 
                 PiQuantity, pItem.SalesFactor, xCNumber, xRegistrationDate, 
                 xQuantity, pReferenceNumber, pLineNumber, xASYCUDA_Id, 
                 pASYCUDA_Id, adj.Cost, Total_CIF_itm, DutyLiability, pIsAssessed, 
                 DoNotAllocateSales, DoNotAllocatePreviousEntry, pItem.WarehouseError, xReferenceNumber, TariffCode, 
                 Invalid, pExpiryDate, pTariffCode, pPrecision1, pItemNumber, adj.EffectiveDate, adj.Comment, AsycudaDocumentSetId, 
                 pitem.AssessmentDate, 
                 DutyFreePaid, adj.ApplicationSettingsId, adj.FileTypeId, adj.EmailId,allo.xStatus, adj.Type, adj.SANumber
FROM    dbo.AsycudaSalesAllocations as Allo INNER JOIN
                 dbo.[AdjustmentShortAllocations-Adjustments] as Adj ON allo.EntryDataDetailsId = Adj.EntryDataDetailsId LEFT OUTER JOIN
                 dbo.[AdjustmentShortAllocations-pDocumentItem] as pItem ON allo.PreviousItem_Id = pItem.Item_Id LEFT OUTER JOIN
                 dbo.[AdjustmentShortAllocations-xDocumentItem] as xItem ON allo.AllocationId = xItem.AllocationId
---ORDER BY allo.AllocationId
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
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 209
               Right = 315
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentShortAllocations-Adjustments"
            Begin Extent = 
               Top = 4
               Left = 399
               Bottom = 309
               Right = 720
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentShortAllocations-xDocumentItem"
            Begin Extent = 
               Top = 16
               Left = 1178
               Bottom = 171
               Right = 1694
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentShortAllocations-pDocumentItem"
            Begin Extent = 
               Top = 9
               Left = 788
               Bottom = 300
               Right = 1143
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
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations'
GO
