USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaSalesAllocationsPIData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaSalesAllocationsPIData]
AS

-- actual pi data based on xitem in asycudaSalesAllocations
SELECT dbo.AsycudaSalesAllocations.AllocationId, XBondItem.LineNumber AS xLineNumber, XBondItem.Item_Id AS xBond_Item_Id, xReg.Number AS xCNumber, CAST(xReg.Date AS DateTime) AS xRegistrationDate, 
                 XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xDeclarant.Number AS xReferenceNumber, cast(ROW_NUMBER() OVER (ORDER BY dbo.AsycudaSalesAllocations.AllocationId,
                  XBondItem.Item_Id) AS int) AS Id
FROM    dbo.Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Registration AS xReg WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item AS XBondItem WITH (NOLOCK) ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id INNER JOIN
                 dbo.PreviousItemsEx ON XBondItem.Item_Id = dbo.PreviousItemsEx.AsycudaDocumentItemId AND XBondItem_SuppUnit.Suppplementary_unit_quantity = dbo.PreviousItemsEx.Suplementary_Quantity AND 
                 XBondItem.LineNumber = dbo.PreviousItemsEx.Current_item_number AND xDeclarant.ASYCUDA_Id = dbo.PreviousItemsEx.ASYCUDA_Id INNER JOIN
                 dbo.AsycudaSalesAllocations ON dbo.PreviousItemsEx.PreviousDocumentItemId = dbo.AsycudaSalesAllocations.PreviousItem_Id and XBondItem.Item_Id = AsycudaSalesAllocations.xEntryItem_Id
where AsycudaSalesAllocations.xEntryItem_Id is not null

-- guess the pi data
/*
SELECT dbo.AsycudaSalesAllocations.AllocationId, XBondItem.LineNumber AS xLineNumber, XBondItem.Item_Id AS xBond_Item_Id, xReg.Number AS xCNumber, CAST(xReg.Date AS DateTime) AS xRegistrationDate, 
                 XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xDeclarant.Number AS xReferenceNumber, cast(ROW_NUMBER() OVER (ORDER BY dbo.AsycudaSalesAllocations.AllocationId,
                  XBondItem.Item_Id) AS int) AS Id
FROM    dbo.Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Registration AS xReg WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item AS XBondItem WITH (NOLOCK) ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id INNER JOIN
                 dbo.PreviousItemsEx ON XBondItem.Item_Id = dbo.PreviousItemsEx.AsycudaDocumentItemId AND XBondItem_SuppUnit.Suppplementary_unit_quantity = dbo.PreviousItemsEx.Suplementary_Quantity AND 
                 XBondItem.LineNumber = dbo.PreviousItemsEx.Current_item_number AND xDeclarant.ASYCUDA_Id = dbo.PreviousItemsEx.ASYCUDA_Id INNER JOIN
                 dbo.AsycudaSalesAllocations ON dbo.PreviousItemsEx.PreviousDocumentItemId = dbo.AsycudaSalesAllocations.PreviousItem_Id INNER JOIN
                 dbo.AsycudaDocumentItemEntryDataDetails ON dbo.AsycudaSalesAllocations.EntryDataDetailsId = dbo.AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId AND 
                 XBondItem.Item_Id = dbo.AsycudaDocumentItemEntryDataDetails.Item_Id and XBondItem_SuppUnit.Suppplementary_unit_quantity = dbo.AsycudaSalesAllocations.QtyAllocated-- and XBondItem.Item_Id = AsycudaSalesAllocations.xEntryItem_Id
where AsycudaSalesAllocations.xEntryItem_Id is null */
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
         Begin Table = "XBondItem_SuppUnit"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 324
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xReg"
            Begin Extent = 
               Top = 6
               Left = 368
               Bottom = 140
               Right = 552
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "XBondItem"
            Begin Extent = 
               Top = 6
               Left = 596
               Bottom = 161
               Right = 887
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xDeclarant"
            Begin Extent = 
               Top = 6
               Left = 931
               Bottom = 161
               Right = 1175
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PreviousItemsEx"
            Begin Extent = 
               Top = 6
               Left = 1219
               Bottom = 161
               Right = 1517
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 1561
               Bottom = 161
               Right = 1768
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 317
             ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocationsPIData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  Right = 283
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocationsPIData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocationsPIData'
GO
