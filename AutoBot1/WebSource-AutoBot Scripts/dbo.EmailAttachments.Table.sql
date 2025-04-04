USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailAttachments]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailAttachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailId] [nvarchar](255) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[FileTypeId] [int] NULL,
	[DocumentSpecific] [bit] NOT NULL,
 CONSTRAINT [PK_EmailAttachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EmailAttachments] ON 

INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18034, N'waybill sample--2025-03-16-20:05:21', 123474, 1144, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18035, N'waybill sample--2025-03-16-20:05:21', 123475, 1148, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18036, N'sample shipment--2025-03-17-14:06:34', 123476, 1144, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18037, N'sample shipment--2025-03-17-14:06:34', 123477, 1148, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18038, N'Shipment: HAWB9595443--2025-04-03-07:13:53', 123484, 100, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18039, N'Shipment: HAWB9595443--2025-04-03-07:13:53', 123485, 111, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18040, N'Shipment: HAWB9595443--2025-04-03-07:13:53', 123486, 1151, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18041, N'Shipment: HAWB9595443--2025-04-03-07:13:53', 123487, 109, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18042, N'Unknown PDF Found--2025-04-03-20:15:02', 123489, 1148, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18043, N'Unknown PDF Found--2025-04-03-20:18:59', 123490, 1148, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18044, N'Shipment: HAWB9595443--2025-04-03-21:34:29', 123497, 100, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18045, N'Shipment: HAWB9595443--2025-04-03-21:34:29', 123498, 111, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18046, N'Shipment: HAWB9595443--2025-04-03-21:34:29', 123499, 1151, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18047, N'Shipment: HAWB9595443--2025-04-03-21:34:29', 123500, 109, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18048, N'Shipment: HAWB9595459--2025-04-03-21:34:32', 123501, 100, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18049, N'Shipment: HAWB9595459--2025-04-03-21:34:32', 123502, 111, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18050, N'Shipment: HAWB9595459--2025-04-03-21:34:32', 123503, 1151, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18051, N'Shipment: HAWB9595459--2025-04-03-21:34:32', 123504, 109, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18052, N'Shipment: HAWB9596948--2025-04-03-21:34:36', 123505, 100, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18053, N'Shipment: HAWB9596948--2025-04-03-21:34:36', 123506, 111, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18054, N'Shipment: HAWB9596948--2025-04-03-21:34:36', 123507, 1151, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18055, N'Shipment: HAWB9596948--2025-04-03-21:34:36', 123508, 109, 0)
SET IDENTITY_INSERT [dbo].[EmailAttachments] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EmailAttachments_560_559]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EmailAttachments_560_559] ON [dbo].[EmailAttachments]
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EmailAttachments_8_7]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EmailAttachments_8_7] ON [dbo].[EmailAttachments]
(
	[AttachmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EmailAttachments]  WITH CHECK ADD  CONSTRAINT [FK_EmailAttachments_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailAttachments] CHECK CONSTRAINT [FK_EmailAttachments_Attachments]
GO
ALTER TABLE [dbo].[EmailAttachments]  WITH CHECK ADD  CONSTRAINT [FK_EmailAttachments_Emails] FOREIGN KEY([EmailId])
REFERENCES [dbo].[Emails] ([EmailId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailAttachments] CHECK CONSTRAINT [FK_EmailAttachments_Emails]
GO
