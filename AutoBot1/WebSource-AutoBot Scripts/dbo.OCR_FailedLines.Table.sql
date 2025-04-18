USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR_FailedLines]    Script Date: 4/8/2025 8:33:18 AM ******/
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
SET IDENTITY_INSERT [dbo].[OCR_FailedLines] ON 

INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (992, 54243, 1827, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (993, 54243, 1828, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (994, 54243, 1829, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (995, 54243, 1972, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (996, 54243, 1973, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (997, 54243, 1827, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (998, 54243, 1828, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (999, 54243, 1829, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1000, 54243, 1972, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1001, 54243, 1973, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1002, 54247, 2098, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1003, 54247, 2098, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1004, 54248, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1005, 54248, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1006, 54249, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1007, 54249, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1008, 54250, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1009, 54250, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1010, 54251, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1011, 54251, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1012, 54252, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1013, 54253, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1014, 54253, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1015, 54254, 2098, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1016, 54254, 2098, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1017, 54255, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1018, 54255, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1019, 54256, 2098, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1020, 54256, 2098, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1021, 54257, 2097, 0)
INSERT [dbo].[OCR_FailedLines] ([Id], [DocSetAttachmentId], [LineId], [Resolved]) VALUES (1022, 54257, 2097, 0)
SET IDENTITY_INSERT [dbo].[OCR_FailedLines] OFF
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
