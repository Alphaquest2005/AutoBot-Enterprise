USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItems]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems](
	[ItemNumber] [nvarchar](20) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Category] [nvarchar](60) NULL,
	[TariffCode] [nvarchar](50) NULL,
	[EntryTimeStamp] [datetime2](7) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UpgradeKey] [nvarchar](20) NULL,
 CONSTRAINT [PK_InventoryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_InventoryItems_153_152]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_153_152] ON [dbo].[InventoryItems]
(
	[ApplicationSettingsId] ASC
)
INCLUDE([TariffCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_InventoryItems_171_170]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_171_170] ON [dbo].[InventoryItems]
(
	[Description] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_InventoryItems_2_1]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_2_1] ON [dbo].[InventoryItems]
(
	[ApplicationSettingsId] ASC,
	[EntryTimeStamp] ASC
)
INCLUDE([TariffCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_InventoryItems_29_28]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_29_28] ON [dbo].[InventoryItems]
(
	[ItemNumber] ASC,
	[ApplicationSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItems] ADD  CONSTRAINT [DF_InventoryItems_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[InventoryItems]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItems_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
GO
ALTER TABLE [dbo].[InventoryItems] CHECK CONSTRAINT [FK_InventoryItems_ApplicationSettings]
GO
ALTER TABLE [dbo].[InventoryItems]  WITH NOCHECK ADD  CONSTRAINT [FK_InventoryItems_TariffCodes] FOREIGN KEY([TariffCode])
REFERENCES [dbo].[TariffCodes] ([TariffCode])
GO
ALTER TABLE [dbo].[InventoryItems] NOCHECK CONSTRAINT [FK_InventoryItems_TariffCodes]
GO
