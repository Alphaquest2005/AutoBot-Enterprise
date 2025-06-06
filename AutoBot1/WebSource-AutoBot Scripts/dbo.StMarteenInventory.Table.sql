USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[StMarteenInventory]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StMarteenInventory](
	[Manual Classification] [nvarchar](255) NULL,
	[Product Category/Parent Category/Parent Category] [nvarchar](255) NULL,
	[Parent Category] [nvarchar](255) NULL,
	[Category] [nvarchar](255) NULL,
	[HS] [nvarchar](255) NULL,
	[Product] [nvarchar](255) NULL,
	[Brand] [nvarchar](255) NULL,
	[Prefix] [nvarchar](255) NULL,
	[Primary Desc] [nvarchar](255) NULL,
	[Category ID] [nvarchar](255) NULL,
	[HS Code] [nvarchar](255) NULL,
	[Index] [nvarchar](255) NULL,
	[Slim Article Code] [nvarchar](255) NULL,
	[Internal Reference] [nvarchar](255) NULL,
	[Product Category/Parent Category] [nvarchar](255) NULL,
	[Product Category] [nvarchar](255) NULL,
	[Product Category/Parent Path] [nvarchar](255) NULL,
	[Product Category1] [nvarchar](255) NULL,
	[Product Category/Parent Category1] [nvarchar](255) NULL,
	[Product Category/Parent Category/Parent Category1] [nvarchar](255) NULL,
	[External ID] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[find] [nvarchar](255) NULL
) ON [PRIMARY]
GO
