USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[LicenceSummary]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[LicenceSummary]
AS
SELECT AsycudaDocumentItem.TariffCode, SUM(AsycudaDocumentItem.ItemQuantity) AS Quantity, /*CAST(SUM(Item_price) AS float)*/0 AS Total, 
             dbo.TariffCodes.Description as TariffCodeDescription,dbo.AsycudaDocumentSet.ApplicationSettingsId, dbo.AsycudaDocumentSet.AsycudaDocumentSetId, ROW_NUMBER() OVER (ORDER BY AsycudaDocumentSet.AsycudaDocumentSetId)  AS RowNumber
FROM   dbo.xcuda_ASYCUDA_ExtendedProperties INNER JOIN
             dbo.AsycudaDocumentSet ON dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = dbo.AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
           AsycudaItemBasicInfo as AsycudaDocumentItem ON dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentItem.Asycuda_Id
		   inner join TariffCodes on AsycudaDocumentItem.TariffCode = TariffCodes.TariffCode
		   /*inner join xcuda_Tarification on AsycudaDocumentItem.Item_Id = xcuda_Tarification.Item_Id*/
where     (TariffCodes.LicenseRequired = 1)
GROUP BY AsycudaDocumentItem.TariffCode, dbo.TariffCodes.Description, dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.TariffCodes.LicenseRequired, 
            dbo.AsycudaDocumentSet.ApplicationSettingsId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[16] 4[46] 2[20] 3) )"
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
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 214
               Right = 379
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 106
               Left = 422
               Bottom = 311
               Right = 772
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItem"
            Begin Extent = 
               Top = 9
               Left = 843
               Bottom = 214
               Right = 1202
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
         Width = 1000
         Width = 1000
         Width = 1720
         Width = 3820
         Width = 2800
         Width = 1000
         Width = 1000
         Width = 1000
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 7150
         Alias = 1420
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LicenceSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LicenceSummary'
GO
