USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_item_other_cost]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_item_other_cost](
	[Amount_national_currency] [float] NOT NULL,
	[Amount_foreign_currency] [float] NOT NULL,
	[Currency_name] [nvarchar](max) NULL,
	[Currency_rate] [float] NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_item_other_cost_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_item_other_cost]  WITH NOCHECK ADD  CONSTRAINT [FK_Valuation_item_item_other_cost] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_item_other_cost] CHECK CONSTRAINT [FK_Valuation_item_item_other_cost]
GO
