USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-LicenceAvailableQty]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[TODO-LicenceAvailableQty]
AS

SELECT licence.RegistrationNumber, licence.ApplicationSettingsId, LEFT(licence.Commodity_code, 8) AS TariffCode, licence.Origin, licence.Quantity_to_approve, CAST(licence.Application_date AS date) AS Application_date, 
                 CAST(licence.Importation_date AS date) AS Importation_date, licence.[Key], licence.Quantity_to_approve - ISNULL(used.Quantity_deducted_from_licence, 0) AS Balance, licence.LicenseId, licence.SourceFile, 
                 licence.DocumentReference,  licence.SegmentId
FROM    (SELECT DISTINCT 
                                  SUM(CAST(xcuda_Item.Quantity_deducted_from_licence AS int)) AS Quantity_deducted_from_licence, xcuda_HScode.Commodity_code, xcuda_Attached_documents.Attached_document_reference, 
                                  xcuda_Attached_documents.Attached_document_code
                 FROM     xcuda_Item INNER JOIN
                                  xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                                  xcuda_Attached_documents ON xcuda_Item.Item_Id = xcuda_Attached_documents.Item_Id
                 WHERE  (xcuda_Attached_documents.Attached_document_code = 'LC02')
                 GROUP BY xcuda_HScode.Commodity_code, xcuda_Attached_documents.Attached_document_reference, xcuda_Attached_documents.Attached_document_code
                 HAVING (SUM(CAST(xcuda_Item.Quantity_deducted_from_licence AS int)) IS NOT NULL)) AS used RIGHT OUTER JOIN
                     (SELECT xLIC_License_Registered.RegistrationNumber, xLIC_License_Registered.ApplicationSettingsId, xLIC_Lic_item_segment.Commodity_code, xLIC_Lic_item_segment.Origin, 
                                       xLIC_Lic_item_segment.Quantity_to_approve, xLIC_General_segment.Application_date, xLIC_General_segment.Importation_date, CAST(YEAR(xLIC_General_segment.Application_date) AS nvarchar(4)) 
                                       + ' ' + xLIC_License_Registered.RegistrationNumber AS [Key], xLIC_License_Registered.LicenseId, xLIC_License_Registered.SourceFile, xLIC_License_Registered.DocumentReference, 
                                       xLIC_Lic_item_segment.Id AS SegmentId
                      FROM     xLIC_License_Registered INNER JOIN
                                       xLIC_Lic_item_segment ON xLIC_License_Registered.LicenseId = xLIC_Lic_item_segment.LicenseId INNER JOIN
                                       xLIC_General_segment ON xLIC_License_Registered.LicenseId = xLIC_General_segment.General_segment_Id) AS licence ON used.Attached_document_reference = licence.[Key] AND 
                 LEFT(licence.Commodity_code, 8) = used.Commodity_code
WHERE (licence.Quantity_to_approve - ISNULL(used.Quantity_deducted_from_licence, 0) > 0) AND (licence.Application_date <= GETDATE()) AND (licence.Importation_date >= GETDATE())
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
         Begin Table = "used"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 335
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "licence"
            Begin Extent = 
               Top = 6
               Left = 379
               Bottom = 161
               Right = 601
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenceAvailableQty'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenceAvailableQty'
GO
