USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-ExBondErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-Error-ExBondErrors]
AS
SELECT dbo.AsycudaSalesAndAdjustmentAllocationsEx.AllocationId, dbo.AsycudaSalesAndAdjustmentAllocationsEx.PreviousItem_Id, dbo.AsycudaSalesAndAdjustmentAllocationsEx.InvoiceDate, 
                 dbo.AsycudaSalesAndAdjustmentAllocationsEx.InvoiceNo, dbo.AsycudaSalesAndAdjustmentAllocationsEx.ItemNumber, dbo.AsycudaSalesAndAdjustmentAllocationsEx.EntryDataDetailsId, 
                 dbo.AsycudaSalesAndAdjustmentAllocationsEx.pCNumber, dbo.AsycudaSalesAndAdjustmentAllocationsEx.pRegistrationDate, dbo.AsycudaSalesAndAdjustmentAllocationsEx.pASYCUDA_Id, 
                 dbo.AsycudaSalesAndAdjustmentAllocationsEx.ApplicationSettingsId, dbo.AsycudaSalesAndAdjustmentAllocationsEx.xStatus, dbo.AsycudaSalesAndAdjustmentAllocationsEx.pReferenceNumber, 
                 dbo.AsycudaSalesAndAdjustmentAllocationsEx.pLineNumber, dbo.AsycudaSalesAndAdjustmentAllocationsEx.ItemDescription, dbo.AsycudaDocumentBasicInfo.DocumentType
FROM    dbo.AsycudaSalesAndAdjustmentAllocationsEx INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.AsycudaSalesAndAdjustmentAllocationsEx.pASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (dbo.AsycudaSalesAndAdjustmentAllocationsEx.xStatus IS NOT NULL)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[10] 3) )"
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
         Begin Table = "AsycudaSalesAndAdjustmentAllocationsEx"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 326
               Right = 310
            End
            DisplayFlags = 280
            TopColumn = 24
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 354
               Bottom = 161
               Right = 628
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
      Begin ColumnWidths = 12
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
         Width = 4333
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-ExBondErrors'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-ExBondErrors'
GO
