USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryData](
	[EntryDataId] [nvarchar](50) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[EntryDataDate] [datetime2](7) NOT NULL,
	[InvoiceTotal] [float] NULL,
	[ImportedLines] [int] NULL,
	[SupplierCode] [nvarchar](100) NULL,
	[TotalFreight] [float] NULL,
	[TotalInternalFreight] [float] NULL,
	[TotalWeight] [float] NULL,
	[Currency] [nvarchar](4) NULL,
	[FileTypeId] [int] NULL,
	[TotalOtherCost] [float] NULL,
	[TotalInsurance] [float] NULL,
	[TotalDeduction] [float] NULL,
	[SourceFile] [nvarchar](max) NULL,
	[EntryData_Id] [int] IDENTITY(1,1) NOT NULL,
	[Packages] [int] NULL,
	[UpgradeKey] [nvarchar](50) NULL,
	[EntryType] [nvarchar](50) NULL,
	[EmailId] [nvarchar](255) NULL,
 CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EntryData]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_EntryData] ON [dbo].[EntryData]
(
	[EntryType] ASC,
	[ApplicationSettingsId] ASC,
	[EntryDataDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_129_128]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_129_128] ON [dbo].[EntryData]
(
	[EntryDataDate] ASC
)
INCLUDE([ApplicationSettingsId],[Currency],[FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_154_153]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_154_153] ON [dbo].[EntryData]
(
	[EntryDataDate] ASC
)
INCLUDE([ApplicationSettingsId],[FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryData_34_33]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_34_33] ON [dbo].[EntryData]
(
	[EntryDataId] ASC,
	[ApplicationSettingsId] ASC,
	[EntryType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_703_702]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_703_702] ON [dbo].[EntryData]
(
	[EntryDataDate] ASC
)
INCLUDE([EntryDataId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_712_711]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_712_711] ON [dbo].[EntryData]
(
	[ApplicationSettingsId] ASC
)
INCLUDE([EntryDataId],[EntryDataDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryData_731_730]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_731_730] ON [dbo].[EntryData]
(
	[EmailId] ASC
)
INCLUDE([EntryDataId],[ApplicationSettingsId],[EntryDataDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryData]  WITH CHECK ADD  CONSTRAINT [FK_EntryData_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
GO
ALTER TABLE [dbo].[EntryData] CHECK CONSTRAINT [FK_EntryData_ApplicationSettings]
GO
ALTER TABLE [dbo].[EntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData_Emails1] FOREIGN KEY([EmailId])
REFERENCES [dbo].[Emails] ([EmailId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData] CHECK CONSTRAINT [FK_EntryData_Emails1]
GO
ALTER TABLE [dbo].[EntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData_EntryDataType] FOREIGN KEY([EntryType])
REFERENCES [dbo].[EntryDataType] ([EntryDataType])
GO
ALTER TABLE [dbo].[EntryData] CHECK CONSTRAINT [FK_EntryData_EntryDataType]
GO
ALTER TABLE [dbo].[EntryData]  WITH CHECK ADD  CONSTRAINT [FK_EntryData_FileTypes1] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData] CHECK CONSTRAINT [FK_EntryData_FileTypes1]
GO
ALTER TABLE [dbo].[EntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData_Suppliers] FOREIGN KEY([SupplierCode], [ApplicationSettingsId])
REFERENCES [dbo].[Suppliers] ([SupplierCode], [ApplicationSettingsId])
GO
ALTER TABLE [dbo].[EntryData] CHECK CONSTRAINT [FK_EntryData_Suppliers]
GO
