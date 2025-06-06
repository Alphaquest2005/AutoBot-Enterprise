USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-DiscrepanciesToSubmit]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[TODO-DiscrepanciesToSubmit]
AS
SELECT EntryData_Id ,[TODO-AdjustmentsToXML].EntryDataDetailsId, [TODO-AdjustmentsToXML].ApplicationSettingsId, [TODO-AdjustmentsToXML].AsycudaDocumentSetId, [TODO-AdjustmentsToXML].IsClassified, 
                 [TODO-AdjustmentsToXML].AdjustmentType, [TODO-AdjustmentsToXML].InvoiceNo, [TODO-AdjustmentsToXML].InvoiceQty, [TODO-AdjustmentsToXML].ReceivedQty, [TODO-AdjustmentsToXML].InvoiceDate, 
                 [TODO-AdjustmentsToXML].ItemNumber, [TODO-AdjustmentsToXML].Status, [TODO-AdjustmentsToXML].CNumber, [TODO-AdjustmentsToXML].Declarant_Reference_Number, EntryDataDetailsEx.EmailId, 
                 EntryDataDetailsEx.FileTypeId, COALESCE (CASE WHEN (dbo.AdjustmentShortAllocations.EntryDataDetailsId IS NULL AND dbo.[TODO-AdjustmentsToXML].InvoiceQty > 0) THEN 'Short Not Found on Entry' ELSE NULL 
                 END, CASE WHEN (isnull(dbo.EntryDataDetailsEx.Cost, 0) = 0 AND dbo.[TODO-AdjustmentsToXML].InvoiceQty = 0) THEN 'Overage Cost is Zero' ELSE NULL END) AS Comment
FROM    [TODO-AdjustmentsToXML] INNER JOIN
                 EntryDataDetailsEx ON [TODO-AdjustmentsToXML].EntryDataDetailsId = EntryDataDetailsEx.EntryDataDetailsId LEFT OUTER JOIN
                 AdjustmentShortAllocations ON [TODO-AdjustmentsToXML].EntryDataDetailsId = AdjustmentShortAllocations.EntryDataDetailsId
WHERE (AdjustmentShortAllocations.EntryDataDetailsId IS NULL) OR ([TODO-AdjustmentsToXML].AdjustmentType = N'DIS')
               /*  (ISNULL(EntryDataDetailsEx.Cost, 0) = 0) and ([TODO-AdjustmentsToXML].AdjustmentType = N'DIS') AND ([TODO-AdjustmentsToXML].InvoiceQty > 0) OR
                  AND ([TODO-AdjustmentsToXML].InvoiceQty = 0)*/
GROUP BY [TODO-AdjustmentsToXML].EntryDataDetailsId, [TODO-AdjustmentsToXML].ApplicationSettingsId, [TODO-AdjustmentsToXML].AsycudaDocumentSetId, [TODO-AdjustmentsToXML].AdjustmentType, 
                 [TODO-AdjustmentsToXML].InvoiceNo, [TODO-AdjustmentsToXML].InvoiceQty, [TODO-AdjustmentsToXML].ReceivedQty, [TODO-AdjustmentsToXML].InvoiceDate, [TODO-AdjustmentsToXML].ItemNumber, 
                 [TODO-AdjustmentsToXML].Status, [TODO-AdjustmentsToXML].CNumber, [TODO-AdjustmentsToXML].Declarant_Reference_Number, EntryDataDetailsEx.EmailId, EntryDataDetailsEx.FileTypeId, [TODO-AdjustmentsToXML].IsClassified,
				 [TODO-AdjustmentsToXML].EntryDataDetailsId,AdjustmentShortAllocations.EntryDataDetailsId, EntryDataDetailsEx.Cost,EntryData_Id
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
         Begin Table = "TODO-AdjustmentsToXML"
            Begin Extent = 
               Top = 0
               Left = 162
               Bottom = 303
               Right = 434
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentShortAllocations"
            Begin Extent = 
               Top = 0
               Left = 652
               Bottom = 155
               Right = 918
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 144
               Left = 604
               Bottom = 299
               Right = 843
            End
            DisplayFlags = 280
            TopColumn = 27
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 13
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepanciesToSubmit'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepanciesToSubmit'
GO
