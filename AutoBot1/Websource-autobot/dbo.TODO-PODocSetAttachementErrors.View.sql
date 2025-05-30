USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetAttachementErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-PODocSetAttachementErrors]
AS
SELECT [AsycudaDocumentAttachments-Required].AsycudaDocumentSetId, [AsycudaDocumentAttachments-Required].ASYCUDA_Id, [AsycudaDocumentAttachments-Required].Code, 
                 [AsycudaDocumentAttachments-Required].ReferenceNumber, [AsycudaDocumentAttachments-Required].ImportComplete, 
                 'Missing Required Attachment: ' + [AsycudaDocumentAttachments-Required].Description AS Status
FROM    [AsycudaDocumentAttachments-Required] LEFT OUTER JOIN
                 [AsycudaDocumentAttachments-Generated] ON [AsycudaDocumentAttachments-Required].AsycudaDocumentSetId = [AsycudaDocumentAttachments-Generated].AsycudaDocumentSetId AND 
                 [AsycudaDocumentAttachments-Required].ASYCUDA_Id = [AsycudaDocumentAttachments-Generated].ASYCUDA_Id AND 
                 [AsycudaDocumentAttachments-Required].Code = [AsycudaDocumentAttachments-Generated].DocumentCode
WHERE ([AsycudaDocumentAttachments-Generated].ASYCUDA_Id IS NULL) AND ([AsycudaDocumentAttachments-Required].ImportComplete = 0)
--ORDER BY [AsycudaDocumentAttachments-Required].ASYCUDA_Id

union

SELECT [AsycudaDocumentItemAttachments-Required].AsycudaDocumentSetId, [AsycudaDocumentItemAttachments-Required].ASYCUDA_Id, [AsycudaDocumentItemAttachments-Required].Code, 
                 [AsycudaDocumentItemAttachments-Required].Reference, [AsycudaDocumentItemAttachments-Required].ImportComplete, 
                 'Missing Required Attachment:: ' + [AsycudaDocumentItemAttachments-Required].TariffCode + '::' + [AsycudaDocumentItemAttachments-Required].Reference + ':' +cast( [AsycudaDocumentItemAttachments-Required].LineNumber  as nvarchar(3))+ '-'  + [AsycudaDocumentItemAttachments-Required].Description AS Status
FROM    [AsycudaDocumentItemAttachments-Generated] RIGHT OUTER JOIN
                 [AsycudaDocumentItemAttachments-Required] ON [AsycudaDocumentItemAttachments-Generated].AsycudaDocumentSetId = [AsycudaDocumentItemAttachments-Required].AsycudaDocumentSetId AND 
                 [AsycudaDocumentItemAttachments-Generated].ASYCUDA_Id = [AsycudaDocumentItemAttachments-Required].ASYCUDA_Id AND 
                 [AsycudaDocumentItemAttachments-Generated].Reference = [AsycudaDocumentItemAttachments-Required].Reference AND 
                 [AsycudaDocumentItemAttachments-Generated].Item_Id = [AsycudaDocumentItemAttachments-Required].Item_Id
WHERE ([AsycudaDocumentItemAttachments-Generated].ASYCUDA_Id IS NULL) AND ([AsycudaDocumentItemAttachments-Required].ImportComplete = 0)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[22] 4[27] 2[20] 3) )"
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
         Width = 2906
         Width = 1309
         Width = 5642
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
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetAttachementErrors'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetAttachementErrors'
GO
