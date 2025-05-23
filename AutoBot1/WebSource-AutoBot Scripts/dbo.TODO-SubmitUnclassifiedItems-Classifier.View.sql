USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitUnclassifiedItems-Classifier]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-SubmitUnclassifiedItems-Classifier]
AS
select * from 
(select *, row_number() over(partition by itemnumber order by[Rank] desc) as row1 from 
(SELECT dbo.[TODO-SubmitUnclassifiedItems].InvoiceNo, dbo.[TODO-SubmitUnclassifiedItems].LineNumber, dbo.[TODO-SubmitUnclassifiedItems].ItemNumber, dbo.[TODO-SubmitUnclassifiedItems].ItemDescription, 
                 dbo.[TODO-SubmitUnclassifiedItems].EmailId, dbo.[TODO-SubmitUnclassifiedItems].Type, dbo.[TODO-SubmitUnclassifiedItems].AsycudaDocumentSetId, 
                 dbo.[TODO-SubmitUnclassifiedItems].Declarant_Reference_Number, dbo.[TODO-SubmitUnclassifiedItems].ApplicationSettingsId, dbo.TariffCodes.TariffCode, KeyWords.Keyword, KeyWords.IsException,
				 (RANk() over (Partition by itemnumber order by isnull(keywords.keyword, exceptions.keyword)))  as Rank
FROM    dbo.TariffCodes INNER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     dbo.TariffKeyWords
                      WHERE  (IsException = 0)) AS KeyWords ON dbo.TariffCodes.TariffCode = KeyWords.TariffCode LEFT OUTER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     dbo.TariffKeyWords AS TariffKeyWords_1
                      WHERE  (IsException = 1)) AS Exceptions ON dbo.TariffCodes.TariffCode = Exceptions.TariffCode RIGHT OUTER JOIN
                 dbo.[TODO-SubmitUnclassifiedItems] ON (Exceptions.Keyword IS NULL OR
                 ISNULL(Exceptions.ApplicationSettingsId, dbo.[TODO-SubmitUnclassifiedItems].ApplicationSettingsId) = dbo.[TODO-SubmitUnclassifiedItems].ApplicationSettingsId AND CHARINDEX(Exceptions.Keyword, 
                 dbo.[TODO-SubmitUnclassifiedItems].ItemDescription) <= 0) AND ISNULL(KeyWords.ApplicationSettingsId, dbo.[TODO-SubmitUnclassifiedItems].ApplicationSettingsId) 
                 = dbo.[TODO-SubmitUnclassifiedItems].ApplicationSettingsId AND CHARINDEX(KeyWords.Keyword, dbo.[TODO-SubmitUnclassifiedItems].ItemDescription) > 0) as t) as tt
where row1 = 1
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
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 277
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "KeyWords"
            Begin Extent = 
               Top = 6
               Left = 321
               Bottom = 161
               Right = 543
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Exceptions"
            Begin Extent = 
               Top = 6
               Left = 587
               Bottom = 161
               Right = 809
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-SubmitUnclassifiedItems"
            Begin Extent = 
               Top = 6
               Left = 853
               Bottom = 161
               Right = 1125
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
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitUnclassifiedItems-Classifier'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitUnclassifiedItems-Classifier'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitUnclassifiedItems-Classifier'
GO
