USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ImportErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportErrors](
	[Id] [int] NOT NULL,
	[PdfText] [nvarchar](max) NOT NULL,
	[Error] [nvarchar](500) NOT NULL,
	[EntryDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ImportErrors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ImportErrors]  WITH CHECK ADD  CONSTRAINT [FK_ImportErrors_AsycudaDocumentSet_Attachments] FOREIGN KEY([Id])
REFERENCES [dbo].[AsycudaDocumentSet_Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ImportErrors] CHECK CONSTRAINT [FK_ImportErrors_AsycudaDocumentSet_Attachments]
GO
