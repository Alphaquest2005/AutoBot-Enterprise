USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItemSource]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItemSource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InventoryId] [int] NOT NULL,
	[InventorySourceId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryItemSource_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_InventoryItemSource_13_12]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItemSource_13_12] ON [dbo].[InventoryItemSource]
(
	[InventorySourceId] ASC
)
INCLUDE([InventoryId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_InventoryItemSource_31_30]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItemSource_31_30] ON [dbo].[InventoryItemSource]
(
	[InventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItemSource]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItemSource_InventoryItems] FOREIGN KEY([InventoryId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryItemSource] CHECK CONSTRAINT [FK_InventoryItemSource_InventoryItems]
GO
ALTER TABLE [dbo].[InventoryItemSource]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItemSource_InventorySources] FOREIGN KEY([InventorySourceId])
REFERENCES [dbo].[InventorySources] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryItemSource] CHECK CONSTRAINT [FK_InventoryItemSource_InventorySources]
GO
