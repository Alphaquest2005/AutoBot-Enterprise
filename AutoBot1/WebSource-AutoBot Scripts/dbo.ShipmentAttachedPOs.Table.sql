USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachedPOs]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachedPOs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
	[ShipmentId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachedPOs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachedPOs]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedPOs_EntryData_PurchaseOrders] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData_PurchaseOrders] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedPOs] CHECK CONSTRAINT [FK_ShipmentAttachedPOs_EntryData_PurchaseOrders]
GO
ALTER TABLE [dbo].[ShipmentAttachedPOs]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedPOs_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedPOs] CHECK CONSTRAINT [FK_ShipmentAttachedPOs_Shipment]
GO
