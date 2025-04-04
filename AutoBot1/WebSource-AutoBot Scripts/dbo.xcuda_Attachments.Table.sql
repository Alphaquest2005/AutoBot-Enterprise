USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Attachments]    Script Date: 4/3/2025 10:23:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Attachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[Attached_documents_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Attachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[xcuda_Attachments] ON 

INSERT [dbo].[xcuda_Attachments] ([Id], [AttachmentId], [Attached_documents_Id]) VALUES (66385, 123498, 99223)
INSERT [dbo].[xcuda_Attachments] ([Id], [AttachmentId], [Attached_documents_Id]) VALUES (66386, 123510, 99224)
INSERT [dbo].[xcuda_Attachments] ([Id], [AttachmentId], [Attached_documents_Id]) VALUES (66387, 123502, 99225)
INSERT [dbo].[xcuda_Attachments] ([Id], [AttachmentId], [Attached_documents_Id]) VALUES (66388, 123514, 99226)
INSERT [dbo].[xcuda_Attachments] ([Id], [AttachmentId], [Attached_documents_Id]) VALUES (66389, 123506, 99227)
INSERT [dbo].[xcuda_Attachments] ([Id], [AttachmentId], [Attached_documents_Id]) VALUES (66390, 123518, 99228)
SET IDENTITY_INSERT [dbo].[xcuda_Attachments] OFF
GO
ALTER TABLE [dbo].[xcuda_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Attachments_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Attachments] CHECK CONSTRAINT [FK_xcuda_Attachments_Attachments]
GO
ALTER TABLE [dbo].[xcuda_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Attachments_xcuda_Attached_documents] FOREIGN KEY([Attached_documents_Id])
REFERENCES [dbo].[xcuda_Attached_documents] ([Attached_documents_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Attachments] CHECK CONSTRAINT [FK_xcuda_Attachments_xcuda_Attached_documents]
GO
