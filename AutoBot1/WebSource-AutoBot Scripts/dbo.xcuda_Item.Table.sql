USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Item]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Item](
	[Amount_deducted_from_licence] [nvarchar](10) NULL,
	[Quantity_deducted_from_licence] [nvarchar](4) NULL,
	[Item_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[Licence_number] [nvarchar](50) NULL,
	[Free_text_1] [nvarchar](35) NULL,
	[Free_text_2] [nvarchar](30) NULL,
	[EntryDataDetailsId] [int] NULL,
	[LineNumber] [int] NOT NULL,
	[IsAssessed] [bit] NULL,
	[DPQtyAllocated] [float] NOT NULL,
	[DFQtyAllocated] [float] NOT NULL,
	[EntryTimeStamp] [datetime2](7) NULL,
	[AttributeOnlyAllocation] [bit] NULL,
	[DoNotAllocate] [bit] NULL,
	[DoNotEX] [bit] NULL,
	[ImportComplete] [bit] NOT NULL,
	[WarehouseError] [nvarchar](50) NULL,
	[SalesFactor] [float] NOT NULL,
	[PreviousInvoiceNumber] [nvarchar](50) NULL,
	[PreviousInvoiceLineNumber] [nvarchar](50) NULL,
	[PreviousInvoiceItemNumber] [nvarchar](50) NULL,
	[EntryDataType] [nvarchar](50) NULL,
	[UpgradeKey] [int] NULL,
	[PreviousInvoiceKey]  AS ((replace([PreviousInvoiceNumber],' ','')+'|')+replace([PreviousInvoiceLineNumber],' ','')) PERSISTED,
	[xWarehouseError] [nvarchar](255) NULL,
 CONSTRAINT [PK_xcuda_Item] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
/****** Object:  Index [IX_xcuda_Item]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_xcuda_Item] ON [dbo].[xcuda_Item]
(
	[Item_Id] ASC,
	[PreviousInvoiceItemNumber] ASC,
	[PreviousInvoiceKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_xcuda_Item_1]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_xcuda_Item_1] ON [dbo].[xcuda_Item]
(
	[Item_Id] ASC,
	[SalesFactor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Item_11_10]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_11_10] ON [dbo].[xcuda_Item]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Item_1132_1131]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_1132_1131] ON [dbo].[xcuda_Item]
(
	[LineNumber] ASC
)
INCLUDE([ASYCUDA_Id],[IsAssessed],[DPQtyAllocated],[DFQtyAllocated],[EntryDataType]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Item_140_139]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_140_139] ON [dbo].[xcuda_Item]
(
	[EntryDataDetailsId] ASC
)
INCLUDE([ASYCUDA_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Item_1459_1458]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_1459_1458] ON [dbo].[xcuda_Item]
(
	[ASYCUDA_Id] ASC
)
INCLUDE([DPQtyAllocated],[DFQtyAllocated]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Item_146_145]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_146_145] ON [dbo].[xcuda_Item]
(
	[WarehouseError] ASC
)
INCLUDE([ASYCUDA_Id],[LineNumber],[DPQtyAllocated],[DFQtyAllocated]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Item_48_47]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_48_47] ON [dbo].[xcuda_Item]
(
	[PreviousInvoiceNumber] ASC,
	[PreviousInvoiceItemNumber] ASC
)
INCLUDE([ASYCUDA_Id],[PreviousInvoiceKey]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Item_728_727]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_728_727] ON [dbo].[xcuda_Item]
(
	[LineNumber] ASC
)
INCLUDE([ASYCUDA_Id],[IsAssessed],[DPQtyAllocated],[DFQtyAllocated]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Item_80_79]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Item_80_79] ON [dbo].[xcuda_Item]
(
	[EntryDataDetailsId] ASC
)
INCLUDE([ASYCUDA_Id],[LineNumber]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Item] ADD  CONSTRAINT [DF_xcuda_Item_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[xcuda_Item] ADD  CONSTRAINT [DF_xcuda_Item_SalesFactor]  DEFAULT ((1)) FOR [SalesFactor]
GO
ALTER TABLE [dbo].[xcuda_Item]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Item] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Item] CHECK CONSTRAINT [FK_ASYCUDA_Item]
GO
ALTER TABLE [dbo].[xcuda_Item]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Item_EntryDataType] FOREIGN KEY([EntryDataType])
REFERENCES [dbo].[EntryDataType] ([EntryDataType])
GO
ALTER TABLE [dbo].[xcuda_Item] CHECK CONSTRAINT [FK_xcuda_Item_EntryDataType]
GO
