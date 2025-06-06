USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentRiderDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentRiderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Consignee] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Shipper] [nvarchar](50) NULL,
	[TrackingNumber] [nvarchar](255) NULL,
	[Pieces] [int] NOT NULL,
	[WarehouseCode] [nvarchar](50) NOT NULL,
	[InvoiceNumber] [nvarchar](255) NULL,
	[InvoiceTotal] [float] NULL,
	[GrossWeightKg] [float] NOT NULL,
	[CubicFeet] [float] NOT NULL,
	[RiderId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentRider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentRiderDetails]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentRiderDetails_ShipmentRider] FOREIGN KEY([RiderId])
REFERENCES [dbo].[ShipmentRider] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentRiderDetails] CHECK CONSTRAINT [FK_ShipmentRiderDetails_ShipmentRider]
GO
