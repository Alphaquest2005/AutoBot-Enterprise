USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentFreight]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentFreight]
AS
SELECT dbo.AsycudaDocumentBasicInfo.CNumber, dbo.xcuda_Gs_external_freight.Amount_foreign_currency, dbo.xcuda_Gs_external_freight.Amount_national_currency, dbo.AsycudaDocumentBasicInfo.RegistrationDate, 
                 dbo.AsycudaDocumentBasicInfo.Reference
FROM    dbo.AsycudaDocumentBasicInfo INNER JOIN
                 dbo.xcuda_Gs_external_freight ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.xcuda_Gs_external_freight.Valuation_Id
WHERE (dbo.AsycudaDocumentBasicInfo.Reference LIKE N'February%') AND (dbo.AsycudaDocumentBasicInfo.RegistrationDate BETWEEN CONVERT(DATETIME, '2020-03-16 00:00:00', 102) AND CONVERT(DATETIME, 
                 '2021-03-19 00:00:00', 102) OR
                 dbo.AsycudaDocumentBasicInfo.RegistrationDate IS NULL)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[27] 4[17] 2[19] 3) )"
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
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 267
               Right = 318
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_Gs_external_freight"
            Begin Extent = 
               Top = 13
               Left = 545
               Bottom = 168
               Right = 801
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
         Width = 2343
         Width = 2448
         Width = 1309
         Width = 2736
         Width = 2945
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
         Filter = 5289
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentFreight'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentFreight'
GO
