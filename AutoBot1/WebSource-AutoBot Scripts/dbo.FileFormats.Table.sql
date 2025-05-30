USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileFormats]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileFormats](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FileFormats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileFormats] ON 

INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (1, N'XML')
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (2, N'CSV')
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (3, N'PDF')
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (4, N'XLSX')
SET IDENTITY_INSERT [dbo].[FileFormats] OFF
GO
