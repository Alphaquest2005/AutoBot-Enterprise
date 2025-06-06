USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileTypeReplaceRegex]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypeReplaceRegex](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[Regex] [nvarchar](50) NOT NULL,
	[ReplacementRegEx] [nvarchar](50) NULL,
 CONSTRAINT [PK_FileTypeReplaceRegex] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileTypeReplaceRegex] ON 

INSERT [dbo].[FileTypeReplaceRegex] ([Id], [FileTypeId], [Regex], [ReplacementRegEx]) VALUES (1, 114, N'[\r\n]+\"', N'"')
INSERT [dbo].[FileTypeReplaceRegex] ([Id], [FileTypeId], [Regex], [ReplacementRegEx]) VALUES (2, 1145, N'[\r\n]+\"', N'"')
SET IDENTITY_INSERT [dbo].[FileTypeReplaceRegex] OFF
GO
ALTER TABLE [dbo].[FileTypeReplaceRegex]  WITH NOCHECK ADD  CONSTRAINT [FK_FileTypeReplaceRegex_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileTypeReplaceRegex] CHECK CONSTRAINT [FK_FileTypeReplaceRegex_FileTypes]
GO
