USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoiceRiderDetails]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoiceRiderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[RiderDetailId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentInvoiceRiderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentInvoiceRiderDetails]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoiceRiderDetails_ShipmentInvoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[ShipmentInvoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoiceRiderDetails] CHECK CONSTRAINT [FK_ShipmentInvoiceRiderDetails_ShipmentInvoice]
GO
ALTER TABLE [dbo].[ShipmentInvoiceRiderDetails]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoiceRiderDetails_ShipmentRiderDetails] FOREIGN KEY([RiderDetailId])
REFERENCES [dbo].[ShipmentRiderDetails] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoiceRiderDetails] CHECK CONSTRAINT [FK_ShipmentInvoiceRiderDetails_ShipmentRiderDetails]
GO
