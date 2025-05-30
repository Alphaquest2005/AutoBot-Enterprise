USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR_FailedFields]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR_FailedFields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FailedLineEventId] [int] NOT NULL,
	[FieldId] [int] NOT NULL,
 CONSTRAINT [PK_OCR_FailedFields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[OCR_FailedFields]  WITH CHECK ADD  CONSTRAINT [FK_OCR_FailedFields_OCR_FailedLines] FOREIGN KEY([FailedLineEventId])
REFERENCES [dbo].[OCR_FailedLines] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR_FailedFields] CHECK CONSTRAINT [FK_OCR_FailedFields_OCR_FailedLines]
GO
ALTER TABLE [dbo].[OCR_FailedFields]  WITH CHECK ADD  CONSTRAINT [FK_OCR_FailedFields_OCR-Fields] FOREIGN KEY([FieldId])
REFERENCES [dbo].[OCR-Fields] ([Id])
GO
ALTER TABLE [dbo].[OCR_FailedFields] CHECK CONSTRAINT [FK_OCR_FailedFields_OCR-Fields]
GO
