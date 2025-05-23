USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[BondTypes]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BondTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_BondTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[BondTypes] ON 

INSERT [dbo].[BondTypes] ([Id], [Name]) VALUES (1, N'Warehouse')
INSERT [dbo].[BondTypes] ([Id], [Name]) VALUES (2, N'DutyFreeShop')
SET IDENTITY_INSERT [dbo].[BondTypes] OFF
GO
