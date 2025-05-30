USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[PackageTypes]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PackageTypes](
	[PackageType] [nvarchar](4) NOT NULL,
	[PackageDescription] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PackageTypes] PRIMARY KEY CLUSTERED 
(
	[PackageDescription] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[PackageTypes] ([PackageType], [PackageDescription]) VALUES (N'PK', N'Package')
GO
