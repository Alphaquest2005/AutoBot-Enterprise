USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItems-NonStock]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems-NonStock](
	[InventoryItemId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryItems-NonStock] PRIMARY KEY CLUSTERED 
(
	[InventoryItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItems-NonStock]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItems-NonStock_InventoryItems] FOREIGN KEY([InventoryItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryItems-NonStock] CHECK CONSTRAINT [FK_InventoryItems-NonStock_InventoryItems]
GO
