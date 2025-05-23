USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Market_valuer]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Market_valuer](
	[Rate] [float] NULL,
	[Currency_amount] [float] NOT NULL,
	[Basis_amount] [float] NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Market_valuer_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Market_valuer]  WITH NOCHECK ADD  CONSTRAINT [FK_Valuation_item_Market_valuer] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Market_valuer] CHECK CONSTRAINT [FK_Valuation_item_Market_valuer]
GO
