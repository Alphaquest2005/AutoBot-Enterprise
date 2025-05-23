USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryDataDetails]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryDataDetails](
	[EntryDataDetailsId] [int] IDENTITY(1,1) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
	[EntryDataId] [nvarchar](50) NOT NULL,
	[LineNumber] [int] NULL,
	[ItemNumber] [nvarchar](20) NOT NULL,
	[Quantity] [float] NOT NULL,
	[Units] [nvarchar](15) NULL,
	[ItemDescription] [nvarchar](255) NOT NULL,
	[Cost] [float] NOT NULL,
	[TotalCost] [float] NULL,
	[QtyAllocated] [float] NOT NULL,
	[UnitWeight] [float] NOT NULL,
	[DoNotAllocate] [bit] NULL,
	[Freight] [float] NULL,
	[Weight] [float] NULL,
	[InternalFreight] [float] NULL,
	[Status] [nvarchar](50) NULL,
	[InvoiceQty] [float] NULL,
	[ReceivedQty] [float] NULL,
	[PreviousInvoiceNumber] [nvarchar](255) NULL,
	[CNumber] [nvarchar](255) NULL,
	[Comment] [nvarchar](255) NULL,
	[EffectiveDate] [datetime2](7) NULL,
	[IsReconciled] [bit] NULL,
	[TaxAmount] [float] NULL,
	[LastCost] [float] NULL,
	[InventoryItemId] [int] NOT NULL,
	[FileLineNumber] [int] NULL,
	[UpgradeKey] [int] NULL,
	[VolumeLiters] [float] NULL,
	[EntryDataDetailsKey]  AS (CONVERT([nvarchar](60),(replace([EntryDataId],' ','')+'|')+CONVERT([nvarchar](50),[LineNumber]))) PERSISTED,
	[TotalValue]  AS ([Quantity]*[Cost]) PERSISTED NOT NULL,
	[CLineNumber] [int] NULL,
 CONSTRAINT [PK_PurchaseOrderDetails] PRIMARY KEY CLUSTERED 
(
	[EntryDataDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EntryDataDetails] ON 

INSERT [dbo].[EntryDataDetails] ([EntryDataDetailsId], [EntryData_Id], [EntryDataId], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [QtyAllocated], [UnitWeight], [DoNotAllocate], [Freight], [Weight], [InternalFreight], [Status], [InvoiceQty], [ReceivedQty], [PreviousInvoiceNumber], [CNumber], [Comment], [EffectiveDate], [IsReconciled], [TaxAmount], [LastCost], [InventoryItemId], [FileLineNumber], [UpgradeKey], [VolumeLiters], [CLineNumber]) VALUES (1858366, 507245, N'111-8019845-2302666', 1, N'MESAILUP16INCHLEDLIG', 3, NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', 39.9900016784668, 119.97000122070313, 0, 0, NULL, 0, 0, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, 68930, 1, NULL, 0, NULL)
SET IDENTITY_INSERT [dbo].[EntryDataDetails] OFF
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_2_1]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_2_1] ON [dbo].[EntryDataDetails]
(
	[InventoryItemId] ASC
)
INCLUDE([EntryData_Id],[EntryDataId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_20_19]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_20_19] ON [dbo].[EntryDataDetails]
(
	[Status] ASC
)
INCLUDE([EntryDataId],[ItemNumber],[DoNotAllocate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_22_21]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_22_21] ON [dbo].[EntryDataDetails]
(
	[ItemNumber] ASC
)
INCLUDE([EntryData_Id],[EntryDataId],[LineNumber],[Quantity],[Units],[ItemDescription],[Cost],[QtyAllocated],[UnitWeight],[DoNotAllocate],[Status],[InvoiceQty],[ReceivedQty],[PreviousInvoiceNumber],[CNumber],[Comment],[EffectiveDate],[IsReconciled],[TaxAmount],[LastCost],[InventoryItemId],[FileLineNumber],[VolumeLiters],[CLineNumber]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_24_23]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_24_23] ON [dbo].[EntryDataDetails]
(
	[Quantity] ASC,
	[EntryDataDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_36_35]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_36_35] ON [dbo].[EntryDataDetails]
(
	[EntryData_Id] ASC
)
INCLUDE([EntryDataId],[LineNumber],[ItemNumber],[Quantity],[Units],[ItemDescription],[Cost],[TotalCost],[QtyAllocated],[UnitWeight],[DoNotAllocate],[Freight],[Weight],[InternalFreight],[Status],[InvoiceQty],[ReceivedQty],[PreviousInvoiceNumber],[CNumber],[Comment],[EffectiveDate],[IsReconciled],[TaxAmount],[CLineNumber],[LastCost],[InventoryItemId],[FileLineNumber],[UpgradeKey],[VolumeLiters]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_76_75]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_76_75] ON [dbo].[EntryDataDetails]
(
	[EffectiveDate] ASC
)
INCLUDE([EntryData_Id],[EntryDataId],[LineNumber],[ItemNumber],[Quantity],[ItemDescription],[Cost],[QtyAllocated],[UnitWeight],[DoNotAllocate],[InvoiceQty],[ReceivedQty],[PreviousInvoiceNumber],[CNumber],[Comment],[Units],[IsReconciled],[TaxAmount],[LastCost],[InventoryItemId],[CLineNumber],[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_82_81]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_82_81] ON [dbo].[EntryDataDetails]
(
	[EntryDataId] ASC,
	[ItemNumber] ASC,
	[EntryDataDetailsKey] ASC
)
INCLUDE([EntryData_Id],[LineNumber]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_865_864]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_865_864] ON [dbo].[EntryDataDetails]
(
	[EntryDataDetailsKey] ASC
)
INCLUDE([EntryData_Id],[EntryDataId],[LineNumber],[ItemNumber]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_9552_9551]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_9552_9551] ON [dbo].[EntryDataDetails]
(
	[ItemNumber] ASC
)
INCLUDE([EntryData_Id],[EntryDataId],[LineNumber],[Quantity],[Units],[ItemDescription],[Cost],[TotalCost],[QtyAllocated],[UnitWeight],[DoNotAllocate],[Freight],[Weight],[InternalFreight],[Status],[InvoiceQty],[ReceivedQty],[PreviousInvoiceNumber],[CNumber],[Comment],[EffectiveDate],[IsReconciled],[TaxAmount],[LastCost],[InventoryItemId],[FileLineNumber],[UpgradeKey],[VolumeLiters],[EntryDataDetailsKey],[TotalValue],[CLineNumber]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_EntryDataDetails_96_95]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_EntryDataDetails_96_95] ON [dbo].[EntryDataDetails]
(
	[InventoryItemId] ASC
)
INCLUDE([EntryDataId],[QtyAllocated],[Status],[Comment],[EffectiveDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryDataDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryDataDetails_EntryData] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryDataDetails] CHECK CONSTRAINT [FK_EntryDataDetails_EntryData]
GO
ALTER TABLE [dbo].[EntryDataDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryDataDetails_InventoryItems] FOREIGN KEY([InventoryItemId])
REFERENCES [dbo].[InventoryItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryDataDetails] CHECK CONSTRAINT [FK_EntryDataDetails_InventoryItems]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'EntryDataDetails.Quantity * EntryDataDetails.Cost' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'EntryDataDetails', @level2type=N'COLUMN',@level2name=N'TotalValue'
GO
