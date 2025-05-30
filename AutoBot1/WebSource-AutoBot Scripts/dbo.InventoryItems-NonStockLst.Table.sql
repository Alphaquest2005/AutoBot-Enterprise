USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItems-NonStockLst]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems-NonStockLst](
	[ItemNumber] [nvarchar](50) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryItems-NonStockLst] PRIMARY KEY CLUSTERED 
(
	[ItemNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItems-NonStockLst] ADD  CONSTRAINT [DF_InventoryItems-NonStockLst_ApplicationSettingsId]  DEFAULT ((7)) FOR [ApplicationSettingsId]
GO
