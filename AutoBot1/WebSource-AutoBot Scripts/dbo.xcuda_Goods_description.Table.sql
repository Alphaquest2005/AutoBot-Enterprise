USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Goods_description]    Script Date: 4/8/2025 8:33:17 AM ******/
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
ALTER TABLE [dbo].[xcuda_Goods_description]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Goods_description] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Goods_description] CHECK CONSTRAINT [FK_Item_Goods_description]
GO
