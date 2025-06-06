USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Inventory_Item]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Inventory_Item](
	[Item_Id] [int] NOT NULL,
	[InventoryItemId] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Inventory_Item_1] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Inventory_Item_28_27]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Inventory_Item_28_27] ON [dbo].[xcuda_Inventory_Item]
(
	[InventoryItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Inventory_Item_InventoryItems] FOREIGN KEY([InventoryItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item] CHECK CONSTRAINT [FK_xcuda_Inventory_Item_InventoryItems]
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Inventory_Item_xcuda_HScode1] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_HScode] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item] CHECK CONSTRAINT [FK_xcuda_Inventory_Item_xcuda_HScode1]
GO
