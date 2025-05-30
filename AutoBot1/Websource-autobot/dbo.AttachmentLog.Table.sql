USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AttachmentLog]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocSetAttachment] [int] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AttachmentLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AttachmentLog]  WITH CHECK ADD  CONSTRAINT [FK_AttachmentLog_AsycudaDocumentSet_Attachments] FOREIGN KEY([DocSetAttachment])
REFERENCES [dbo].[AsycudaDocumentSet_Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AttachmentLog] CHECK CONSTRAINT [FK_AttachmentLog_AsycudaDocumentSet_Attachments]
GO
