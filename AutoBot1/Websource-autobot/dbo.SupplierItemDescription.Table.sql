USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[SupplierItemDescription]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierItemDescription](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierCode] [nvarchar](100) NOT NULL,
	[Item] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SupplierItemDescription] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[SupplierItemDescription] ON 

INSERT [dbo].[SupplierItemDescription] ([Id], [SupplierCode], [Item]) VALUES (1, N'CAPTAIN', N'Cotter Pin')
INSERT [dbo].[SupplierItemDescription] ([Id], [SupplierCode], [Item]) VALUES (2, N'CAPTAIN', N'Screw')
INSERT [dbo].[SupplierItemDescription] ([Id], [SupplierCode], [Item]) VALUES (3, N'CAPTAIN', N'Hex Head Bolt')
SET IDENTITY_INSERT [dbo].[SupplierItemDescription] OFF
GO
