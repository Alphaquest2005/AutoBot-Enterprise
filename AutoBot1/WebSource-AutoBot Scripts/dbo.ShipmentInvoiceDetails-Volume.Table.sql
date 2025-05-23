USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoiceDetails-Volume]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoiceDetails-Volume](
	[Id] [int] NOT NULL,
	[Quantity] [float] NOT NULL,
	[Units] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ShipmentInvoiceDetails-Volume] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentInvoiceDetails-Volume]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoiceDetails-Volume_ShipmentInvoiceDetails] FOREIGN KEY([Id])
REFERENCES [dbo].[ShipmentInvoiceDetails] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoiceDetails-Volume] CHECK CONSTRAINT [FK_ShipmentInvoiceDetails-Volume_ShipmentInvoiceDetails]
GO
