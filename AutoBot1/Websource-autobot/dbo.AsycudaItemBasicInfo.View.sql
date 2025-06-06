USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaItemBasicInfo]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[AsycudaItemBasicInfo]
 
AS
SELECT        xcuda_Item.ASYCUDA_Id, xcuda_Item.Item_Id, xcuda_HScode.Precision_4 AS ItemNumber, xcuda_Goods_description.Commercial_Description, xcuda_HScode.Commodity_code AS TariffCode, 
                         Primary_Supplementary_Unit.Suppplementary_unit_quantity AS ItemQuantity, xcuda_Item.DPQtyAllocated, xcuda_Item.DFQtyAllocated, xcuda_Item.IsAssessed, xcuda_Item.LineNumber, 
                         xcuda_Registration.Number AS CNumber, xcuda_Registration.Date AS RegistrationDate, xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, [ApplicationSettings-Declarants].ApplicationSettingsId, EntryDataType
FROM            xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) INNER JOIN
                         xcuda_Registration WITH (NOLOCK) ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
                         xcuda_Declarant ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id INNER JOIN
                         [ApplicationSettings-Declarants] ON xcuda_Declarant.Declarant_code = [ApplicationSettings-Declarants].DeclarantCode RIGHT OUTER JOIN
                         xcuda_Item WITH (NOLOCK) ON xcuda_Registration.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                         Primary_Supplementary_Unit WITH (NOLOCK) ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id LEFT OUTER JOIN
                         xcuda_HScode WITH (NOLOCK) ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id LEFT OUTER JOIN
                         xcuda_Goods_description WITH (NOLOCK) ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id
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
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 308
               Right = 301
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 47
               Left = 894
               Bottom = 198
               Right = 1145
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 150
               Left = 521
               Bottom = 280
               Right = 705
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 0
               Left = 718
               Bottom = 134
               Right = 902
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
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 903
         Table = 2893
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1348
         Or = 1350
         Or = ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemBasicInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemBasicInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaItemBasicInfo'
GO
