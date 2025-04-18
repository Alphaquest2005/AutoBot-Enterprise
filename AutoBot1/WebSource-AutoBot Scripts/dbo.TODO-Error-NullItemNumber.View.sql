USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-NullItemNumber]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-Error-NullItemNumber]
AS
SELECT dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_Item.Item_Id, 'Null ItemNumber' AS Type, dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure, dbo.AsycudaDocumentBasicInfo.DocumentType, 
                 dbo.AsycudaDocumentBasicInfo.CNumber, dbo.AsycudaDocumentBasicInfo.Reference, dbo.AsycudaDocumentBasicInfo.ImportComplete, dbo.xcuda_Item.LineNumber, 
                 dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.xcuda_HScode.Commodity_code AS TariffCode, 
                 dbo.xcuda_Goods_description.Commercial_Description AS Description, dbo.xcuda_HScode.Precision_4 AS ItemNumber
FROM    dbo.xcuda_HScode WITH (Nolock) INNER JOIN
                 dbo.xcuda_Item WITH (Nolock) ON dbo.xcuda_HScode.Item_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_Goods_description ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (dbo.xcuda_HScode.Precision_4 IS NULL OR  (dbo.xcuda_HScode.Precision_4 = 'null'))  AND (dbo.AsycudaDocumentBasicInfo.ImportComplete = 1)  
			and (AsycudaDocumentCustomsProcedures.CustomsOperation <> 'Import')
--WHERE (dbo.xcuda_HScode.Precision_4 IS NULL) AND (dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure <> '4000') AND (dbo.AsycudaDocumentBasicInfo.ImportComplete = 1) OR
--                 (dbo.xcuda_HScode.Precision_4 = 'null') AND (dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure <> N'9000')
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
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 15
               Left = 449
               Bottom = 317
               Right = 740
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 27
               Left = 868
               Bottom = 301
               Right = 1142
            End
            DisplayFlags = 280
            TopColumn = 4
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 136
               Left = 82
               Bottom = 291
               Right = 322
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
      Begin ColumnWidths = 11
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1898
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1859
         Width = 1309
         Width = 1309
         Width = 1309
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
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-NullItemNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-NullItemNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-NullItemNumber'
GO
