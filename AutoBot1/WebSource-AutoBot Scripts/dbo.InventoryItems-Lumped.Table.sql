USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItems-Lumped]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems-Lumped](
	[InventoryItemId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryItems-Lumped] PRIMARY KEY CLUSTERED 
(
	[InventoryItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItems-Lumped]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItems-Lumped_InventoryItems] FOREIGN KEY([InventoryItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryItems-Lumped] CHECK CONSTRAINT [FK_InventoryItems-Lumped_InventoryItems]
GO
