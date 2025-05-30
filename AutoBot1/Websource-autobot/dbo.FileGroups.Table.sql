USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileGroups]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FileGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileGroups] ON 

INSERT [dbo].[FileGroups] ([Id], [Name]) VALUES (1, N'Monthly Discrepancies')
INSERT [dbo].[FileGroups] ([Id], [Name]) VALUES (2, N'Discrepancies')
SET IDENTITY_INSERT [dbo].[FileGroups] OFF
GO
