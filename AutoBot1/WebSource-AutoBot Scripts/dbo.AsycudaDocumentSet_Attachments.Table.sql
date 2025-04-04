USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocumentSet_Attachments]    Script Date: 4/3/2025 10:23:54 PM ******/
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

INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53966, 123530, 5381, 1, CAST(N'2025-03-25' AS Date), 1183, NULL)
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53940, 123474, 5428, 0, CAST(N'2025-04-03' AS Date), 1144, N'waybill sample--2025-03-16-20:05:21')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53942, 123476, 5428, 0, CAST(N'2025-04-03' AS Date), 1144, N'sample shipment--2025-03-17-14:06:34')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53941, 123475, 5428, 0, CAST(N'2025-04-03' AS Date), 1148, N'waybill sample--2025-03-16-20:05:21')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53943, 123477, 5428, 0, CAST(N'2025-04-03' AS Date), 1148, N'sample shipment--2025-03-17-14:06:34')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53949, 123489, 5428, 0, CAST(N'2025-04-03' AS Date), 1148, N'Unknown PDF Found--2025-04-03-20:15:02')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53950, 123490, 5428, 0, CAST(N'2025-04-03' AS Date), 1148, N'Unknown PDF Found--2025-04-03-20:18:59')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53963, 123509, 7847, 1, CAST(N'2025-04-03' AS Date), 92, NULL)
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53944, 123484, 7847, 0, CAST(N'2025-04-03' AS Date), 100, N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53951, 123497, 7847, 0, CAST(N'2025-04-03' AS Date), 100, N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53947, 123487, 7847, 0, CAST(N'2025-04-03' AS Date), 109, N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53954, 123500, 7847, 0, CAST(N'2025-04-03' AS Date), 109, N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53945, 123485, 7847, 1, CAST(N'2025-04-03' AS Date), 111, N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53952, 123498, 7847, 1, CAST(N'2025-04-03' AS Date), 111, N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53946, 123486, 7847, 1, CAST(N'2025-04-03' AS Date), 1151, N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53953, 123499, 7847, 1, CAST(N'2025-04-03' AS Date), 1151, N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53964, 123513, 7849, 1, CAST(N'2025-04-03' AS Date), 92, NULL)
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53955, 123501, 7849, 0, CAST(N'2025-04-03' AS Date), 100, N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53958, 123504, 7849, 0, CAST(N'2025-04-03' AS Date), 109, N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53956, 123502, 7849, 1, CAST(N'2025-04-03' AS Date), 111, N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53957, 123503, 7849, 1, CAST(N'2025-04-03' AS Date), 1151, N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53965, 123517, 7850, 1, CAST(N'2025-04-03' AS Date), 92, NULL)
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53959, 123505, 7850, 0, CAST(N'2025-04-03' AS Date), 100, N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53962, 123508, 7850, 0, CAST(N'2025-04-03' AS Date), 109, N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53960, 123506, 7850, 1, CAST(N'2025-04-03' AS Date), 111, N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[AsycudaDocumentSet_Attachments] ([Id], [AttachmentId], [AsycudaDocumentSetId], [DocumentSpecific], [FileDate], [FileTypeId], [EmailId]) VALUES (53961, 123507, 7850, 1, CAST(N'2025-04-03' AS Date), 1151, N'Shipment: HAWB9596948--2025-04-03-21:34:36')
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet_Attachments] OFF
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_1215_1214]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_1215_1214] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AttachmentId] ASC,
	[AsycudaDocumentSetId] ASC
)
INCLUDE([DocumentSpecific],[FileDate],[FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_1221_1220]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_1221_1220] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AttachmentId] ASC
)
INCLUDE([FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_20_19]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_20_19] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AsycudaDocumentSetId] ASC,
	[FileTypeId] ASC
)
INCLUDE([AttachmentId],[DocumentSpecific],[FileDate],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_24_23]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSet_Attachments_24_23] ON [dbo].[AsycudaDocumentSet_Attachments]
(
	[AsycudaDocumentSetId] ASC,
	[FileTypeId] ASC
)
INCLUDE([AttachmentId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSet_Attachments_26_25]    Script Date: 4/3/2025 10:23:55 PM ******/
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
