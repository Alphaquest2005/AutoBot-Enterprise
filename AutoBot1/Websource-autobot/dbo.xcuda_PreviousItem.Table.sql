USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_PreviousItem]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_PreviousItem](
	[Packages_number] [nvarchar](20) NULL,
	[Previous_Packages_number] [nvarchar](20) NULL,
	[Hs_code] [nvarchar](20) NULL,
	[Commodity_code] [nvarchar](20) NULL,
	[Previous_item_number] [int] NULL,
	[Goods_origin] [nvarchar](20) NULL,
	[Net_weight] [decimal](9, 2) NOT NULL,
	[Prev_net_weight] [decimal](9, 2) NOT NULL,
	[Prev_reg_ser] [nvarchar](20) NULL,
	[Prev_reg_nbr] [nvarchar](20) NULL,
	[Prev_reg_year] [int] NULL,
	[Prev_reg_cuo] [nvarchar](20) NULL,
	[Suplementary_Quantity] [decimal](9, 2) NOT NULL,
	[Preveious_suplementary_quantity] [float] NOT NULL,
	[Current_value] [float] NOT NULL,
	[Previous_value] [float] NOT NULL,
	[Current_item_number] [int] NULL,
	[PreviousItem_Id] [int] NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[QtyAllocated] [float] NOT NULL,
	[Prev_decl_HS_spec] [nvarchar](20) NULL,
 CONSTRAINT [PK_xcuda_PreviousItem] PRIMARY KEY CLUSTERED 
(
	[PreviousItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_PreviousItem_218_217]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_PreviousItem_218_217] ON [dbo].[xcuda_PreviousItem]
(
	[ASYCUDA_Id] ASC
)
INCLUDE([Suplementary_Quantity]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_PreviousItem_489_488]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_PreviousItem_489_488] ON [dbo].[xcuda_PreviousItem]
(
	[Prev_reg_nbr] ASC
)
INCLUDE([Packages_number],[Previous_Packages_number],[Hs_code],[Commodity_code],[Previous_item_number],[QtyAllocated],[Prev_decl_HS_spec],[Suplementary_Quantity],[Preveious_suplementary_quantity],[Current_value],[Previous_value],[Current_item_number],[ASYCUDA_Id],[Goods_origin],[Net_weight],[Prev_net_weight],[Prev_reg_ser],[Prev_reg_year],[Prev_reg_cuo]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_PreviousItem]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_PreviousItem] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
GO
ALTER TABLE [dbo].[xcuda_PreviousItem] CHECK CONSTRAINT [FK_ASYCUDA_PreviousItem]
GO
ALTER TABLE [dbo].[xcuda_PreviousItem]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_PreviousItem_xcuda_Item] FOREIGN KEY([PreviousItem_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_PreviousItem] CHECK CONSTRAINT [FK_xcuda_PreviousItem_xcuda_Item]
GO
