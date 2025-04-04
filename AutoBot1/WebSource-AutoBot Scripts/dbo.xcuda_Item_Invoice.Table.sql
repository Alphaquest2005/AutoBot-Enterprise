USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Item_Invoice]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Item_Invoice](
	[Amount_national_currency] [float] NOT NULL,
	[Amount_foreign_currency] [float] NOT NULL,
	[Currency_code] [nvarchar](20) NULL,
	[Currency_rate] [float] NOT NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Item_Invoice_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Item_Invoice] ([Amount_national_currency], [Amount_foreign_currency], [Currency_code], [Currency_rate], [Valuation_item_Id]) VALUES (0, 119.97000122070313, N'USD', 0, 1004680)
INSERT [dbo].[xcuda_Item_Invoice] ([Amount_national_currency], [Amount_foreign_currency], [Currency_code], [Currency_rate], [Valuation_item_Id]) VALUES (0, 119.97000122070313, N'USD', 0, 1004681)
INSERT [dbo].[xcuda_Item_Invoice] ([Amount_national_currency], [Amount_foreign_currency], [Currency_code], [Currency_rate], [Valuation_item_Id]) VALUES (0, 119.97000122070313, N'USD', 0, 1004682)
GO
ALTER TABLE [dbo].[xcuda_Item_Invoice]  WITH NOCHECK ADD  CONSTRAINT [FK_Valuation_item_Item_Invoice] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Item_Invoice] CHECK CONSTRAINT [FK_Valuation_item_Item_Invoice]
GO
