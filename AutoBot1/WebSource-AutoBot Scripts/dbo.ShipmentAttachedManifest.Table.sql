USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachedManifest]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachedManifest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentId] [int] NOT NULL,
	[ManifestId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachedManifest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachedManifest]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedManifest_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedManifest] CHECK CONSTRAINT [FK_ShipmentAttachedManifest_Shipment]
GO
ALTER TABLE [dbo].[ShipmentAttachedManifest]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedManifest_ShipmentManifest] FOREIGN KEY([ManifestId])
REFERENCES [dbo].[ShipmentManifest] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedManifest] CHECK CONSTRAINT [FK_ShipmentAttachedManifest_ShipmentManifest]
GO
