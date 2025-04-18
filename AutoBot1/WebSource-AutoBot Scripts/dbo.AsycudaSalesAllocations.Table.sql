USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaSalesAllocations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaSalesAllocations](
	[AllocationId] [int] IDENTITY(1,1) NOT NULL,
	[EntryDataDetailsId] [int] NULL,
	[PreviousItem_Id] [int] NULL,
	[Status] [nvarchar](255) NULL,
	[QtyAllocated] [float] NOT NULL,
	[EntryTimeStamp] [datetime2](7) NULL,
	[EANumber] [int] NOT NULL,
	[SANumber] [int] NOT NULL,
	[xEntryItem_Id] [int] NULL,
	[xStatus] [nvarchar](255) NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_AsycudaSalesAllocations] PRIMARY KEY CLUSTERED 
(
	[AllocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_AsycudaSalesAllocations]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_AsycudaSalesAllocations] ON [dbo].[AsycudaSalesAllocations]
(
	[AllocationId] ASC,
	[EntryDataDetailsId] ASC,
	[xEntryItem_Id] ASC,
	[PreviousItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaSalesAllocations_17_16]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaSalesAllocations_17_16] ON [dbo].[AsycudaSalesAllocations]
(
	[xEntryItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaSalesAllocations_20_19]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaSalesAllocations_20_19] ON [dbo].[AsycudaSalesAllocations]
(
	[xEntryItem_Id] ASC
)
INCLUDE([EntryDataDetailsId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaSalesAllocations_227_226]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaSalesAllocations_227_226] ON [dbo].[AsycudaSalesAllocations]
(
	[EntryDataDetailsId] ASC
)
INCLUDE([PreviousItem_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_AsycudaSalesAllocations_478_477]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaSalesAllocations_478_477] ON [dbo].[AsycudaSalesAllocations]
(
	[Status] ASC,
	[QtyAllocated] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaSalesAllocations_9_8]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaSalesAllocations_9_8] ON [dbo].[AsycudaSalesAllocations]
(
	[PreviousItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_AsycudaSalesAllocations_930_929]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaSalesAllocations_930_929] ON [dbo].[AsycudaSalesAllocations]
(
	[Status] ASC
)
INCLUDE([EntryDataDetailsId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] ADD  CONSTRAINT [DF_AsycudaSalesAllocations_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaSalesAllocations_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] CHECK CONSTRAINT [FK_AsycudaSalesAllocations_EntryDataDetails]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaSalesAllocations_xcuda_Item] FOREIGN KEY([PreviousItem_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] CHECK CONSTRAINT [FK_AsycudaSalesAllocations_xcuda_Item]
GO
