USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR_FailedLines]    Script Date: 4/3/2025 10:23:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR_FailedLines](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocSetAttachmentId] [int] NOT NULL,
	[LineId] [int] NOT NULL,
	[Resolved] [bit] NOT NULL,
 CONSTRAINT [PK_OCR_FailedLines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OCR_FailedLines]  WITH CHECK ADD  CONSTRAINT [FK_OCR_FailedLines_ImportErrors] FOREIGN KEY([DocSetAttachmentId])
REFERENCES [dbo].[ImportErrors] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR_FailedLines] CHECK CONSTRAINT [FK_OCR_FailedLines_ImportErrors]
GO
ALTER TABLE [dbo].[OCR_FailedLines]  WITH CHECK ADD  CONSTRAINT [FK_OCR_FailedLines_OCR-Lines] FOREIGN KEY([LineId])
REFERENCES [dbo].[OCR-Lines] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR_FailedLines] CHECK CONSTRAINT [FK_OCR_FailedLines_OCR-Lines]
GO
