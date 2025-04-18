USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventorySources]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventorySources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_InventorySources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[InventorySources] ON 

INSERT [dbo].[InventorySources] ([Id], [Name]) VALUES (1, N'POS')
INSERT [dbo].[InventorySources] ([Id], [Name]) VALUES (2, N'Asycuda')
INSERT [dbo].[InventorySources] ([Id], [Name]) VALUES (3, N'Supplier')
SET IDENTITY_INSERT [dbo].[InventorySources] OFF
GO
