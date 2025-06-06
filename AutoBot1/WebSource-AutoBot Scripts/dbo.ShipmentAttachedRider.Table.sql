USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachedRider]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachedRider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RiderId] [int] NOT NULL,
	[ShipmentId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachedRider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachedRider]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedRider_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedRider] CHECK CONSTRAINT [FK_ShipmentAttachedRider_Shipment]
GO
ALTER TABLE [dbo].[ShipmentAttachedRider]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedRider_ShipmentRider] FOREIGN KEY([RiderId])
REFERENCES [dbo].[ShipmentRider] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedRider] CHECK CONSTRAINT [FK_ShipmentAttachedRider_ShipmentRider]
GO
