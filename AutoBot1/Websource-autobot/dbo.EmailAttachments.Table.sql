USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailAttachments]    Script Date: 3/27/2025 1:48:23 AM ******/
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

INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18012, N'Shipment: HAWB9595443--2025-03-26-20:00:34', 123339, 100, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18013, N'Shipment: HAWB9595443--2025-03-26-20:00:34', 123340, 111, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18014, N'Shipment: HAWB9595443--2025-03-26-20:00:34', 123341, 1151, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18015, N'Shipment: HAWB9595443--2025-03-26-20:00:34', 123342, 109, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18016, N'Shipment: HAWB9595459--2025-03-26-20:05:53', 123343, 100, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18017, N'Shipment: HAWB9595459--2025-03-26-20:05:53', 123344, 111, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18018, N'Shipment: HAWB9595459--2025-03-26-20:05:53', 123345, 1151, 1)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18019, N'Shipment: HAWB9595459--2025-03-26-20:05:53', 123346, 109, 0)
SET IDENTITY_INSERT [dbo].[EmailAttachments] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EmailAttachments_560_559]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EmailAttachments_560_559] ON [dbo].[EmailAttachments]
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EmailAttachments_8_7]    Script Date: 3/27/2025 1:48:25 AM ******/
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
