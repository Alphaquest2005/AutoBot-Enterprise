USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitUnclassifiedItems]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-SubmitUnclassifiedItems]
AS
SELECT EntryDataDetailsEx.EntryDataId AS InvoiceNo, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.TariffCode, EntryDataDetailsEx.EmailId, 
                 EntryDataEx.Type, AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.ApplicationSettingsId
FROM    EntryDataDetailsEx INNER JOIN
                 AsycudaDocumentSetEx ON EntryDataDetailsEx.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId AND 
                 EntryDataDetailsEx.ApplicationSettingsId = AsycudaDocumentSetEx.ApplicationSettingsId INNER JOIN
                 EntryDataEx ON EntryDataDetailsEx.EntryDataId = EntryDataEx.InvoiceNo AND EntryDataDetailsEx.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSetEx.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (SystemDocumentSets.Id IS NULL) and (EntryDataDetailsEx.TariffCode IS NULL) 
GROUP BY EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.TariffCode, EntryDataDetailsEx.EmailId, 
                 EntryDataEx.Type, AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.ApplicationSettingsId

--AND (EntryDataEx.Type = N'PO' OR             EntryDataEx.Type = N'DIS')
--SELECT EntryDataDetailsEx.EntryDataId AS InvoiceNo, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.TariffCode, EntryDataDetailsEx.EmailId, 
--                 EntryDataEx.Type, AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.ApplicationSettingsId
--FROM    EntryDataDetailsEx INNER JOIN
--                 AsycudaDocumentSetEx ON EntryDataDetailsEx.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId AND 
--                 EntryDataDetailsEx.ApplicationSettingsId = AsycudaDocumentSetEx.ApplicationSettingsId INNER JOIN
--                 EntryDataEx ON EntryDataDetailsEx.EntryDataId = EntryDataEx.InvoiceNo AND EntryDataDetailsEx.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId INNER JOIN
--                 [TODO-PODocSet] ON AsycudaDocumentSetEx.AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId LEFT OUTER JOIN
--                 SystemDocumentSets ON AsycudaDocumentSetEx.AsycudaDocumentSetId = SystemDocumentSets.Id
--WHERE (SystemDocumentSets.Id IS NULL)
--GROUP BY EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.TariffCode, EntryDataDetailsEx.EmailId, 
--                 EntryDataEx.Type, AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.ApplicationSettingsId
--HAVING (EntryDataDetailsEx.TariffCode IS NULL) AND (EntryDataEx.Type = N'PO' OR
--                 EntryDataEx.Type = N'DIS')
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[13] 3) )"
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
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 0
               Left = 917
               Bottom = 288
               Right = 1156
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "AsycudaDocumentSetEx"
            Begin Extent = 
               Top = 56
               Left = 377
               Bottom = 211
               Right = 649
            End
            DisplayFlags = 280
            TopColumn = 16
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 23
               Left = 1409
               Bottom = 276
               Right = 1648
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 98
               Right = 228
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
         Width = 2278
         Width = 3940
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 3757
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 2566
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitUnclassifiedItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Filter = 2631
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitUnclassifiedItems'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitUnclassifiedItems'
GO
