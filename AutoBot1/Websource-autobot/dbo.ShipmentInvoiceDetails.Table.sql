USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoiceDetails]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoiceDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentInvoiceId] [int] NOT NULL,
	[LineNumber] [int] NULL,
	[ItemNumber] [nvarchar](20) NULL,
	[Quantity] [float] NOT NULL,
	[Units] [nvarchar](15) NULL,
	[ItemDescription] [nvarchar](255) NOT NULL,
	[Cost] [float] NOT NULL,
	[TotalCost] [float] NULL,
	[FileLineNumber] [int] NULL,
	[InventoryItemId] [int] NULL,
	[SalesFactor] [float] NOT NULL,
	[Discount] [float] NULL,
	[TariffCode] [nvarchar](12) NULL,
 CONSTRAINT [PK_ShipmentInvoiceDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ShipmentInvoiceDetails] ON 

INSERT [dbo].[ShipmentInvoiceDetails] ([Id], [ShipmentInvoiceId], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [FileLineNumber], [InventoryItemId], [SalesFactor], [Discount], [TariffCode]) VALUES (141214, 15791, 1, N'MESAILUP16InchLEDLig', 3, NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', 39.99, 119.970001220703, 1, 68930, 1, 0, N'94054000')
SET IDENTITY_INSERT [dbo].[ShipmentInvoiceDetails] OFF
GO
/****** Object:  Index [SQLOPS_ShipmentInvoiceDetails_1412_1411]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ShipmentInvoiceDetails_1412_1411] ON [dbo].[ShipmentInvoiceDetails]
(
	[ShipmentInvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentInvoiceDetails]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoiceDetails_ShipmentInvoice] FOREIGN KEY([ShipmentInvoiceId])
REFERENCES [dbo].[ShipmentInvoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoiceDetails] CHECK CONSTRAINT [FK_ShipmentInvoiceDetails_ShipmentInvoice]
GO
