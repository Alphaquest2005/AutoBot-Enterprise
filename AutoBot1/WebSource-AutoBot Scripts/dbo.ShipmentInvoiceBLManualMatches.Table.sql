USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoiceBLManualMatches]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoiceBLManualMatches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseCode] [nvarchar](50) NOT NULL,
	[BLInvoiceNumber] [nvarchar](50) NOT NULL,
	[InvoiceNo] [nvarchar](50) NOT NULL,
	[Packages] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentInvoiceBLManualMatches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ShipmentInvoiceBLManualMatches] ON 

INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (1, N'11043961', N'1865240486', N'2143817642', 2)
INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (2, N'11052511', N'1865330599', N'2143866515', 1)
INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (4, N'11057364', N'538766', N'137863', 1)
INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (9, N'11057364', N'538764', N'137864', 0)
INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (10, N'11057364', N'538768', N'137865', 0)
INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (11, N'11057364', N'538767', N'137866', 0)
INSERT [dbo].[ShipmentInvoiceBLManualMatches] ([Id], [WarehouseCode], [BLInvoiceNumber], [InvoiceNo], [Packages]) VALUES (12, N'11047114', N'1865240486', N'2143817642', 2)
SET IDENTITY_INSERT [dbo].[ShipmentInvoiceBLManualMatches] OFF
GO
