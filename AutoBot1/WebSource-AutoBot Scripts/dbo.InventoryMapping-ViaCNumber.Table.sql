USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryMapping-ViaCNumber]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryMapping-ViaCNumber](
	[POSItemCode] [nvarchar](50) NOT NULL,
	[CNumber] [nvarchar](50) NOT NULL,
	[LineNumber] [int] NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryMapping-ViaCNumber] PRIMARY KEY CLUSTERED 
(
	[POSItemCode] ASC,
	[CNumber] ASC,
	[LineNumber] ASC,
	[ApplicationSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
