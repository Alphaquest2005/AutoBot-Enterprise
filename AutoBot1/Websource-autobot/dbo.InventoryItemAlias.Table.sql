USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItemAlias]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItemAlias](
	[AliasId] [int] IDENTITY(1,1) NOT NULL,
	[InventoryItemId] [int] NOT NULL,
	[AliasItemId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryItemAlias] PRIMARY KEY CLUSTERED 
(
	[AliasId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItemAlias]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItemAlias_InventoryItems] FOREIGN KEY([InventoryItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryItemAlias] CHECK CONSTRAINT [FK_InventoryItemAlias_InventoryItems]
GO
ALTER TABLE [dbo].[InventoryItemAlias]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItemAlias_InventoryItems1] FOREIGN KEY([AliasItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
GO
ALTER TABLE [dbo].[InventoryItemAlias] CHECK CONSTRAINT [FK_InventoryItemAlias_InventoryItems1]
GO
ALTER TABLE [dbo].[InventoryItemAlias]  WITH CHECK ADD  CONSTRAINT [CK_InventoryItemIdNOTAliasItemId] CHECK  (([inventoryitemid]<>[aliasitemid]))
GO
ALTER TABLE [dbo].[InventoryItemAlias] CHECK CONSTRAINT [CK_InventoryItemIdNOTAliasItemId]
GO
