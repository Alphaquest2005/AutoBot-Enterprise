USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachedBL]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachedBL](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShipmentId] [int] NOT NULL,
	[BlId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachedBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachedBL]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedBL_Shipment] FOREIGN KEY([ShipmentId])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedBL] CHECK CONSTRAINT [FK_ShipmentAttachedBL_Shipment]
GO
ALTER TABLE [dbo].[ShipmentAttachedBL]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachedBL_ShipmentBL1] FOREIGN KEY([BlId])
REFERENCES [dbo].[ShipmentBL] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachedBL] CHECK CONSTRAINT [FK_ShipmentAttachedBL_ShipmentBL1]
GO
