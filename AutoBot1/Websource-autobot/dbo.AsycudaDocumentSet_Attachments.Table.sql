USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocumentSet_Attachments]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaDocumentSet_Attachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[AsycudaDocumentSetId] [int] NOT NULL,
	[DocumentSpecific] [bit] NOT NULL,
	[FileDate] [date] NOT NULL,
	[FileTypeId] [int] NULL,
	[EmailId] [nvarchar](255) NULL,
 CONSTRAINT [PK_AsycudaDocumentSet_Attachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet_Attachments] ON 

INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53916, 123339, 7841, 0, CAST(N'2025-03-26' AS Date), 100, N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53919, 123342, 7841, 0, CAST(N'2025-03-26' AS Date), 109, N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53917, 123340, 7841, 1, CAST(N'2025-03-26' AS Date), 111, N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53918, 123341, 7841, 1, CAST(N'2025-03-26' AS Date), 1151, N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53920, 123343, 7842, 0, CAST(N'2025-03-26' AS Date), 100, N'Shipment: HAWB9595459--2025-03-26-20:05:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53923, 123346, 7842, 0, CAST(N'2025-03-26' AS Date), 109, N'Shipment: HAWB9595459--2025-03-26-20:05:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53921, 123344, 7842, 1, CAST(N'2025-03-26' AS Date), 111, N'Shipment: HAWB9595459--2025-03-26-20:05:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53922, 123345, 7842, 1, CAST(N'2025-03-26' AS Date), 1151, N'Shipment: HAWB9595459--2025-03-26-20:05:53')
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet_Attachments] OFF
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_1215_1214]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_1215_1214] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AttachmentId] ASC,
	[AsycudaDocumentSetId] ASC
)
INCLUDE([DocumentSpecific],[FileDate],[FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_1221_1220]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_1221_1220] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AttachmentId] ASC
)
INCLUDE([FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_20_19]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_20_19] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AsycudaDocumentSetId] ASC,
	[FileTypeId] ASC
)
INCLUDE([AttachmentId],[DocumentSpecific],[FileDate],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_24_23]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_24_23] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AsycudaDocumentSetId] ASC,
	[FileTypeId] ASC
)
INCLUDE([AttachmentId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_26_25]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_26_25] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AsycudaDocumentSetId] ASC
)
INCLUDE([AttachmentId],[FileTypeId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Attachments_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Attachments_AsycudaDocumentSet]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Attachments_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Attachments_Attachments]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Attachments_Emails1] FOREIGN KEY([EmailId])
REFERENCES [dbo].[Emails] ([EmailId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Attachments_Emails1]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Attachments_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentSet_Attachments] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Attachments_FileTypes]
GO
