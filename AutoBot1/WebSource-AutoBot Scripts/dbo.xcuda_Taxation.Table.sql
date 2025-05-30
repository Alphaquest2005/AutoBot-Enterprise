USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Taxation]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Taxation](
	[Item_taxes_amount] [float] NOT NULL,
	[Item_taxes_guaranted_amount] [float] NULL,
	[Counter_of_normal_mode_of_payment] [nvarchar](20) NULL,
	[Displayed_item_taxes_amount] [nvarchar](20) NULL,
	[Taxation_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NULL,
	[Item_taxes_mode_of_payment] [nvarchar](20) NULL,
 CONSTRAINT [PK_xcuda_Taxation] PRIMARY KEY CLUSTERED 
(
	[Taxation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Taxation_472_471]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Taxation_472_471] ON [dbo].[xcuda_Taxation]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Taxation]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Taxation] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Taxation] CHECK CONSTRAINT [FK_Item_Taxation]
GO
