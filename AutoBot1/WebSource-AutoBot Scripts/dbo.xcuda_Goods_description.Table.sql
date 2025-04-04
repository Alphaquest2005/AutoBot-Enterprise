USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Goods_description]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Goods_description](
	[Country_of_origin_code] [varchar](50) NULL,
	[Description_of_goods] [varchar](255) NULL,
	[Commercial_Description] [varchar](255) NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Goods_description] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Goods_description] ([Country_of_origin_code], [Description_of_goods], [Commercial_Description], [Item_Id]) VALUES (N'US', NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Ti', 1004680)
INSERT [dbo].[xcuda_Goods_description] ([Country_of_origin_code], [Description_of_goods], [Commercial_Description], [Item_Id]) VALUES (N'US', NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Ti', 1004681)
INSERT [dbo].[xcuda_Goods_description] ([Country_of_origin_code], [Description_of_goods], [Commercial_Description], [Item_Id]) VALUES (N'US', NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Ti', 1004682)
GO
ALTER TABLE [dbo].[xcuda_Goods_description]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Goods_description] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Goods_description] CHECK CONSTRAINT [FK_Item_Goods_description]
GO
