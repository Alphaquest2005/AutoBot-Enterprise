USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Weight_itm]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Weight_itm](
	[Gross_weight_itm] [decimal](18, 2) NOT NULL,
	[Net_weight_itm] [decimal](18, 2) NOT NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Weight_itm_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Weight_itm] ([Gross_weight_itm], [Net_weight_itm], [Valuation_item_Id]) VALUES (CAST(7.50 AS Decimal(18, 2)), CAST(7.50 AS Decimal(18, 2)), 1004680)
INSERT [dbo].[xcuda_Weight_itm] ([Gross_weight_itm], [Net_weight_itm], [Valuation_item_Id]) VALUES (CAST(7.50 AS Decimal(18, 2)), CAST(7.50 AS Decimal(18, 2)), 1004681)
INSERT [dbo].[xcuda_Weight_itm] ([Gross_weight_itm], [Net_weight_itm], [Valuation_item_Id]) VALUES (CAST(7.50 AS Decimal(18, 2)), CAST(7.50 AS Decimal(18, 2)), 1004682)
GO
ALTER TABLE [dbo].[xcuda_Weight_itm]  WITH NOCHECK ADD  CONSTRAINT [FK_Valuation_item_Weight_itm] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Weight_itm] CHECK CONSTRAINT [FK_Valuation_item_Weight_itm]
GO
