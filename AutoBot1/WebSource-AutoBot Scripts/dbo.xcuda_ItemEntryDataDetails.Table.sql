USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_ItemEntryDataDetails]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_ItemEntryDataDetails](
	[Item_Id] [int] NOT NULL,
	[EntryDataDetailsId] [int] NOT NULL,
	[ItemEntryDataDetailId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_ItemEntryDataDetails] PRIMARY KEY CLUSTERED 
(
	[ItemEntryDataDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[xcuda_ItemEntryDataDetails] ON 

INSERT [dbo].[xcuda_ItemEntryDataDetails] ([Item_Id], [EntryDataDetailsId], [ItemEntryDataDetailId]) VALUES (1004680, 1858782, 967911)
INSERT [dbo].[xcuda_ItemEntryDataDetails] ([Item_Id], [EntryDataDetailsId], [ItemEntryDataDetailId]) VALUES (1004681, 1858783, 967912)
INSERT [dbo].[xcuda_ItemEntryDataDetails] ([Item_Id], [EntryDataDetailsId], [ItemEntryDataDetailId]) VALUES (1004682, 1858784, 967913)
SET IDENTITY_INSERT [dbo].[xcuda_ItemEntryDataDetails] OFF
GO
/****** Object:  Index [SQLOPS_xcuda_ItemEntryDataDetails_816_815]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_ItemEntryDataDetails_816_815] ON [dbo].[xcuda_ItemEntryDataDetails]
(
	[EntryDataDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_ItemEntryDataDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_ItemEntryDataDetails_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_ItemEntryDataDetails] CHECK CONSTRAINT [FK_xcuda_ItemEntryDataDetails_EntryDataDetails]
GO
ALTER TABLE [dbo].[xcuda_ItemEntryDataDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_ItemEntryDataDetails_xcuda_Item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_ItemEntryDataDetails] CHECK CONSTRAINT [FK_xcuda_ItemEntryDataDetails_xcuda_Item]
GO
