USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryData]    Script Date: 4/3/2025 10:23:54 PM ******/
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
SET IDENTITY_INSERT [dbo].[EntryData] ON 

INSERT [dbo].[EntryData] ([EntryDataId], [ApplicationSettingsId], [EntryDataDate], [InvoiceTotal], [ImportedLines], [SupplierCode], [TotalFreight], [TotalInternalFreight], [TotalWeight], [Currency], [FileTypeId], [TotalOtherCost], [TotalInsurance], [TotalDeduction], [SourceFile], [EntryData_Id], [Packages], [UpgradeKey], [EntryType], [EmailId]) VALUES (N'111-8019845-2302666', 3, CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 171.3699951171875, NULL, N'AMAZON.COM', 0, 43, 0, NULL, 1152, 8.3999996185302734, 0, 0, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\111-8019845-2302666-Fixed.csv', 507282, 1, NULL, N'PO', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[EntryData] ([EntryDataId], [ApplicationSettingsId], [EntryDataDate], [InvoiceTotal], [ImportedLines], [SupplierCode], [TotalFreight], [TotalInternalFreight], [TotalWeight], [Currency], [FileTypeId], [TotalOtherCost], [TotalInsurance], [TotalDeduction], [SourceFile], [EntryData_Id], [Packages], [UpgradeKey], [EntryType], [EmailId]) VALUES (N'111-8019845-2302666', 3, CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 171.3699951171875, NULL, N'AMAZON.COM', 0, 43, 0, NULL, 1152, 8.3999996185302734, 0, 0, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\111-8019845-2302666-Fixed.csv', 507283, 1, NULL, N'PO', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[EntryData] ([EntryDataId], [ApplicationSettingsId], [EntryDataDate], [InvoiceTotal], [ImportedLines], [SupplierCode], [TotalFreight], [TotalInternalFreight], [TotalWeight], [Currency], [FileTypeId], [TotalOtherCost], [TotalInsurance], [TotalDeduction], [SourceFile], [EntryData_Id], [Packages], [UpgradeKey], [EntryType], [EmailId]) VALUES (N'111-8019845-2302666', 3, CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 171.3699951171875, NULL, N'AMAZON.COM', 0, 43, 0, NULL, 1152, 8.3999996185302734, 0, 0, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\111-8019845-2302666-Fixed.csv', 507284, 1, NULL, N'PO', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
SET IDENTITY_INSERT [dbo].[EntryData] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EntryData]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [IX_EntryData] ON [dbo].[EntryData]
(
	[EntryType] ASC,
	[ApplicationSettingsId] ASC,
	[EntryDataDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_129_128]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_129_128] ON [dbo].[EntryData]
(
	[EntryDataDate] ASC
)
INCLUDE([ApplicationSettingsId],[Currency],[FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_154_153]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_154_153] ON [dbo].[EntryData]
(
	[EntryDataDate] ASC
)
INCLUDE([ApplicationSettingsId],[FileTypeId],[EmailId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryData_34_33]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_34_33] ON [dbo].[EntryData]
(
	[EntryDataId] ASC,
	[ApplicationSettingsId] ASC,
	[EntryType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_703_702]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_703_702] ON [dbo].[EntryData]
(
	[EntryDataDate] ASC
)
INCLUDE([EntryDataId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryData_712_711]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryData_712_711] ON [dbo].[EntryData]
(
	[ApplicationSettingsId] ASC
)
INCLUDE([EntryDataId],[EntryDataDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryData_731_730]    Script Date: 4/3/2025 10:23:55 PM ******/
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
