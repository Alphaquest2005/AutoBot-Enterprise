USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryMapping]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryMapping](
	[POSItemCode] [nvarchar](50) NOT NULL,
	[AsycudaItemCode] [nvarchar](50) NOT NULL,
	[ApplicationsSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_InventoryMapping] PRIMARY KEY CLUSTERED 
(
	[POSItemCode] ASC,
	[AsycudaItemCode] ASC,
	[ApplicationsSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryMapping] ADD  CONSTRAINT [DF_InventoryMapping_ApplicationsSettingsId]  DEFAULT ((2)) FOR [ApplicationsSettingsId]
GO
ALTER TABLE [dbo].[InventoryMapping]  WITH CHECK ADD  CONSTRAINT [FK_InventoryMapping_ApplicationSettings] FOREIGN KEY([ApplicationsSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InventoryMapping] CHECK CONSTRAINT [FK_InventoryMapping_ApplicationSettings]
GO
