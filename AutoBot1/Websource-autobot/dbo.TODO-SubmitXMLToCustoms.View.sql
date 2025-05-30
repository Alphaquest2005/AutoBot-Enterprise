USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitXMLToCustoms]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[TODO-SubmitXMLToCustoms]
AS

SELECT Id, CNumber, ASYCUDA_Id, RegistrationDate, ReferenceNumber, AsycudaDocumentSetId, DocumentType, AssessmentDate, ApplicationSettingsId, EmailId, CustomsProcedure, FilePath, Status, ToBePaid
FROM    [TODO-SubmitAllXMLToCustoms]
WHERE ((FilePath IS NULL) OR
                 (Status <> N'Submit XML To Customs')) --and SystemDocumentSetId is null


--SELECT ISNULL(CAST((row_number() OVER (ORDER BY dbo.AsycudaDocument.CNumber)) AS int), 0) as Id, AsycudaDocument.CNumber, AsycudaDocument.ASYCUDA_Id, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.AsycudaDocumentSetId, 
--                 AsycudaDocument.DocumentType, AsycudaDocument.AssessmentDate, AsycudaDocument.ApplicationSettingsId, EntryDataDetailsEx.EmailId, AsycudaDocument.CustomsProcedure, Attachments.FilePath, 
--                 AttachmentLog.Status
--FROM    AttachmentLog INNER JOIN
--                 AsycudaDocumentSet_Attachments ON AttachmentLog.DocSetAttachment = AsycudaDocumentSet_Attachments.Id INNER JOIN
--                 Attachments ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id RIGHT OUTER JOIN
--                 AsycudaDocumentItemEntryDataDetails INNER JOIN
--                 EntryDataDetailsEx ON AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = EntryDataDetailsEx.EntryDataDetailsId INNER JOIN
--                 xcuda_Item ON AsycudaDocumentItemEntryDataDetails.Item_Id = xcuda_Item.Item_Id INNER JOIN
--                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id ON Attachments.FilePath = AsycudaDocument.SourceFileName
--				 left outer join SystemDocumentSets on EntryDataDetailsEx.AsycudaDocumentSetId = SystemDocumentSets.Id
--WHERE (SystemDocumentSets.Id is null) and (AsycudaDocument.ImportComplete = 1) AND (AsycudaDocument.SubmitToCustoms = 1) AND ((Attachments.FilePath IS NULL) or (AttachmentLog.Status <> N'Submit XML To Customs'))
--GROUP BY EntryDataDetailsEx.EmailId, AsycudaDocument.CNumber, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.AsycudaDocumentSetId, AsycudaDocument.DocumentType, 
--                 AsycudaDocument.AssessmentDate, AsycudaDocument.ApplicationSettingsId, AsycudaDocument.Extended_customs_procedure, AsycudaDocument.ASYCUDA_Id, AttachmentLog.Status, Attachments.FilePath,AsycudaDocument.CustomsProcedure
----IN ('9074-000', '4074-000', '3075-000', '7400-OPS', '4074-801','7400-OPP')
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
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 6
               Left = 311
               Bottom = 161
               Right = 566
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 6
               Left = 610
               Bottom = 161
               Right = 917
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocument"
            Begin Extent = 
               Top = 6
               Left = 961
               Bottom = 161
               Right = 1271
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
     ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitXMLToCustoms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitXMLToCustoms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-SubmitXMLToCustoms'
GO
