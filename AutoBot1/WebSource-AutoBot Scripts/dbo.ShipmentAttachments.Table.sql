USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentAttachments]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentAttachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachmentId] [int] NOT NULL,
	[ShipmentID] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentAttachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentAttachments]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachments_Attachments] FOREIGN KEY([AttachmentId])
REFERENCES [dbo].[Attachments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachments] CHECK CONSTRAINT [FK_ShipmentAttachments_Attachments]
GO
ALTER TABLE [dbo].[ShipmentAttachments]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentAttachments_Shipment] FOREIGN KEY([ShipmentID])
REFERENCES [dbo].[Shipment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentAttachments] CHECK CONSTRAINT [FK_ShipmentAttachments_Shipment]
GO
