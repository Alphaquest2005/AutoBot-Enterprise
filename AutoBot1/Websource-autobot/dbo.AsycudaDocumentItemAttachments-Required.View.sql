USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemAttachments-Required]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaDocumentItemAttachments-Required]
AS
SELECT AsycudaDocumentBasicInfo.AsycudaDocumentSetId, AsycudaItemBasicInfo.ASYCUDA_Id, AsycudaItemBasicInfo.Item_Id, AsycudaDocumentBasicInfo.Reference, AsycudaItemBasicInfo.LineNumber, 
                 AsycudaItemBasicInfo.ItemNumber, xcuda_HScode.Commodity_code AS TariffCode, AttachmentCodes.Code, AttachmentCodes.Description, AsycudaDocumentBasicInfo.ImportComplete
FROM    TariffCodeRequiredAttachments INNER JOIN
                 AttachmentCodes ON TariffCodeRequiredAttachments.AttachmentCodeId = AttachmentCodes.Id INNER JOIN
                 xcuda_HScode INNER JOIN
                 AsycudaItemBasicInfo ON xcuda_HScode.Item_Id = AsycudaItemBasicInfo.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo ON AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 Customs_Procedure INNER JOIN
                 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id ON AsycudaDocumentBasicInfo.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId ON 
                 TariffCodeRequiredAttachments.TariffCode = xcuda_HScode.Commodity_code
WHERE (CustomsOperations.Name = N'Import' or CustomsOperations.Name = N'Warehouse') --AND (AsycudaDocumentBasicInfo.AsycudaDocumentSetId = 5411)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[20] 3) )"
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
         Begin Table = "TariffCodeRequiredAttachments"
            Begin Extent = 
               Top = 20
               Left = 449
               Bottom = 154
               Right = 658
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemBasicInfo"
            Begin Extent = 
               Top = 18
               Left = 921
               Bottom = 311
               Right = 1144
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AttachmentCodes"
            Begin Extent = 
               Top = 14
               Left = 140
               Bottom = 148
               Right = 324
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 21
               Left = 689
               Bottom = 176
               Right = 889
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 15
               Left = 1210
               Bottom = 226
               Right = 1484
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "CustomsOperations"
            Begin Extent = 
               Top = 182
               Left = 47
               Bottom = 295
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 193
               Left = 425
               ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemAttachments-Required'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'Bottom = 348
               Right = 699
            End
            DisplayFlags = 280
            TopColumn = 10
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
         Width = 1846
         Width = 1309
         Width = 1924
         Width = 1309
         Width = 1309
         Width = 1977
         Width = 1466
         Width = 1309
         Width = 2265
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
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemAttachments-Required'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemAttachments-Required'
GO
