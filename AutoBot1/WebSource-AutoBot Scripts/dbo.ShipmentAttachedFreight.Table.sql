USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachedFreight]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachedFreight](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FreightInvoiceId] [int] NOT NULL,
	[ShipmentId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachedFreight] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachedFreight]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedFreight_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedFreight] CHECK CONSTRAINT [FK_ShipmentAttachedFreight_Shipment]
GO
ALTER TABLE [dbo].[ShipmentAttachedFreight]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedFreight_ShipmentFreight] FOREIGN KEY([FreightInvoiceId])
REFERENCES [dbo].[ShipmentFreight] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedFreight] CHECK CONSTRAINT [FK_ShipmentAttachedFreight_ShipmentFreight]
GO
