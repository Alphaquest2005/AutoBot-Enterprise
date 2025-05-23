USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Attachments]    Script Date: 4/8/2025 8:33:18 AM ******/
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
