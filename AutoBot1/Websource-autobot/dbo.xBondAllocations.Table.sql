USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xBondAllocations]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xBondAllocations](
	[xEntryItem_Id] [int] NOT NULL,
	[AllocationId] [int] NOT NULL,
	[xBondAllocationId] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK_xBondAllocations] PRIMARY KEY CLUSTERED 
(
	[xBondAllocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xBondAllocations_512_511]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xBondAllocations_512_511] ON [dbo].[xBondAllocations]
(
	[AllocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xBondAllocations]  WITH NOCHECK ADD  CONSTRAINT [FK_xBondAllocations_AsycudaSalesAllocations] FOREIGN KEY([AllocationId])
REFERENCES [dbo].[AsycudaSalesAllocations] ([AllocationId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xBondAllocations] CHECK CONSTRAINT [FK_xBondAllocations_AsycudaSalesAllocations]
GO
ALTER TABLE [dbo].[xBondAllocations]  WITH NOCHECK ADD  CONSTRAINT [FK_xBondAllocations_xcuda_Item] FOREIGN KEY([xEntryItem_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xBondAllocations] CHECK CONSTRAINT [FK_xBondAllocations_xcuda_Item]
GO
