--USE [BudgetMarine-AutoBot]
GO
SET IDENTITY_INSERT [dbo].[FileTypes] ON 
Go
INSERT [dbo].[FileTypes] ([Id], [ApplicationSettingsId], [FilePattern], [Type], [AsycudaDocumentSetId], [CreateDocumentSet], [DocumentSpecific], [DocumentCode], [ReplyToMail], [FileGroupId], [MergeEmails], [CopyEntryData], [ParentFileTypeId], [OverwriteFiles], [HasFiles], [OldFileTypeId], [ReplicateHeaderRow], [IsImportable], [MaxFileSizeInMB], [FileInfoId]) VALUES (1168, 2, N'Sales-\w+.csv', N'xSales', 3, 0, 0, N'DSF1', 0, NULL, 0, 1, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[FileTypes] OFF
GO

