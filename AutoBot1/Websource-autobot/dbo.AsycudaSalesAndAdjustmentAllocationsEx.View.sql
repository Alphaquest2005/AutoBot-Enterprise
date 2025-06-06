USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaSalesAndAdjustmentAllocationsEx]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[AsycudaSalesAndAdjustmentAllocationsEx]
AS
SELECT TOP (100) PERCENT  TotalValue, AllocatedValue, Status,  QtyAllocated,SANumber,   InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo,SalesLineNumber, ItemNumber, 
                 ItemDescription,  DutyFreePaid,Cost, Total_CIF_itm, DutyLiability, TaxAmount,SalesFactor, pReferenceNumber,pRegistrationDate,pCNumber,pLineNumber,  pQuantity, pQtyAllocated, PiQuantity,pExpiryDate, pTariffCode,   
                    pIsAssessed, DoNotAllocateSales, DoNotAllocatePreviousEntry, WarehouseError,   TariffCode, 
                 Invalid,  pItemNumber, Type,AssessmentDate,xStatus,xReferenceNumber, xCNumber,xLineNumber, xRegistrationDate, xQuantity,AllocationId,EntryDataDetailsId, PreviousItem_Id, xBond_Item_Id,xASYCUDA_Id, pASYCUDA_Id, ApplicationSettingsId
FROM    (SELECT AllocationId, TotalValue, AllocatedValue, Status, xStatus, QtyAllocated, xLineNumber, PreviousItem_Id, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo,SalesLineNumber, ItemNumber, ItemDescription, 
                                  EntryDataDetailsId, xBond_Item_Id, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, pQtyAllocated, PiQuantity, SalesFactor, xCNumber, xRegistrationDate, xQuantity, pReferenceNumber, pLineNumber, 
                                  xASYCUDA_Id, pASYCUDA_Id, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, DoNotAllocatePreviousEntry, WarehouseError, SANumber, xReferenceNumber, TariffCode, 
                                  Invalid, pExpiryDate, pTariffCode, pItemNumber, 'Sale' AS Type,AssessmentDate, ApplicationSettingsId
                 FROM     dbo.AsycudaSalesAllocationsEx
                 UNION
                 SELECT AllocationId, TotalValue, AllocatedValue, Status, xStatus, QtyAllocated, xLineNumber, PreviousItem_Id, InvoiceDate, Comment AS CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, SalesLineNumber, ItemNumber, 
                                  ItemDescription, EntryDataDetailsId, xBond_Item_Id, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, pQtyAllocated, PiQuantity, SalesFactor, xCNumber, xRegistrationDate, xQuantity, 
                                  pReferenceNumber, pLineNumber, xASYCUDA_Id, pASYCUDA_Id, Cost, Total_CIF_itm, DutyLiability, 0 AS TaxAmount, pIsAssessed, DoNotAllocateSales, DoNotAllocatePreviousEntry, WarehouseError, 
                                  SANumber, xReferenceNumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, Type,AssessmentDate, ApplicationSettingsId
                 FROM    dbo.AdjustmentShortAllocations) AS t
ORDER BY AllocationId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[20] 4[19] 2[32] 3) )"
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
         Begin Table = "t"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 310
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
      Begin ColumnWidths = 46
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
         Or =' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAndAdjustmentAllocationsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAndAdjustmentAllocationsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAndAdjustmentAllocationsEx'
GO
