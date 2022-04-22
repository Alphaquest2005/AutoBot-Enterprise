USE [AutoBot-EnterpriseDB]
GO
/****** Object:  Table [dbo].[FileFormats]    Script Date: 4/21/2022 2:38:49 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileFormats] ON 
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (1, N'XML')
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (2, N'CSV')
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (3, N'PDF')
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (4, N'XLSX')
GO
SET IDENTITY_INSERT [dbo].[FileFormats] OFF
GO


