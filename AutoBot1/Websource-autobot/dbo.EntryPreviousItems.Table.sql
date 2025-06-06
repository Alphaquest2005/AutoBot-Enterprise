USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryPreviousItems]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryPreviousItems](
	[PreviousItem_Id] [int] NOT NULL,
	[Item_Id] [int] NOT NULL,
	[EntryPreviousItemId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_EntryPreviousItems_1] PRIMARY KEY CLUSTERED 
(
	[EntryPreviousItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_EntryPreviousItems]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_EntryPreviousItems] ON [dbo].[EntryPreviousItems]
(
	[PreviousItem_Id] ASC,
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryPreviousItems_468_467]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryPreviousItems_468_467] ON [dbo].[EntryPreviousItems]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryPreviousItems_518_517]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryPreviousItems_518_517] ON [dbo].[EntryPreviousItems]
(
	[PreviousItem_Id] ASC
)
INCLUDE([Item_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryPreviousItems]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryPreviousItems_xcuda_Item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryPreviousItems] CHECK CONSTRAINT [FK_EntryPreviousItems_xcuda_Item]
GO
