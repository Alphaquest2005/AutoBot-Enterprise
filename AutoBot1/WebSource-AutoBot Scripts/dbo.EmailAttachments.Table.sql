USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailAttachments]    Script Date: 4/8/2025 8:33:17 AM ******/
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

INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18315, N'Fw: Invoices * BOL--2025-04-07-13:16:27', 124289, 1144, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18316, N'Fw: Invoices * BOL--2025-04-07-13:16:27', 124290, 1144, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18317, N'Fw: Invoices * BOL--2025-04-07-13:16:27', 124291, 1144, 0)
INSERT [dbo].[EmailAttachments] ([Id], [EmailId], [AttachmentId], [FileTypeId], [DocumentSpecific]) VALUES (18318, N'Fw: Invoices * BOL--2025-04-07-13:16:27', 124292, 1148, 0)
SET IDENTITY_INSERT [dbo].[EmailAttachments] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EmailAttachments_560_559]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EmailAttachments_560_559] ON [dbo].[EmailAttachments]
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EmailAttachments_8_7]    Script Date: 4/8/2025 8:33:18 AM ******/
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
