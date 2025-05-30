USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItems-DoNotMap]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems-DoNotMap](
	[InventoryItemId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryItems-DoNotMap] PRIMARY KEY CLUSTERED 
(
	[InventoryItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItems-DoNotMap]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItems-DoNotMap_InventoryItems] FOREIGN KEY([InventoryItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryItems-DoNotMap] CHECK CONSTRAINT [FK_InventoryItems-DoNotMap_InventoryItems]
GO
