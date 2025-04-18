USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachedInvoices]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachedInvoices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentId] [int] NOT NULL,
	[ShipmentInvoiceId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachedInvoices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachedInvoices]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedInvoices_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedInvoices] CHECK CONSTRAINT [FK_ShipmentAttachedInvoices_Shipment]
GO
ALTER TABLE [dbo].[ShipmentAttachedInvoices]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedInvoices_ShipmentInvoice] FOREIGN KEY([ShipmentInvoiceId])
REFERENCES [dbo].[ShipmentInvoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedInvoices] CHECK CONSTRAINT [FK_ShipmentAttachedInvoices_ShipmentInvoice]
GO
