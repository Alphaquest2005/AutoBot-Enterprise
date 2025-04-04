USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocument_Attachments]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaDocument_Attachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[AsycudaDocumentId] [int] NOT NULL,
 CONSTRAINT [PK_AsycudaDocument_Attachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AsycudaDocument_Attachments] ON 

INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57492, 123498, 48427)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57493, 123511, 48427)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57494, 123512, 48427)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57495, 123502, 48428)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57496, 123515, 48428)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57497, 123516, 48428)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57498, 123506, 48429)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57499, 123519, 48429)
INSERT [dbo].[AsycudaDocument_Attachments] ([Id], [AttachmentId], [AsycudaDocumentId]) VALUES (57500, 123520, 48429)
SET IDENTITY_INSERT [dbo].[AsycudaDocument_Attachments] OFF
GO
ALTER TABLE [dbo].[AsycudaDocument_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocument_Attachments_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocument_Attachments] CHECK CONSTRAINT [FK_AsycudaDocument_Attachments_Attachments]
GO
ALTER TABLE [dbo].[AsycudaDocument_Attachments]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocument_Attachments_xcuda_ASYCUDA] FOREIGN KEY([AsycudaDocumentId])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocument_Attachments] CHECK CONSTRAINT [FK_AsycudaDocument_Attachments_xcuda_ASYCUDA]
GO
