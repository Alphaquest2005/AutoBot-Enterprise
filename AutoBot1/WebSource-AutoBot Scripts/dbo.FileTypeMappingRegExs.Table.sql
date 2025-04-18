USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileTypeMappingRegExs]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypeMappingRegExs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileTypeMappingId] [int] NOT NULL,
	[ReplacementRegex] [nvarchar](1000) NOT NULL,
	[ReplacementValue] [nvarchar](1000) NULL,
 CONSTRAINT [PK_FileTypeMappingRegExs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileTypeMappingRegExs] ON 

INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (1, 252, N'PO/GD/', NULL)
INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (6, 258, N'$', NULL)
INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (7, 258, N'^$', N'0')
INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (19, 251, N'PO/GD/', NULL)
INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (20, 256, N'(\d+)(st|nd|rd|th)', N'$1')
INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (21, 257, N'(\d+)(st|nd|rd|th)', N'$1')
INSERT [dbo].[FileTypeMappingRegExs] ([Id], [FileTypeMappingId], [ReplacementRegex], [ReplacementValue]) VALUES (22, 2378, N'(\d+)(st|nd|rd|th)', N'$1')
SET IDENTITY_INSERT [dbo].[FileTypeMappingRegExs] OFF
GO
ALTER TABLE [dbo].[FileTypeMappingRegExs]  WITH CHECK ADD  CONSTRAINT [FK_FileTypeMappingRegExs_FileTypeMappings] FOREIGN KEY([FileTypeMappingId])
REFERENCES [dbo].[FileTypeMappings] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileTypeMappingRegExs] CHECK CONSTRAINT [FK_FileTypeMappingRegExs_FileTypeMappings]
GO
