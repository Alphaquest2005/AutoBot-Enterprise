USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[C71Summary]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[C71Summary]
AS
SELECT MAX(Address) AS Address,isnull(MAX(Value_declaration_form_Id),0) AS Value_declaration_form_Id, Total, MAX(RegisteredId) AS RegisteredId, MAX(Reference) as Reference, MAX(SourceFile) as SourceFile, Max(RegNumber) as RegNumber, max(DocumentReference) as DocumentReference, 
                 max(ApplicationSettingsId) as ApplicationSettingsId
FROM    (SELECT xC71_Seller_segment.Address, xC71_Value_declaration_form.Value_declaration_form_Id, SUM(CAST(xC71_Item.Net_Price AS float)) AS Total, 
                                  MAX(xC71_Value_declaration_form_Registered.Value_declaration_form_Id) AS RegisteredId, CASE WHEN LEFT(xC71_Seller_segment.Address, 4) = 'Ref:' THEN substring(xC71_Seller_segment.Address, 5, 
                                  CHARINDEX(',', xC71_Seller_segment.Address) - 5) ELSE '' END AS Reference, xC71_Value_declaration_form_Registered.SourceFile, xC71_Value_declaration_form_Registered.RegNumber, 
                                  xC71_Value_declaration_form_Registered.DocumentReference, xC71_Value_declaration_form_Registered.ApplicationSettingsId
                 FROM     xC71_Seller_segment INNER JOIN
                                  xC71_Identification_segment ON xC71_Identification_segment.Identification_segment_Id = xC71_Seller_segment.Identification_segment_Id INNER JOIN
                                  xC71_Value_declaration_form ON xC71_Identification_segment.Identification_segment_Id = xC71_Value_declaration_form.Value_declaration_form_Id INNER JOIN
                                  xC71_Item ON xC71_Value_declaration_form.Value_declaration_form_Id = xC71_Item.Value_declaration_form_Id LEFT OUTER JOIN
                                  xC71_Value_declaration_form_Registered ON xC71_Value_declaration_form.Value_declaration_form_Id = xC71_Value_declaration_form_Registered.Value_declaration_form_Id
                 GROUP BY xC71_Seller_segment.Address, xC71_Value_declaration_form.Value_declaration_form_Id, xC71_Value_declaration_form_Registered.SourceFile, xC71_Value_declaration_form_Registered.RegNumber, 
                                  xC71_Value_declaration_form_Registered.DocumentReference, xC71_Value_declaration_form_Registered.ApplicationSettingsId) AS t
GROUP BY Reference, Total
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
         Begin Table = "xC71_Seller_segment"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 309
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xC71_Identification_segment"
            Begin Extent = 
               Top = 6
               Left = 353
               Bottom = 161
               Right = 629
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xC71_Value_declaration_form"
            Begin Extent = 
               Top = 6
               Left = 673
               Bottom = 98
               Right = 942
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xC71_Value_declaration_form_Registered"
            Begin Extent = 
               Top = 6
               Left = 986
               Bottom = 161
               Right = 1255
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xC71_Item"
            Begin Extent = 
               Top = 102
               Left = 673
               Bottom = 257
               Right = 942
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
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'C71Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'C71Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'C71Summary'
GO
