USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailFileTypes]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailFileTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailMappingId] [int] NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[IsRequired] [bit] NULL,
 CONSTRAINT [PK_EmailFileTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EmailFileTypes] ON 

INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (30, 27, 93, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (31, 27, 94, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (33, 22, 99, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (35, 22, 100, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (39, 22, 104, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (41, 22, 109, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (43, 22, 111, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (45, 22, 112, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (47, 22, 114, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (55, 36, 1126, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (56, 37, 1127, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (57, 22, 1139, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (61, 39, 1141, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (62, 39, 1143, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (63, 39, 1144, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (64, 39, 1148, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (65, 39, 1145, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (66, 22, 1151, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (68, 22, 1156, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (70, 39, 1158, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (74, 22, 1162, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (79, 43, 1173, NULL)
INSERT [dbo].[EmailFileTypes] ([Id], [EmailMappingId], [FileTypeId], [IsRequired]) VALUES (80, 43, 1144, NULL)
SET IDENTITY_INSERT [dbo].[EmailFileTypes] OFF
GO
ALTER TABLE [dbo].[EmailFileTypes]  WITH CHECK ADD  CONSTRAINT [FK_EmailFileTypes_EmailMapping] FOREIGN KEY([EmailMappingId])
REFERENCES [dbo].[EmailMapping] ([Id])
GO
ALTER TABLE [dbo].[EmailFileTypes] CHECK CONSTRAINT [FK_EmailFileTypes_EmailMapping]
GO
ALTER TABLE [dbo].[EmailFileTypes]  WITH NOCHECK ADD  CONSTRAINT [FK_EmailFileTypes_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailFileTypes] CHECK CONSTRAINT [FK_EmailFileTypes_FileTypes]
GO
