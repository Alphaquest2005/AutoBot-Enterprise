USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AdjustmentOversAllocations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdjustmentOversAllocations](
	[AllocationId] [int] IDENTITY(1,1) NOT NULL,
	[EntryDataDetailsId] [int] NOT NULL,
	[PreviousItem_Id] [int] NULL,
	[Asycuda_Id] [int] NOT NULL,
 CONSTRAINT [PK_AdjustmentOversAllocations] PRIMARY KEY CLUSTERED 
(
	[AllocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AdjustmentOversAllocations]  WITH NOCHECK ADD  CONSTRAINT [FK_AdjustmentOversAllocations_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdjustmentOversAllocations] CHECK CONSTRAINT [FK_AdjustmentOversAllocations_EntryDataDetails]
GO
ALTER TABLE [dbo].[AdjustmentOversAllocations]  WITH CHECK ADD  CONSTRAINT [FK_AdjustmentOversAllocations_xcuda_ASYCUDA] FOREIGN KEY([Asycuda_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdjustmentOversAllocations] CHECK CONSTRAINT [FK_AdjustmentOversAllocations_xcuda_ASYCUDA]
GO
ALTER TABLE [dbo].[AdjustmentOversAllocations]  WITH CHECK ADD  CONSTRAINT [FK_AdjustmentOversAllocations_xcuda_Item] FOREIGN KEY([PreviousItem_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
GO
ALTER TABLE [dbo].[AdjustmentOversAllocations] CHECK CONSTRAINT [FK_AdjustmentOversAllocations_xcuda_Item]
GO
