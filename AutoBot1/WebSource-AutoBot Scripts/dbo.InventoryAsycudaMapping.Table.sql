USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryAsycudaMapping]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryAsycudaMapping](
	[ItemNumber] [nvarchar](20) NOT NULL,
	[Item_Id] [int] NOT NULL,
	[InventoryAsycudaMappingId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_InventoryAsycudaMapping] PRIMARY KEY CLUSTERED 
(
	[InventoryAsycudaMappingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryAsycudaMapping]  WITH NOCHECK ADD  CONSTRAINT [FK_InventoryAsycudaMapping_xcuda_Item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryAsycudaMapping] CHECK CONSTRAINT [FK_InventoryAsycudaMapping_xcuda_Item]
GO
