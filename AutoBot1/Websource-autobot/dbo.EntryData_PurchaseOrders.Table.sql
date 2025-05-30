USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryData_PurchaseOrders]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryData_PurchaseOrders](
	[PONumber] [nvarchar](50) NOT NULL,
	[SupplierInvoiceNo] [nvarchar](50) NULL,
	[EntryData_Id] [int] NOT NULL,
	[PreviousCNumber] [nvarchar](50) NULL,
	[FinancialInformation] [nvarchar](255) NULL,
 CONSTRAINT [PK_EntryData_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[EntryData_PurchaseOrders] ([PONumber], [SupplierInvoiceNo], [EntryData_Id], [PreviousCNumber], [FinancialInformation]) VALUES (N'111-8019845-2302666', N'111-8019845-2302666', 507245, NULL, NULL)
GO
ALTER TABLE [dbo].[EntryData_PurchaseOrders]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData_PurchaseOrders_EntryData1] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData_PurchaseOrders] CHECK CONSTRAINT [FK_EntryData_PurchaseOrders_EntryData1]
GO
