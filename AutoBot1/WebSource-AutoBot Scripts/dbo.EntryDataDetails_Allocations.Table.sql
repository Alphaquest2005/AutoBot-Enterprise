USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryDataDetails_Allocations]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryDataDetails_Allocations](
	[EntryDataDetailsId] [int] NOT NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_EntryDataDetails_Allocations] PRIMARY KEY CLUSTERED 
(
	[EntryDataDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryDataDetails_Allocations]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryDataDetails_Allocations_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryDataDetails_Allocations] CHECK CONSTRAINT [FK_EntryDataDetails_Allocations_EntryDataDetails]
GO
ALTER TABLE [dbo].[EntryDataDetails_Allocations]  WITH CHECK ADD  CONSTRAINT [FK_EntryDataDetails_Allocations_xcuda_Item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryDataDetails_Allocations] CHECK CONSTRAINT [FK_EntryDataDetails_Allocations_xcuda_Item]
GO
