USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoice]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoice](
	[InvoiceNo] [nvarchar](50) NOT NULL,
	[InvoiceDate] [datetime2](7) NULL,
	[InvoiceTotal] [float] NULL,
	[ImportedLines] [int] NULL,
	[SupplierCode] [nvarchar](100) NULL,
	[SupplierName] [nvarchar](100) NULL,
	[SupplierAddress] [nvarchar](500) NULL,
	[SupplierCountry] [nvarchar](50) NULL,
	[ConsigneeName] [nvarchar](100) NULL,
	[ConsigneeAddress] [nvarchar](500) NULL,
	[ConsigneeCountry] [nvarchar](50) NULL,
	[TotalInternalFreight] [float] NULL,
	[Currency] [nvarchar](4) NULL,
	[EmailId] [nvarchar](255) NULL,
	[FileTypeId] [int] NOT NULL,
	[TotalOtherCost] [float] NULL,
	[TotalInsurance] [float] NULL,
	[TotalDeduction] [float] NULL,
	[SourceFile] [nvarchar](max) NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileLineNumber] [int] NULL,
	[SubTotal] [float] NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentEntryData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ShipmentInvoice] ON 

INSERT [dbo].[ShipmentInvoice] ([InvoiceNo], [InvoiceDate], [InvoiceTotal], [ImportedLines], [SupplierCode], [SupplierName], [SupplierAddress], [SupplierCountry], [ConsigneeName], [ConsigneeAddress], [ConsigneeCountry], [TotalInternalFreight], [Currency], [EmailId], [FileTypeId], [TotalOtherCost], [TotalInsurance], [TotalDeduction], [SourceFile], [Id], [FileLineNumber], [SubTotal], [ApplicationSettingsId]) VALUES (N'111-8019845-2302666', CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 171.369995117188, 1, N'AMAZON.COM', N'AMAZON.COM', NULL, NULL, NULL, NULL, NULL, 43, NULL, N'Shipment: HAWB9595459--2025-03-26-20:05:53', 1167, 8.39999961853027, NULL, 0, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\429\111-8019845-2302666.pdf', 15791, 1, 119.96999549865774, 3)
SET IDENTITY_INSERT [dbo].[ShipmentInvoice] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_ShipmentInvoice_1239_1238]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ShipmentInvoice_1239_1238] ON [dbo].[ShipmentInvoice]
(
	[InvoiceNo] ASC
)
INCLUDE([ApplicationSettingsId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
