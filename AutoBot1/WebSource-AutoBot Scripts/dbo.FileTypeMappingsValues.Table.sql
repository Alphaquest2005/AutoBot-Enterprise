USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileTypeMappingsValues]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypeMappingsValues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileTypeMappingId] [int] NOT NULL,
	[Value] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FileTypeMappingsValues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileTypeMappingsValues] ON 

INSERT [dbo].[FileTypeMappingsValues] ([Id], [FileTypeMappingId], [Value]) VALUES (1, 3524, N'WEB SOURCE')
SET IDENTITY_INSERT [dbo].[FileTypeMappingsValues] OFF
GO
ALTER TABLE [dbo].[FileTypeMappingsValues]  WITH CHECK ADD  CONSTRAINT [FK_FileTypeMappingsValues_FileTypeMappings] FOREIGN KEY([FileTypeMappingId])
REFERENCES [dbo].[FileTypeMappings] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileTypeMappingsValues] CHECK CONSTRAINT [FK_FileTypeMappingsValues_FileTypeMappings]
GO
