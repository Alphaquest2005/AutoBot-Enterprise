USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Valuation_item]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Valuation_item](
	[Total_cost_itm] [float] NOT NULL,
	[Total_CIF_itm] [float] NOT NULL,
	[Rate_of_adjustement] [float] NULL,
	[Statistical_value] [float] NOT NULL,
	[Alpha_coeficient_of_apportionment] [nvarchar](50) NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Valuation_item] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Valuation_item]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Valuation_item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Valuation_item] CHECK CONSTRAINT [FK_Item_Valuation_item]
GO
