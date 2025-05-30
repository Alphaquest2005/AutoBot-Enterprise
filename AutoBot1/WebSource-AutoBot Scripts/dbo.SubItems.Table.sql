USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[SubItems]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubItems](
	[SubItem_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NOT NULL,
	[ItemNumber] [nvarchar](100) NOT NULL,
	[ItemDescription] [nvarchar](255) NULL,
	[Quantity] [float] NOT NULL,
	[QtyAllocated] [float] NOT NULL,
 CONSTRAINT [PK_SubItems] PRIMARY KEY CLUSTERED 
(
	[SubItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SubItems] ADD  CONSTRAINT [DF_SubItems_Quantity]  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[SubItems] ADD  CONSTRAINT [DF_SubItems_QtyAllocated]  DEFAULT ((0)) FOR [QtyAllocated]
GO
ALTER TABLE [dbo].[SubItems]  WITH NOCHECK ADD  CONSTRAINT [FK_SubItems_xcuda_Item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SubItems] CHECK CONSTRAINT [FK_SubItems_xcuda_Item]
GO
