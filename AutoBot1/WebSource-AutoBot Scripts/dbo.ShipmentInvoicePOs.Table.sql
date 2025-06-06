USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoicePOs]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoicePOs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[EntryData_Id] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentInvoicePOs] PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC,
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentInvoicePOs]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoicePOs_EntryData_PurchaseOrders] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData_PurchaseOrders] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoicePOs] CHECK CONSTRAINT [FK_ShipmentInvoicePOs_EntryData_PurchaseOrders]
GO
ALTER TABLE [dbo].[ShipmentInvoicePOs]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoicePOs_ShipmentInvoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[ShipmentInvoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoicePOs] CHECK CONSTRAINT [FK_ShipmentInvoicePOs_ShipmentInvoice]
GO
