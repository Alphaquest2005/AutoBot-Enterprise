USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetAttachements]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-PODocSetAttachements]
AS
SELECT dbo.AsycudaDocumentAttachments.ASYCUDA_Id, dbo.AsycudaDocumentAttachments.DocumentCode, dbo.xcuda_Attached_documents.Item_Id, dbo.AsycudaDocumentAttachments.Id AS AttachementId, 
                 AsycudaDocument.AsycudaDocumentSetId, AsycudaDocument.Reference as ReferenceNumber, EntryData.EntryData_Id, EntryDataId as InvoiceNo
FROM    dbo.AsycudaDocumentAttachments INNER JOIN
                 dbo.xcuda_Attached_documents ON dbo.AsycudaDocumentAttachments.Reference = dbo.xcuda_Attached_documents.Attached_document_reference INNER JOIN
                 dbo.xcuda_Item ON dbo.xcuda_Attached_documents.Item_Id = dbo.xcuda_Item.Item_Id AND dbo.AsycudaDocumentAttachments.ASYCUDA_Id = dbo.xcuda_Item.ASYCUDA_Id RIGHT OUTER JOIN
                 AsycudaDocumentBasicInfo as AsycudaDocument ON dbo.AsycudaDocumentAttachments.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
				 inner join 				
                 AsycudaDocument_Attachments on AsycudaDocumentAttachments.ASYCUDA_Id = AsycudaDocument_Attachments.AsycudaDocumentId 
				 inner join Attachments
				 ON  Attachments.Id = AsycudaDocument_Attachments.AttachmentId INNER JOIN
                 EntryData INNER JOIN
                 AsycudaDocumentEntryData ON EntryData.EntryData_Id = AsycudaDocumentEntryData.EntryData_Id ON 
                 AsycudaDocument_Attachments.AsycudaDocumentId = AsycudaDocumentEntryData.AsycudaDocumentId AND replace(Attachments.FilePath,'.pdf','.csv') = EntryData.SourceFile and Attachments.FilePath like '%'+entrydata.EntryDataId + '%'
WHERE (dbo.AsycudaDocumentAttachments.DocumentCode IN ('IV05', 'BL10', 'IV04')) AND (AsycudaDocument.ImportComplete = 0)
GROUP BY dbo.AsycudaDocumentAttachments.ASYCUDA_Id, dbo.AsycudaDocumentAttachments.DocumentCode, dbo.xcuda_Attached_documents.Item_Id, dbo.AsycudaDocumentAttachments.Id, 
                 AsycudaDocument.AsycudaDocumentSetId, AsycudaDocument.Reference, AsycudaDocument.ImportComplete, EntryData.EntryData_Id, EntryDataId
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
         Begin Table = "AsycudaDocumentAttachments"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 262
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Attached_documents"
            Begin Extent = 
               Top = 6
               Left = 306
               Bottom = 161
               Right = 602
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 646
               Bottom = 161
               Right = 953
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 6
               Left = 997
               Bottom = 161
               Right = 1307
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
         GroupBy = 1350
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetAttachements'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetAttachements'
GO
