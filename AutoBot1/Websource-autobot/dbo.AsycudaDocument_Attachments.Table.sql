USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocument_Attachments]    Script Date: 3/27/2025 1:48:23 AM ******/
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
